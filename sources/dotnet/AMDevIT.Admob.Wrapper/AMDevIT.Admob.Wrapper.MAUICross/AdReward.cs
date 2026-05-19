namespace AMDevIT.Admob.Wrapper.MAUICross;

public class AdReward(string type, long amount)
{
    #region Properties

    public string Type => type;

    public long Amount => amount;

    #endregion

    #region Methods

    public override string ToString()
    {
        return $"Type: {Type}, Amount: {Amount}";
    }

    #endregion
}
