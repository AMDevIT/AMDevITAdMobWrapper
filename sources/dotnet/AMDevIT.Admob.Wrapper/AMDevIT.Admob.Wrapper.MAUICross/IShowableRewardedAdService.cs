namespace AMDevIT.Admob.Wrapper.MAUICross;

public interface IShowableRewardedAdService 
    : IFullScreenAdService
{
    #region Methods

    Task<AdReward> ShowAsync();
    void Show(Action<AdReward> onRewardEarned);

    #endregion
}
