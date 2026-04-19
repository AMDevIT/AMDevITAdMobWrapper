namespace AMDevIT.Admob.Wrapper
{
    public class AdException(int errorCode, string message) 
        : Exception(message)
    {
        #region Properties

        public int ErrorCode { get; } = errorCode;

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{base.ToString()}, ErrorCode: {ErrorCode}";
        }

        #endregion
    }
}
