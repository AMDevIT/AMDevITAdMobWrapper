namespace AMDevIT.Admob.Wrapper.MAUICross;

public class AdReward(string type, int amount)
{
    #region Properties

    public string Type => type;

    public int Amount => amount;

    #endregion

    #region Methods

    public override string ToString()
    {
        return $"Type: {Type}, Amount: {Amount}";
    }

    #endregion
}
