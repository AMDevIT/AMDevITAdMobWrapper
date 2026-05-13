using AMDevIT.Admob.Wrapper.MAUITestApp.ViewModels;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp
{
    public partial class MainPage : ContentPage
    {
        #region Properties

        protected ILogger<MainPage> Logger
        { 
            get; 
        }

        protected MainPageViewModel MainPageViewModel
        {
            get;
        }

        #endregion

        public MainPage(ILogger<MainPage> logger,
                        MainPageViewModel viewModel)
        {
            InitializeComponent();

            this.MainPageViewModel = viewModel;
            this.Logger = logger;
            this.BindingContext = this.MainPageViewModel;           
        }

        #region Methods

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            this.MainPageViewModel.RegisterEvents();
        }

        protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
        {
            base.OnNavigatingFrom(args);

            this.MainPageViewModel.UnregisterEvents();
        }

        #endregion

        #region Event Handlers

        private void BannerAd_AdLoaded(object? sender, EventArgs e)
        {
           Logger.LogDebug("Banner loaded");
        }

        private void BannerAd_AdFailed(object? sender, MAUICross.AdFailedEventArgs e)
        {
            Logger.LogDebug($"Banner failed: [{e.ErrorCode}] {e.ErrorMessage}");
        }

        #endregion
    }
}
