namespace AMDevIT.Admob.Wrapper.MAUICross;

public class AdFailedEventArgs(int errorCode, string errorMessage)
    : EventArgs
{
    #region Properties

    public int ErrorCode => errorCode;
    public string ErrorMessage => errorMessage;

    #endregion

    #region Methods

    public override string ToString()
    {
        return $"ErrorCode={ErrorCode}, ErrorMessage={ErrorMessage}";
    }

    #endregion
}
