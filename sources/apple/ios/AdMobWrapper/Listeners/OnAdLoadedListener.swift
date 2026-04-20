//
//  OnAdLoadedListener.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation

@objc public protocol OnAdLoadedListener: AnyObject {
    func onAdLoaded()
    func onAdFailedToLoad(errorCode: Int32, errorMessage: String)
}
