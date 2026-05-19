using UIKit;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Helpers;

public static class ViewControllerHelper
{
    //public static UIViewController? GetTopViewController()
    //{
    //    // UIWindow? window = UIApplication.SharedApplication.KeyWindow;
    //    // UIViewController? vc = window?.RootViewController;

    //    UIViewController? viewController = UIApplication.SharedApplication
    //                                                    .ConnectedScenes
    //                                                    .OfType<UIWindowScene>()
    //                                                    .SelectMany(scene => scene.Windows)
    //                                                    .FirstOrDefault(w => w.IsKeyWindow)?
    //                                                    .RootViewController;


    //    while (viewController is { PresentedViewController: { } })
    //        viewController = viewController.PresentedViewController;

    //    if (viewController is UINavigationController { ViewControllers: { } } navController)
    //        viewController = navController.ViewControllers.Last();

    //    return viewController;
    //}

    public static UIViewController? GetTopViewController()
    {
        UIViewController? viewController = UIApplication.SharedApplication
                                                        .ConnectedScenes
                                                        .OfType<UIWindowScene>()
                                                        .SelectMany(scene => scene.Windows)
                                                        .FirstOrDefault(w => w.IsKeyWindow)?
                                                        .RootViewController;

        return GetTopViewController(viewController);
    }

    private static UIViewController? GetTopViewController(UIViewController? root)
    {
        if (root is UINavigationController nav)
            return GetTopViewController(nav.VisibleViewController);

        if (root is UITabBarController tab)
            return GetTopViewController(tab.SelectedViewController);

        if (root?.PresentedViewController != null)
            return GetTopViewController(root.PresentedViewController);

        return root;
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
