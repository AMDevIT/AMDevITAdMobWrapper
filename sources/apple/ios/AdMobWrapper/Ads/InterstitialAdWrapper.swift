//
//  InterstitialAdWrapper.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds
import UIKit

@objc public class InterstitialAdWrapper: NSObject {
    
    internal var interstitialAd: InterstitialAd?
    internal var loadListener: OnAdLoadedListener?
    internal var eventListener: OnAdEventListener?
    
    @objc public override init() {
        super.init()
    }
    
    @objc public func load(adUnitId: String,
                           loadListener: OnAdLoadedListener,
                           eventListener: OnAdEventListener?) {
        self.loadListener = loadListener
        self.eventListener = eventListener
        
        let request = Request()
        InterstitialAd.load(with: adUnitId, request: request) { [weak self] ad, error in
            guard let self = self else { return }
            
            if let error = error {
                let nsError = error as NSError
                self.loadListener?.onAdFailedToLoad(errorCode: nsError.code,
                                                    errorMessage: nsError.localizedDescription)
                return
            }
            
            self.interstitialAd = ad
            self.interstitialAd?.fullScreenContentDelegate = self
            self.loadListener?.onAdLoaded()
        }
    }
    
    @objc public func show(viewController: UIViewController) {
        guard let ad = self.interstitialAd else {
            self.loadListener?.onAdFailedToLoad(errorCode: -1,
                                                errorMessage: "Ad not loaded yet")
            return
        }
        ad.present(from: viewController)
    }
    
    @objc public func isLoaded() -> Bool {
        return self.interstitialAd != nil
    }
}

extension InterstitialAdWrapper: FullScreenContentDelegate {
    
    public func adDidRecordImpression(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdImpression()
    }
    
    public func adDidRecordClick(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdClicked()
    }
    
    public func adWillPresentFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdShown()
    }
    
    public func adWillDismissFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdDismissed()
    }
    
    public func ad(_ ad: FullScreenPresentingAd,
                   didFailToPresentFullScreenContentWithError error: Error) {
        let nsError = error as NSError
        self.eventListener?.onAdFailedToShow(errorCode: nsError.code,
                                             errorMessage: nsError.localizedDescription)
        self.interstitialAd = nil
    }
    
    public func adDidDismissFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.interstitialAd = nil
        self.eventListener?.onAdDismissed()
    }
}
