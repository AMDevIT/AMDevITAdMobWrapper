using System;
using UIKit;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Helpers;

public static class ViewControllerHelper
{
    public static UIViewController? GetTopViewController()
    {
        var window = UIApplication.SharedApplication.KeyWindow;
        var vc = window?.RootViewController;

        while (vc is { PresentedViewController: { } })
            vc = vc.PresentedViewController;

        if (vc is UINavigationController { ViewControllers: { } } navController)
            vc = navController.ViewControllers.Last();

        return vc;
    }

    public static UIWindow? GetKeyWindow(this UIApplication application)
    {
        if (!UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            return application.KeyWindow; // deprecated in iOS 13

        var window = application
            .ConnectedScenes
            .ToArray()
            .OfType<UIWindowScene>()
            .SelectMany(scene => scene.Windows)
            .FirstOrDefault(window => window.IsKeyWindow);

        return window;
    }

    public static UIViewController? GetViewController(UIView view)
    {
        UIResponder? responder = view.NextResponder;
        while (responder != null)
        {
            if (responder is UIViewController vc)
                return vc;
            responder = responder.NextResponder;
        }
        return null;
    }
}
