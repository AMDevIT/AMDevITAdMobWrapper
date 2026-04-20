//
//  RewardedAdWrapper.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds
import UIKit

@objc public class RewardedAdWrapper: NSObject {
    
    private var rewardedAd: RewardedAd?
    private var loadListener: OnAdLoadedListener?
    private var eventListener: OnAdEventListener?
    private var rewardListener: OnRewardEarnedListener?
    
    @objc public override init() {
        super.init()
    }
    
    @objc public func load(adUnitId: String,
                           loadListener: OnAdLoadedListener,
                           eventListener: OnAdEventListener?) {
        self.loadListener = loadListener
        self.eventListener = eventListener
        
        let request = Request()
        RewardedAd.load(with: adUnitId, request: request) { [weak self] ad, error in
            guard let self = self else { return }
            
            if let error = error {
                let nsError = error as NSError
                self.loadListener?.onAdFailedToLoad(errorCode: Int32(nsError.code),
                                                    errorMessage: nsError.localizedDescription)
                return
            }
            
            self.rewardedAd = ad
            self.rewardedAd?.fullScreenContentDelegate = self
            self.loadListener?.onAdLoaded()
        }
    }
    
    @objc public func show(viewController: UIViewController,
                           rewardListener: OnRewardEarnedListener) {
        guard let ad = self.rewardedAd else {
            self.loadListener?.onAdFailedToLoad(errorCode: -1,
                                                errorMessage: "Ad not loaded yet")
            return
        }
        
        self.rewardListener = rewardListener
        ad.present(from: viewController) { [weak self] in
            guard let self = self else { return }
            let reward = ad.adReward
            self.rewardListener?.onRewardEarned(type: reward.type,
                                                amount: Int32(truncating: reward.amount))
        }
    }
    
    @objc public func isLoaded() -> Bool {
        return self.rewardedAd != nil
    }
}

extension RewardedAdWrapper: FullScreenContentDelegate {
    
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
    
    public func adDidDismissFullScreenContent(_ ad: FullScreenPresentingAd) {
        self.rewardedAd = nil
        self.eventListener?.onAdDismissed()
    }
    
    public func ad(_ ad: FullScreenPresentingAd,
                   didFailToPresentFullScreenContentWithError error: Error) {
        let nsError = error as NSError
        self.eventListener?.onAdFailedToShow(errorCode: Int32(nsError.code),
                                             errorMessage: nsError.localizedDescription)
        self.rewardedAd = nil
    }
}
