//
//  OnConsentInformationRequestListener.swift
//  AdMobWrapper
//

import Foundation

@objc public protocol OnConsentInformationRequestListener: AnyObject {
    func onConsentInformationRequestSuccess()
    func onConsentInformationRequestFailure(errorCode: Int, errorMessage: String)
}
