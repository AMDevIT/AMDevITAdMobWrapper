using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUICross.Services;

public partial class AppOpenAdService
    : BaseFullScreenAdService, IAppOpenAdService
{
    #region Properties

    protected override string AdTypeName => "open app ad";

    #endregion
}
