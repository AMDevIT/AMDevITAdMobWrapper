//
//  AppOpenAdWrapper.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds
import UIKit

@objc public class AppOpenAdWrapper: NSObject {
    
    private var appOpenAd: AppOpenAd?
    private var isShowingAd: Bool = false
    private var loadListener: OnAdLoadedListener?
    private var eventListener: OnAdEventListener?
    
    @objc public override init() {
        super.init()
    }
    
    @objc public func load(adUnitId: String,
                           loadListener: OnAdLoadedListener,
                           eventListener: OnAdEventListener?) {
        self.loadListener = loadListener
        self.eventListener = eventListener
        
        let request = Request()
        AppOpenAd.load(with: adUnitId, request: request) { [weak self] ad, error in
            guard let self = self else { return }
            
            if let error = error {
                let nsError = error as NSError
                self.loadListener?.onAdFailedToLoad(errorCode: Int32(nsError.code),
                                                    errorMessage: nsError.localizedDescription)
                return
            }
            
            self.appOpenAd = ad
            self.appOpenAd?.fullScreenContentDelegate = self
            self.loadListener?.onAdLoaded()
        }
    }
    
    @objc public func show(viewController: UIViewController) {
        guard let ad = self.appOpenAd, !self.isShowingAd else {
            if self.appOpenAd == nil {
                self.loadListener?.onAdFailedToLoad(errorCode: -1,
                                                    errorMessage: "Ad not loaded yet")
            }
            return
        }
        ad.present(from: viewController)
    }
    
    @objc public func isLoaded() -> Bool {
        return self.appOpenAd != nil
    }
    
    @objc public func isShowing() -> Bool {
        return self.isShowingAd
    }
}

extension AppOpenAdWrapper: FullScreenContentDelegate {
    
    public func adDidRecordImpression(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdImpression()
    }
    
    public func adDidRecordClick(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdClicked()
    }
    
    public func adWillPresentFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.isShowingAd = true
        self.eventListener?.onAdShown()
    }
    
    public func adWillDismissFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.eventListener?.onAdDismissed()
    }
    
    public func adDidDismissFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.isShowingAd = false
        self.appOpenAd = nil
        self.eventListener?.onAdDismissed()
    }
    
    public func ad(_ ad: FullScreenPresentingAd,
                   didFailToPresentFullScreenContentWithError error: Error) {
        let nsError = error as NSError
        self.isShowingAd = false
        self.appOpenAd = nil
        self.eventListener?.onAdFailedToShow(errorCode: Int32(nsError.code),
                                             errorMessage: nsError.localizedDescription)
    }
}
