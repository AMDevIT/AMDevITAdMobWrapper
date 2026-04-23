#if ANDROID

using System;
using System.Collections.Generic;
using System.Text;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class RewardedAdService
{
    #region Properties

    public bool IsLoaded => throw new NotImplementedException();

    #endregion

    #region Methods

    public Task LoadAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Show(Action<AdReward> onRewardEarned)
    {
        throw new NotImplementedException();
    }

    public async Task ShowAsync(string adUnitId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AdReward> ShowAsync()
    {
        throw new NotImplementedException();
    }

    #endregion
}

#endif