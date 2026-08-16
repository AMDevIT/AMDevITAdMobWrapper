//
//  BannerAdWrapper.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds
import UIKit

@objc public enum BannerAdViewSize: Int {
    case adaptive
    case banner
    case largeBanner
    case mediumRectangle
    case fullBanner
    case leaderboard
}

@objc public class BannerAdWrapper: NSObject {

    private static let logTag = "BannerAdWrapper"
    
    private var bannerView: BannerView?
    private var loadListener: OnAdLoadedListener?
    private var eventListener: OnAdEventListener?
    private var logger: IAppleLogger?
    private var isDestroyed = false
    
    @objc public override init() {
        self.logger = nil
        super.init()
    }

    @objc public init(logger: IAppleLogger?) {
        self.logger = logger
        super.init()
    }
    
    @objc public func load(adUnitId: String,
                           viewController: UIViewController,
                           loadListener: OnAdLoadedListener,
                           eventListener: OnAdEventListener?) -> UIView {
        return self.load(adUnitId: adUnitId,
                         viewController: viewController,
                         adSize: .banner,
                         adWidth: 320,
                         loadListener: loadListener,
                         eventListener: eventListener)
    }

    @objc public func load(adUnitId: String,
                           viewController: UIViewController,
                           adSize: BannerAdViewSize,
                           adWidth: CGFloat,
                           loadListener: OnAdLoadedListener,
                           eventListener: OnAdEventListener?) -> UIView {
        if !Thread.isMainThread {
            return DispatchQueue.main.sync {
                self.load(adUnitId: adUnitId,
                          viewController: viewController,
                          adSize: adSize,
                          adWidth: adWidth,
                          loadListener: loadListener,
                          eventListener: eventListener)
            }
        }

        self.clearCurrentBanner()
        self.isDestroyed = false
        self.loadListener = loadListener
        self.eventListener = eventListener
        
        let banner = BannerView(adSize: self.nativeAdSize(for: adSize,
                                                          availableWidth: adWidth))
        banner.adUnitID = adUnitId
        banner.rootViewController = viewController
        banner.delegate = self
        self.bannerView = banner
        banner.load(Request())

        return banner
    }
    
    @objc public func destroy() {
        if !Thread.isMainThread {
            DispatchQueue.main.sync {
                self.destroy()
            }
            return
        }

        guard !self.isDestroyed || self.bannerView != nil else {
            return
        }

        self.isDestroyed = true
        self.clearCurrentBanner()
    }

    private func clearCurrentBanner() {
        let banner = self.bannerView

        self.bannerView = nil
        self.loadListener = nil
        self.eventListener = nil
        banner?.delegate = nil
        banner?.rootViewController = nil
        banner?.removeFromSuperview()
    }

    private func nativeAdSize(for adSize: BannerAdViewSize,
                              availableWidth: CGFloat) -> AdSize {
        switch adSize {
        case .adaptive:
            return largeAnchoredAdaptiveBanner(width: max(1, availableWidth))
        case .banner:
            return AdSizeBanner
        case .largeBanner:
            return AdSizeLargeBanner
        case .mediumRectangle:
            return AdSizeMediumRectangle
        case .fullBanner:
            return AdSizeFullBanner
        case .leaderboard:
            return AdSizeLeaderboard
        }
    }
}


extension BannerAdWrapper: BannerViewDelegate {
    
    public func bannerViewDidReceiveAd(_ bannerView: BannerView) {
        guard self.isCurrent(bannerView) else {
            return
        }

        self.loadListener?.onAdLoaded()
    }
    
    public func bannerView(_ bannerView: BannerView,
                           didFailToReceiveAdWithError error: Error) {
        guard self.isCurrent(bannerView) else {
            return
        }

        let nsError = error as NSError
        self.loadListener?.onAdFailedToLoad(errorCode: nsError.code,
                                            errorMessage: nsError.localizedDescription)
    }
    
    public func bannerViewDidRecordImpression(_ bannerView: BannerView) {
        guard self.isCurrent(bannerView) else {
            return
        }

        self.eventListener?.onAdImpression()
    }
    
    public func bannerViewDidRecordClick(_ bannerView: BannerView) {
        guard self.isCurrent(bannerView) else {
            return
        }

        self.eventListener?.onAdClicked()
    }
    
    public func bannerViewWillPresentScreen(_ bannerView: BannerView) {
        guard self.isCurrent(bannerView) else {
            return
        }

        self.eventListener?.onAdShown()
    }
    
    public func bannerViewWillDismissScreen(_ bannerView: BannerView) {
        guard self.isCurrent(bannerView) else {
            return
        }

        self.logger?.logDebug(message: "Banner will dismiss screen",
                              tag: Self.logTag)
    }

    public func bannerViewDidDismissScreen(_ bannerView: BannerView) {
        guard self.isCurrent(bannerView) else {
            return
        }

        self.eventListener?.onAdDismissed()
    }

    private func isCurrent(_ bannerView: BannerView) -> Bool {
        return !self.isDestroyed && bannerView === self.bannerView
    }
}

