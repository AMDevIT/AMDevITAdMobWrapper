#if WINDOWS || MACCATALYST

using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class AppOpenAdService
{
    #region .ctor

    public AppOpenAdService(ILogger<AppOpenAdService> logger,
                            IContextResolverService contextResolverService)
        : base(logger, contextResolverService)
    {
    }

    #endregion

    #region Methods

    public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        return Task.FromException(CreatePlatformNotSupportedException());
    }

    public override void Show()
    {
        throw CreatePlatformNotSupportedException();
    }

    protected override void DisposeObjects()
    {
    }

    private static PlatformNotSupportedException CreatePlatformNotSupportedException()
    {
        return new PlatformNotSupportedException("App open ads are only supported on Android and iOS.");
    }

    #endregion
}

public partial class InterstitialAdService
{
    #region .ctor

    public InterstitialAdService(ILogger<InterstitialAdService> logger,
                                 IContextResolverService contextResolverService)
        : base(logger, contextResolverService)
    {
    }

    #endregion

    #region Methods

    public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        return Task.FromException(CreatePlatformNotSupportedException());
    }

    public override void Show()
    {
        throw CreatePlatformNotSupportedException();
    }

    protected override void DisposeObjects()
    {
    }

    private static PlatformNotSupportedException CreatePlatformNotSupportedException()
    {
        return new PlatformNotSupportedException("Interstitial ads are only supported on Android and iOS.");
    }

    #endregion
}

public partial class RewardedAdService
{
    #region .ctor

    public RewardedAdService(ILogger<AppOpenAdService> logger,
                             IContextResolverService contextResolverService)
        : base(logger, contextResolverService)
    {
    }

    #endregion

    #region Methods

    public override Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        return Task.FromException(CreatePlatformNotSupportedException());
    }

    public override void Show()
    {
        throw CreatePlatformNotSupportedException();
    }

    protected override void DisposeObjects()
    {
    }

    private static PlatformNotSupportedException CreatePlatformNotSupportedException()
    {
        return new PlatformNotSupportedException("Rewarded ads are only supported on Android and iOS.");
    }

    #endregion
}

#endif
