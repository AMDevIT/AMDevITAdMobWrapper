//
//  AdMobManager.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation
import GoogleMobileAds

@objc public class AdMobManager: NSObject {
    
    @objc public static let instance = AdMobManager()
    
    private var initialized = false
    
    private override init() {
        super.init()
    }
    
    @objc public func initialize(viewController: UIViewController,
                                 listener: OnInitializedListener) {
        if self.initialized {
            listener.onInitialized()
            return
        }
        
        MobileAds.shared.start { [weak self] status in
            guard let self = self else { return }
            self.initialized = true
            listener.onInitialized()
        }
    }
    
    @objc public func isInitialized() -> Bool {
        return self.initialized
    }
}
