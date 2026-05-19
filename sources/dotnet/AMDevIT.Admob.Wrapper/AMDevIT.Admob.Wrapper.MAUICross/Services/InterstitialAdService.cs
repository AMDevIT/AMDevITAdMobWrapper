using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class InterstitialAdService
    : BaseFullScreenAdService, IInterstitialAdService
{
    #region Properties

    protected override string AdTypeName => "interstitial ad";

    #endregion  
}
