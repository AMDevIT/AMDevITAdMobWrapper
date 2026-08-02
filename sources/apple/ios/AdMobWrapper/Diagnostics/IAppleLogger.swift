//
//  IAppleLogger.swift
//  AdMobWrapper
//

import Foundation

@objc public protocol IAppleLogger: AnyObject {
    func isEnabled(level: LogLevel) -> Bool

    func logTrace(message: String, tag: String?)
    func logDebug(message: String, tag: String?)
    func logInfo(message: String, tag: String?)
    func logWarning(message: String, tag: String?)
    func logError(message: String, tag: String?)
    func logCritical(message: String, tag: String?)
}
