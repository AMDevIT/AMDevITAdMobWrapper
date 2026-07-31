namespace AMDevIT.Admob.Wrapper.MAUICross;

public sealed class AdLoadException : Exception
{
    #region Properties

    public long ErrorCode { get; }

    #endregion

    #region .ctor

    public AdLoadException(long errorCode, string message)
        : base(message)
    {
        this.ErrorCode = errorCode;
    }

    #endregion

    #region Methods

    public override string ToString()
    {
        return $"[{this.ErrorCode}] {base.ToString()}";
    }

    #endregion
}
