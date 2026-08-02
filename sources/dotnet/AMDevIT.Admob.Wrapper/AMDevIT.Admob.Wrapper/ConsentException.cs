namespace AMDevIT.Admob.Wrapper;

public sealed class ConsentException : Exception
{
    #region Properties

    public int ErrorCode { get; }

    public bool? CanRequestAds { get; }

    public bool? PrivacyOptionsRequired { get; }

    #endregion

    #region .ctor

    public ConsentException(int errorCode,
                            string? message,
                            bool? canRequestAds = null,
                            bool? privacyOptionsRequired = null)
        : base(message ?? "An unknown consent error occurred.")
    {
        this.ErrorCode = errorCode;
        this.CanRequestAds = canRequestAds;
        this.PrivacyOptionsRequired = privacyOptionsRequired;
    }

    #endregion
}
