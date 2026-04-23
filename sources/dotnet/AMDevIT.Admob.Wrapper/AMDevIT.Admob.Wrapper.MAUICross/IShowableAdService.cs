namespace AMDevIT.Admob.Wrapper.MAUICross;

public interface IShowableAdService
    : IFullScreenAdService
{
    #region Properties

    bool IsShowing { get; }

    #endregion

    #region Methods

    void Show();

    #endregion
}
