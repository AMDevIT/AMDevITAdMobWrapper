#if WINDOWS

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using NativeGrid = Microsoft.UI.Xaml.Controls.Grid;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public partial class BannerAdHandler
    : ViewHandler<BannerAd, FrameworkElement>
{
    #region Fields

    private View? fallbackView;

    #endregion

    #region Methods

    protected override FrameworkElement CreatePlatformView()
    {
        return new NativeGrid();
    }

    protected override void ConnectHandler(FrameworkElement platformView)
    {
        base.ConnectHandler(platformView);

        this.VirtualView.BindingContextChanged += BannerAd_BindingContextChanged;

        if (this.fallbackView == null)
            this.UpdateFallbackTemplate();
    }

    protected override void DisconnectHandler(FrameworkElement platformView)
    {
        this.VirtualView.BindingContextChanged -= BannerAd_BindingContextChanged;
        this.ClearFallbackView();

        base.DisconnectHandler(platformView);
    }

    partial void UpdateFallbackTemplate()
    {
        if (this.PlatformView is not NativeGrid container)
            return;

        this.ClearFallbackView();

        this.fallbackView = this.CreateFallbackView();
        container.Children.Add(this.fallbackView.ToPlatform(this.MauiContext
                                                             ?? throw new InvalidOperationException("The MAUI context isn't available.")));
    }

    private void ClearFallbackView()
    {
        if (this.PlatformView is NativeGrid container)
            container.Children.Clear();

        if (this.fallbackView?.Handler != null)
        {
            this.fallbackView.Handler.DisconnectHandler();
            this.fallbackView.Handler = null;
        }

        this.fallbackView = null;
    }

    private View CreateFallbackView()
    {
        View fallbackView = this.VirtualView.FallbackTemplate?.CreateContent() as View
                            ?? new Microsoft.Maui.Controls.ContentView();

        fallbackView.BindingContext = this.VirtualView.BindingContext;

        return fallbackView;
    }

    private void UpdateFallbackBindingContext()
    {
        if (this.fallbackView != null)
            this.fallbackView.BindingContext = this.VirtualView.BindingContext;
    }

    #endregion

    #region Event handlers

    private void BannerAd_BindingContextChanged(object? sender, EventArgs e)
    {
        this.UpdateFallbackBindingContext();
    }

    #endregion
}

#endif
