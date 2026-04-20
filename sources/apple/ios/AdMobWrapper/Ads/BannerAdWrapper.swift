//
//  BannerAdWrapper.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds
import UIKit

@objc public class BannerAdWrapper: NSObject {
    
    private var bannerView: BannerView?
    private var loadListener: OnAdLoadedListener?
    private var eventListener: OnAdEventListener?
    
    @objc public override init() {
        super.init()
    }
    
    @objc public func load(adUnitId: String,
                           viewController: UIViewController,
                           loadListener: OnAdLoadedListener,
                           eventListener: OnAdEventListener?) -> BannerView {
        self.bannerView?.removeFromSuperview()
        self.loadListener = loadListener
        self.eventListener = eventListener
        
        let banner = BannerView(adSize: AdSizeBanner)
        banner.adUnitID = adUnitId
        banner.rootViewController = viewController
        banner.delegate = self
        banner.load(Request())
        
        self.bannerView = banner
        return banner
    }
    
    @objc public func destroy() {
        self.bannerView?.removeFromSuperview()
        self.bannerView = nil
        self.loadListener = nil
        self.eventListener = nil
    }
}


extension BannerAdWrapper: BannerViewDelegate {
    
    public func bannerViewDidReceiveAd(_ bannerView: BannerView) {
        self.loadListener?.onAdLoaded()
    }
    
    public func bannerView(_ bannerView: BannerView,
                           didFailToReceiveAdWithError error: Error) {
        let nsError = error as NSError
        self.loadListener?.onAdFailedToLoad(errorCode: Int32(nsError.code),
                                            errorMessage: nsError.localizedDescription)
    }
    
    public func bannerViewDidRecordImpression(_ bannerView: BannerView) {
        self.eventListener?.onAdImpression()
    }
    
    public func bannerViewDidRecordClick(_ bannerView: BannerView) {
        self.eventListener?.onAdClicked()
    }
    
    public func bannerViewWillPresentScreen(_ bannerView: BannerView) {
        self.eventListener?.onAdShown()
    }
    
    public func bannerViewWillDismissScreen(_ bannerView: BannerView) {
        self.eventListener?.onAdDismissed()
    }
}

