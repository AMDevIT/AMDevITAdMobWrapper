namespace AMDevIT.Admob.Wrapper.MAUICross.Platforms.iOS.Listeners;

internal class AdFailedToShowArgs(long errorCode, string errorMessage)
    : EventArgs
{
    #region Properties

    public long ErrorCode => errorCode;

    public string ErrorMessage => errorMessage;

    #endregion

    #region Methods

    public override string ToString()
    {
        return $"ErrorCode={this.ErrorCode}, ErrorMessage={this.ErrorMessage}";
    }

    #endregion
}
