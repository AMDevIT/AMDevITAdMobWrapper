//
//  OnInitializedListener.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation

@objc public protocol OnInitializedListener: AnyObject {
    func onInitialized()
    func onInitializationFailed(error: String)
}
