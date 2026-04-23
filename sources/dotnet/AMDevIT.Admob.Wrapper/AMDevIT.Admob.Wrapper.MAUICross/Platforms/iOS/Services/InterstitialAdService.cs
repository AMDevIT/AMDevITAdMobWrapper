#if IOS

using AMDevIT.Admob.Wrapper.iOSNative;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class InterstitialAdService
{
    #region Fields

    private InterstitialAdWrapper? wrapper;

    #endregion

    #region Properties

    public bool IsShowing => throw new NotImplementedException();

    public bool IsLoaded => this.wrapper?.IsLoaded ?? false;

    #endregion

    #region .ctor

    public InterstitialAdService(ILogger<InterstitialAdService> logger)
    {
        this.Logger = logger;
    }

    #endregion

    #region Methods

    public Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Show()
    {
        throw new NotImplementedException();
    }

    public async Task ShowAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected virtual void DisposeObjects()
    {

    }

    #endregion
}

#endif