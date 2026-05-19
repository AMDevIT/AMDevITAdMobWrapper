namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class RewardedAdService
    : BaseFullScreenAdService, IShowableRewardedAdService
{
    #region Events

    public event EventHandler<AdReward>? AdRewardEarned;

    #endregion

    #region Properties

    protected override string AdTypeName => "rewarded ad";

    #endregion

    #region Methods

    protected void OnAdRewardEarned(AdReward adReward)
    {
        MainThread.BeginInvokeOnMainThread(() => this.AdRewardEarned?.Invoke(this, adReward));
    }

    #endregion
}
