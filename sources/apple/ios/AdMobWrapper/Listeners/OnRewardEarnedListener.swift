//
//  OnRewardEarnedListener.swift
//  AdMobWrapper
//
//  Created by Alessandro Morvillo on 20/04/26.
//

import Foundation

@objc public protocol OnRewardEarnedListener: AnyObject {
    func onRewardEarned(type: String, amount: Int32)
}
