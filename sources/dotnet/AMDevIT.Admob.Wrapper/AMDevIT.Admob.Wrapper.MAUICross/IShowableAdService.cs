namespace AMDevIT.Admob.Wrapper.MAUICross;

public interface IShowableAdService
    : IFullScreenAdService
{
    #region Properties

    bool IsShowing { get; }

    #endregion

    #region Methods

    void Show();
    Task LoadAndShowAsync(string adUnitId, CancellationToken cancellationToken = default);

    #endregion
}
