using AMDevIT.Admob.Wrapper.Listeners;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.Android.Listeners
{
    internal class DroidRewardListener
        : Java.Lang.Object, IOnRewardEarnedListener
    {
        #region Events

        public event EventHandler<AdReward>? RewardEarned;

        #endregion

        #region Methods

        public void OnRewardEarned(string type, int amount)
        {
            AdReward adReward = new(type, amount);
            this.RewardEarned?.Invoke(this, adReward);
        }

        #endregion
    }
}
