//
//  OnConsentGatheringListener.swift
//  AdMobWrapper
//

import Foundation

@objc public protocol OnConsentGatheringListener: AnyObject {
    func onCompleted(canRequestAds: Bool,
                     privacyOptionsRequired: Bool)

    func onCompletedWithError(errorCode: Int,
                              errorMessage: String,
                              canRequestAds: Bool,
                              privacyOptionsRequired: Bool)
}
