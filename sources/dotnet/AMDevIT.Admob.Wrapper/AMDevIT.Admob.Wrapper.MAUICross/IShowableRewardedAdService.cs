namespace AMDevIT.Admob.Wrapper.MAUICross;

public interface IShowableRewardedAdService
    : IShowableAdService
{
    #region Events

    event EventHandler<AdReward>? AdRewardEarned;

    #endregion
}
