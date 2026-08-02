//
//  ConsentStatusData.swift
//  AdMobWrapper
//

import Foundation

@objc public class ConsentStatusData: NSObject {
    @objc public let lastRefreshTimestampMilliseconds: Int64
    @objc public let consentStatus: Int
    @objc public let privacyOptionsRequirementStatus: Int

    @objc public init(lastRefreshTimestampMilliseconds: Int64,
                      consentStatus: Int,
                      privacyOptionsRequirementStatus: Int) {
        self.lastRefreshTimestampMilliseconds = lastRefreshTimestampMilliseconds
        self.consentStatus = consentStatus
        self.privacyOptionsRequirementStatus = privacyOptionsRequirementStatus
        super.init()
    }

    @objc public override var description: String {
        return "ConsentStatusData(lastRefreshTimestamp=\(self.lastRefreshTimestampMilliseconds), " +
               "consentStatus=\(self.consentStatus), " +
               "privacyOptionsRequirementStatus=\(self.privacyOptionsRequirementStatus))"
    }
}
