using System;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp.ViewModels;

public class MainPageViewModel(ILogger<MainPageViewModel> logger)
    : ViewModelBase(logger)
{
    #region Commands

    private AsyncRelayCommand? showInterstitialAdCommand;

    #endregion

    #region Methods

    private async Task ShowInterstitialAd()
    {
        this.Logger.LogDebug("Show interstitial ad");

        
    }

    #endregion
}
