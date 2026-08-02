using AMDevIT.Admob.Wrapper.AppleTestApp.Controllers;

namespace AMDevIT.Admob.Wrapper.AppleTestApp;

[Register("SceneDelegate")]
public class SceneDelegate : UIResponder, IUIWindowSceneDelegate
{
    #region Properties

    [Export("window")]
    public UIWindow? Window { get; set; }

    #endregion

    #region Methods

    [Export("scene:willConnectToSession:options:")]
    public void WillConnect(UIScene scene,
                            UISceneSession session,
                            UISceneConnectionOptions connectionOptions)
    {
        _ = session;
        _ = connectionOptions;

        if (scene is not UIWindowScene windowScene)
            return;

        this.Window ??= new UIWindow(windowScene);
        this.Window.RootViewController = new UINavigationController(new MainViewController());
        this.Window.MakeKeyAndVisible();
    }

    [Export("sceneDidDisconnect:")]
    public void DidDisconnect(UIScene scene)
    {
        _ = scene;
    }

    [Export("sceneDidBecomeActive:")]
    public void DidBecomeActive(UIScene scene)
    {
        _ = scene;
    }

    [Export("sceneWillResignActive:")]
    public void WillResignActive(UIScene scene)
    {
        _ = scene;
    }

    [Export("sceneWillEnterForeground:")]
    public void WillEnterForeground(UIScene scene)
    {
        _ = scene;
    }

    [Export("sceneDidEnterBackground:")]
    public void DidEnterBackground(UIScene scene)
    {
        _ = scene;
    }

    #endregion
}
