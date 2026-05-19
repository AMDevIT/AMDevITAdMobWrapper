using AMDevIT.Admob.Wrapper.iOSNative;
using Foundation;

namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Listeners
{
    internal class AppleRewardListener
        : NSObject, IOnRewardEarnedListener
    {
        #region Events

        public event EventHandler<AdReward>? RewardEarned;

        #endregion

        #region Methods

        public void Amount(string type, nint amount)
        {
            AdReward adReward = new(type, amount);
            this.RewardEarned?.Invoke(this, adReward);
        }

        #endregion
    }
}
