using System;
using System.Collections.Generic;
using System.Text;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class RewardedAdService
    : IShowableRewardedAdService
{
    #region Events

    public event EventHandler? AdLoaded;
    public event EventHandler<AdFailedEventArgs>? AdFailed;
    public event EventHandler? AdShown;
    public event EventHandler? AdDismissed;
    public event EventHandler? AdClicked;
    public event EventHandler? AdImpression;
    public event EventHandler<AdFailedEventArgs>? AdFailedToShow;

    #endregion

    
}
