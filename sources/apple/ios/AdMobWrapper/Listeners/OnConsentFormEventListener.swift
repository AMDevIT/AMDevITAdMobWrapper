//
//  OnConsentFormEventListener.swift
//  AdMobWrapper
//

import Foundation

@objc public protocol OnConsentFormEventListener: AnyObject {
    func onDismissed()
    func onDismissedWithError(errorCode: Int, errorMessage: String?)
}
