namespace AMDevIT.Admob.Wrapper.MAUICross;

public interface IFullScreenAdService
{
    #region Events

    event EventHandler? AdLoaded;
    event EventHandler<AdFailedEventArgs>? AdFailed;
    event EventHandler? AdShown;
    event EventHandler? AdDismissed;
    event EventHandler? AdClicked;
    event EventHandler? AdImpression;
    event EventHandler<AdFailedEventArgs>? AdFailedToShow;

    #endregion

    #region Properties

    bool IsLoaded { get; }

    #endregion

    #region Methods

    Task LoadAsync(string adUnitId);

    #endregion
}
