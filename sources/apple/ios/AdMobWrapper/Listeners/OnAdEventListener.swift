//
//  OnAdEventListener.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation

@objc public protocol OnAdEventListener: AnyObject {
    func onAdShown()
    func onAdDismissed()
    func onAdClicked()
    func onAdImpression()
    func onAdFailedToShow(errorCode: Int, errorMessage: String)
}
