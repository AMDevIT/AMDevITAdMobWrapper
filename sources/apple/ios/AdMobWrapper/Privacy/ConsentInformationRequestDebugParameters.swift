//
//  ConsentInformationRequestDebugParameters.swift
//  AdMobWrapper
//

import Foundation

@objc public class ConsentInformationRequestDebugParameters: NSObject {
    @objc public let debugGeography: NSNumber?
    @objc public let testDeviceHashedId: String?

    @objc public override init() {
        self.debugGeography = nil
        self.testDeviceHashedId = nil
        super.init()
    }

    @objc public init(debugGeography: NSNumber?,
                      testDeviceHashedId: String?) {
        self.debugGeography = debugGeography
        self.testDeviceHashedId = testDeviceHashedId
        super.init()
    }

    @objc public override var description: String {
        return "ConsentInformationRequestDebugParameters(debugGeography=\(String(describing: self.debugGeography)), " +
               "testDeviceHashedId=\(String(describing: self.testDeviceHashedId)))"
    }
}
