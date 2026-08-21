//
//  AdMobManager.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds
import UIKit
import UserMessagingPlatform

@objc public class AdMobManager: NSObject {

    private static let logTag = "AdMobManager"

    @objc public static let instance = AdMobManager()

    private var initialized = false
    private var consentInformationAvailable = false
    private var lastConsentRefreshTimestampMilliseconds: Int64?
    private var logger: IAppleLogger?

    private override init() {
        self.logger = nil
        super.init()
    }

    @objc public convenience init(logger: IAppleLogger?) {
        self.init()
        self.logger = logger
    }

    @objc public func initialize(viewController: UIViewController,
                                 listener: OnInitializedListener) {
        self.initializeInternal(viewController: viewController,
                                ageTreatment: nil,
                                listener: listener)
    }

    @objc public func initialize(viewController: UIViewController,
                                 ageTreatment: AdMobAgeTreatment,
                                 listener: OnInitializedListener) {
        self.initializeInternal(viewController: viewController,
                                ageTreatment: ageTreatment,
                                listener: listener)
    }

    @objc public func isInitialized() -> Bool {
        return self.executeOnMainAndWait {
            self.initialized
        }
    }

    @objc public func updateCurrentConsentInformation(
        viewController: UIViewController,
        tagForUnderAgeOfConsent: Bool,
        listener: OnConsentInformationRequestListener
    ) {
        self.updateCurrentConsentInformation(
            viewController: viewController,
            tagForUnderAgeOfConsent: tagForUnderAgeOfConsent,
            listener: listener,
            requestDebugParameters: nil
        )
    }

    @objc public func updateCurrentConsentInformation(
        viewController: UIViewController,
        tagForUnderAgeOfConsent: Bool,
        listener: OnConsentInformationRequestListener,
        requestDebugParameters: ConsentInformationRequestDebugParameters?
    ) {
        self.executeOnMain { [weak self] in
            guard let self = self else { return }

            self.consentInformationAvailable = true

            let parameters = self.createConsentRequestParameters(
                tagForUnderAgeOfConsent: tagForUnderAgeOfConsent,
                requestDebugParameters: requestDebugParameters
            )

            ConsentInformation.shared.requestConsentInfoUpdate(with: parameters) {
                [weak self] requestConsentError in
                guard let self = self else { return }

                if let requestConsentError = requestConsentError {
                    let error = requestConsentError as NSError
                    listener.onConsentInformationRequestFailure(
                        errorCode: error.code,
                        errorMessage: error.localizedDescription
                    )
                    return
                }

                self.lastConsentRefreshTimestampMilliseconds = Int64(
                    Date().timeIntervalSince1970 * 1_000
                )
                listener.onConsentInformationRequestSuccess()
            }
        }
    }

    @objc public func currentConsentInformation() -> ConsentStatusData? {
        return self.executeOnMainAndWait {
            guard self.consentInformationAvailable else { return nil }

            let consentInformation = ConsentInformation.shared
            return ConsentStatusData(
                lastRefreshTimestampMilliseconds: self.lastConsentRefreshTimestampMilliseconds ?? 0,
                consentStatus: consentInformation.consentStatus.rawValue,
                privacyOptionsRequirementStatus: consentInformation.privacyOptionsRequirementStatus.rawValue
            )
        }
    }

    @objc public func showPrivacyOptionsForm(viewController: UIViewController,
                                             listener: OnConsentFormEventListener) {
        Task { @MainActor [weak self] in
            guard let self = self else { return }

            guard self.consentInformationAvailable else {
                listener.onDismissedWithError(
                    errorCode: -1,
                    errorMessage: "Consent information has not been updated."
                )
                return
            }

            guard ConsentInformation.shared.privacyOptionsRequirementStatus == .required else {
                listener.onDismissedWithError(
                    errorCode: -2,
                    errorMessage: "Privacy options form is not required."
                )
                return
            }

            do {
                try await ConsentForm.presentPrivacyOptionsForm(from: viewController)
                self.logger?.logDebug(
                    message: "Privacy options form dismissed successfully",
                    tag: Self.logTag
                )
                listener.onDismissed()
            } catch {
                let error = error as NSError
                self.logger?.logError(
                    message: "Error showing privacy options form: \(error.localizedDescription)",
                    tag: Self.logTag
                )
                listener.onDismissedWithError(
                    errorCode: error.code,
                    errorMessage: error.localizedDescription
                )
            }
        }
    }

    @objc public func loadAndShowConsentFormIfRequired(
        viewController: UIViewController,
        listener: OnConsentFormEventListener
    ) {
        Task { @MainActor [weak self] in
            guard let self = self else { return }

            do {
                try await ConsentForm.loadAndPresentIfRequired(from: viewController)
                self.logger?.logDebug(
                    message: "Required consent form dismissed successfully",
                    tag: Self.logTag
                )
                listener.onDismissed()
            } catch {
                let error = error as NSError
                self.logger?.logError(
                    message: "Error loading or showing the required consent form: " +
                             error.localizedDescription,
                    tag: Self.logTag
                )
                listener.onDismissedWithError(
                    errorCode: error.code,
                    errorMessage: error.localizedDescription
                )
            }
        }
    }

    @objc public func canRequestAds() -> Bool {
        return self.executeOnMainAndWait {
            guard self.consentInformationAvailable else { return false }
            return ConsentInformation.shared.canRequestAds
        }
    }

    @objc public func gatherConsent(
        viewController: UIViewController,
        tagForUnderAgeOfConsent: Bool,
        listener: OnConsentGatheringListener
    ) {
        self.gatherConsent(
            viewController: viewController,
            tagForUnderAgeOfConsent: tagForUnderAgeOfConsent,
            listener: listener,
            requestDebugParameters: nil
        )
    }

    @objc public func gatherConsent(
        viewController: UIViewController,
        tagForUnderAgeOfConsent: Bool,
        listener: OnConsentGatheringListener,
        requestDebugParameters: ConsentInformationRequestDebugParameters?
    ) {
        let requestListener = ConsentInformationRequestListenerAdapter(
            onSuccess: { [weak self] in
                guard let self = self else { return }

                self.loadAndShowConsentFormIfRequired(
                    viewController: viewController,
                    listener: ConsentFormEventListenerAdapter(
                        onDismissed: {
                            listener.onCompleted(
                                canRequestAds: self.canRequestAds(),
                                privacyOptionsRequired: self.isPrivacyOptionsRequired()
                            )
                        },
                        onDismissedWithError: { errorCode, errorMessage in
                            listener.onCompletedWithError(
                                errorCode: errorCode,
                                errorMessage: errorMessage ?? "Unknown error",
                                canRequestAds: self.canRequestAds(),
                                privacyOptionsRequired: self.isPrivacyOptionsRequired()
                            )
                        }
                    )
                )
            },
            onFailure: { [weak self] errorCode, errorMessage in
                guard let self = self else { return }

                listener.onCompletedWithError(
                    errorCode: errorCode,
                    errorMessage: errorMessage,
                    canRequestAds: self.canRequestAds(),
                    privacyOptionsRequired: self.isPrivacyOptionsRequired()
                )
            }
        )

        self.updateCurrentConsentInformation(
            viewController: viewController,
            tagForUnderAgeOfConsent: tagForUnderAgeOfConsent,
            listener: requestListener,
            requestDebugParameters: requestDebugParameters
        )
    }

    @objc public func resetConsentForTesting() {
        self.executeOnMainAndWait {
            ConsentInformation.shared.reset()
            self.lastConsentRefreshTimestampMilliseconds = nil
        }
    }

    private func initializeInternal(viewController: UIViewController,
                                    ageTreatment: AdMobAgeTreatment?,
                                    listener: OnInitializedListener) {
        self.executeOnMain { [weak self] in
            guard let self = self else { return }

            if let ageTreatment = ageTreatment {
                MobileAds.shared.requestConfiguration.ageRestrictedTreatment =
                    ageTreatment.nativeAgeRestrictedTreatment
            }

            if self.initialized {
                listener.onInitialized()
                return
            }

            MobileAds.shared.start { [weak self] _ in
                guard let self = self else { return }
                self.initialized = true
                listener.onInitialized()
            }
        }
    }

    private func createConsentRequestParameters(
        tagForUnderAgeOfConsent: Bool,
        requestDebugParameters: ConsentInformationRequestDebugParameters?
    ) -> RequestParameters {
        let parameters = RequestParameters()
        parameters.isTaggedForUnderAgeOfConsent = tagForUnderAgeOfConsent

        if let requestDebugParameters = requestDebugParameters {
            let debugSettings = DebugSettings()

            if let geographyValue = requestDebugParameters.debugGeography?.intValue,
               let geography = DebugGeography(rawValue: geographyValue) {
                debugSettings.geography = geography
            }

            if let testDeviceHashedId = requestDebugParameters.testDeviceHashedId,
               !testDeviceHashedId.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty {
                debugSettings.testDeviceIdentifiers = [testDeviceHashedId]
            }

            parameters.debugSettings = debugSettings
        }

        return parameters
    }

    private func isPrivacyOptionsRequired() -> Bool {
        return self.executeOnMainAndWait {
            guard self.consentInformationAvailable else { return false }
            return ConsentInformation.shared.privacyOptionsRequirementStatus == .required
        }
    }

    private func executeOnMain(_ action: @escaping () -> Void) {
        if Thread.isMainThread {
            action()
        } else {
            DispatchQueue.main.async(execute: action)
        }
    }

    @discardableResult
    private func executeOnMainAndWait<T>(_ action: () -> T) -> T {
        if Thread.isMainThread {
            return action()
        }

        return DispatchQueue.main.sync(execute: action)
    }
}

private extension AdMobAgeTreatment {
    var nativeAgeRestrictedTreatment: AgeRestrictedTreatment {
        switch self {
        case .unspecified:
            return .unspecified
        case .child:
            return .child
        case .teen:
            return .teen
        }
    }
}

private final class ConsentInformationRequestListenerAdapter: NSObject,
                                                              OnConsentInformationRequestListener {
    private let onSuccess: () -> Void
    private let onFailure: (Int, String) -> Void

    init(onSuccess: @escaping () -> Void,
         onFailure: @escaping (Int, String) -> Void) {
        self.onSuccess = onSuccess
        self.onFailure = onFailure
        super.init()
    }

    func onConsentInformationRequestSuccess() {
        self.onSuccess()
    }

    func onConsentInformationRequestFailure(errorCode: Int,
                                             errorMessage: String) {
        self.onFailure(errorCode, errorMessage)
    }
}

private final class ConsentFormEventListenerAdapter: NSObject,
                                                     OnConsentFormEventListener {
    private let onDismissedHandler: () -> Void
    private let onDismissedWithErrorHandler: (Int, String?) -> Void

    init(onDismissed: @escaping () -> Void,
         onDismissedWithError: @escaping (Int, String?) -> Void) {
        self.onDismissedHandler = onDismissed
        self.onDismissedWithErrorHandler = onDismissedWithError
        super.init()
    }

    func onDismissed() {
        self.onDismissedHandler()
    }

    func onDismissedWithError(errorCode: Int,
                              errorMessage: String?) {
        self.onDismissedWithErrorHandler(errorCode, errorMessage)
    }
}
