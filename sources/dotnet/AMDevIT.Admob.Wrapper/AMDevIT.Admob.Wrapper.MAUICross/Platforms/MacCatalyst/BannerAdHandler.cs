#if MACCATALYST

using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace AMDevIT.Admob.Wrapper.MAUICross;

public partial class BannerAdHandler
    : ViewHandler<BannerAd, UIView>
{
    #region Fields

    private View? fallbackView;

    #endregion

    #region Methods

    protected override UIView CreatePlatformView()
    {
        return new FallbackContainerView
        {
            BackgroundColor = UIColor.Clear
        };
    }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);

        this.VirtualView.BindingContextChanged += BannerAd_BindingContextChanged;

        if (this.fallbackView == null)
            this.UpdateFallbackTemplate();
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        this.VirtualView.BindingContextChanged -= BannerAd_BindingContextChanged;
        this.ClearFallbackView();

        base.DisconnectHandler(platformView);
    }

    partial void UpdateFallbackTemplate()
    {
        if (this.PlatformView is not FallbackContainerView container)
            return;

        this.ClearFallbackView();

        this.fallbackView = this.CreateFallbackView();
        container.Content = this.fallbackView.ToPlatform(this.MauiContext
                                                          ?? throw new InvalidOperationException("The MAUI context isn't available."));
    }

    private void ClearFallbackView()
    {
        if (this.PlatformView is FallbackContainerView container)
            container.Content = null;

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

    private sealed class FallbackContainerView
        : UIView
    {
        #region Fields

        private UIView? content;

        #endregion

        #region Properties

        public UIView? Content
        {
            get => this.content;
            set
            {
                if (ReferenceEquals(this.content, value))
                    return;

                this.content?.RemoveFromSuperview();
                this.content = value;

                if (this.content != null)
                    this.AddSubview(this.content);

                this.InvalidateIntrinsicContentSize();
                this.SetNeedsLayout();
            }
        }

        public override CGSize IntrinsicContentSize => this.content?.IntrinsicContentSize ?? CGSize.Empty;

        #endregion

        #region Methods

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (this.content != null)
                this.content.Frame = this.Bounds;
        }

        public override CGSize SizeThatFits(CGSize size)
        {
            return this.content?.SizeThatFits(size) ?? CGSize.Empty;
        }

        #endregion
    }
}

#endif
