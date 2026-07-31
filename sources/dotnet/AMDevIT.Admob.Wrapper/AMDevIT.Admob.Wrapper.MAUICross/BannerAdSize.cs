namespace AMDevIT.Admob.Wrapper.MAUICross
{
    public enum BannerAdSize
    {
        Adaptive,
        Banner,
        LargeBanner,
        MediumRectangle,
        FullBanner,
        Leaderboard
    }

    internal static class BannerAdSizeExtensions
    {
        #region Methods

        internal static bool TryGetFixedSize(this BannerAdSize adSize, out Size size)
        {
            size = adSize switch
            {
                BannerAdSize.Banner => new Size(320, 50),
                BannerAdSize.LargeBanner => new Size(320, 100),
                BannerAdSize.MediumRectangle => new Size(300, 250),
                BannerAdSize.FullBanner => new Size(468, 60),
                BannerAdSize.Leaderboard => new Size(728, 90),
                _ => Size.Zero
            };

            return adSize != BannerAdSize.Adaptive;
        }

        #endregion
    }
}
