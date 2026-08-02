using System.Windows.Input;

namespace AMDevIT.Admob.Wrapper.MAUICross
{
    public partial class BannerAd
        : View
    {
        #region Events

        public event EventHandler? AdLoaded;
        public event EventHandler<AdFailedEventArgs>? AdFailed;
        public event EventHandler? AdClicked;
        public event EventHandler? AdImpression;
        public event EventHandler? AdDismissed;

        #endregion

        #region Properties

        public static readonly BindableProperty AdUnitIdProperty = BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(BannerAd), null);

        public static readonly BindableProperty AdSizeProperty = BindableProperty.Create(nameof(AdSize), typeof(BannerAdSize), typeof(BannerAd), BannerAdSize.Adaptive);

        /// <summary>
        /// The static property that identifies <see cref="FallbackTemplate"/>.
        /// The template is rendered on platforms where AdMob banner ads aren't supported.
        /// </summary>
        public static readonly BindableProperty FallbackTemplateProperty = BindableProperty.Create(nameof(FallbackTemplate),
                                                                                                    typeof(DataTemplate),
                                                                                                    typeof(BannerAd),
                                                                                                    defaultValueCreator: CreateDefaultFallbackTemplate);

        /// <summary>
        /// The static property that identifies <see cref="AdLoadedCommand"/>. 
        /// AdLoadedCommand is executed when an ad is successfully loaded. The command parameter can be set using AdLoadedCommandParameter.
        /// </summary>
        public static readonly BindableProperty AdLoadedCommandProperty = BindableProperty.Create(nameof(AdLoadedCommand), typeof(ICommand), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdLoadedCommandParameter"/>. 
        /// The command parameter for AdLoadedCommand. 
        /// </summary>
        public static readonly BindableProperty AdLoadedCommandParameterProperty = BindableProperty.Create(nameof(AdLoadedCommandParameter), typeof(object), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdFailedCommand"/>.
        /// AdFailedCommand is executed when an ad fails to load. The command parameter can be set using AdFailedCommandParameter, which will receive an instance of <see cref="AdFailedEventArgs"/> 
        /// containing the error code and message.
        /// </summary>
        public static readonly BindableProperty AdFailedCommandProperty = BindableProperty.Create(nameof(AdFailedCommand), typeof(ICommand), typeof(BannerAd), null);
        /// <summary>
        /// The static property that identifies <see cref="AdFailedCommandParameter"/>.
        /// The command parameter for AdFailedCommand. 
        /// </summary>
        public static readonly BindableProperty AdFailedCommandParameterProperty = BindableProperty.Create(nameof(AdFailedCommandParameter), typeof(object), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdClickedCommand"/>.
        /// AdClickedCommand is executed when the user clicks on the ad. The command parameter can be set using AdClickedCommandParameter.
        /// </summary>
        public static readonly BindableProperty AdClickedCommandProperty = BindableProperty.Create(nameof(AdClickedCommand), typeof(ICommand), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdClickedCommandParameter"/>.
        /// The command parameter for AdClickedCommand.
        /// </summary>
        public static readonly BindableProperty AdClickedCommandParameterProperty = BindableProperty.Create(nameof(AdClickedCommandParameter), typeof(object), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdImpressionCommand"/>.
        /// AdImpressionCommand is executed when an impression is recorded for the ad. The command parameter can be set using AdImpressionCommandParameter.
        /// </summary>
        public static readonly BindableProperty AdImpressionCommandProperty = BindableProperty.Create(nameof(AdImpressionCommand), typeof(ICommand), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdImpressionCommandParameter"/>.
        /// The command parameter for AdImpressionCommand.
        /// </summary>
        public static readonly BindableProperty AdImpressionCommandParameterProperty = BindableProperty.Create(nameof(AdImpressionCommandParameter), typeof(object), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdDismissedCommand"/>.
        /// AdDismissedCommand is executed when the ad is dismissed. The command parameter can be set using AdDismissedCommandParameter.
        /// </summary>
        public static readonly BindableProperty AdDismissedCommandProperty = BindableProperty.Create(nameof(AdDismissedCommand), typeof(ICommand), typeof(BannerAd), null);

        /// <summary>
        /// The static property that identifies <see cref="AdDismissedCommandParameter"/>.
        /// The command parameter for AdDismissedCommand.
        /// </summary>
        public static readonly BindableProperty AdDismissedCommandParameterProperty = BindableProperty.Create(nameof(AdDismissedCommandParameter), typeof(object), typeof(BannerAd), null); 

        public string? AdUnitId
        {
            get => (string?)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }

        public BannerAdSize AdSize
        {
            get => (BannerAdSize)GetValue(AdSizeProperty);
            set => SetValue(AdSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the template rendered on platforms where AdMob banner ads aren't supported.
        /// The default template creates an empty <see cref="ContentView"/>.
        /// </summary>
        public DataTemplate? FallbackTemplate
        {
            get => (DataTemplate?)GetValue(FallbackTemplateProperty);
            set => SetValue(FallbackTemplateProperty, value);
        }

        /// <summary>
        /// AdLoadedCommand is executed when an ad is successfully loaded. The command parameter can be set using AdLoadedCommandParameter.
        /// </summary>
        public ICommand? AdLoadedCommand
        {
            get => (ICommand?)GetValue(AdLoadedCommandProperty);
            set => SetValue(AdLoadedCommandProperty, value);
        }

        /// <summary>
        /// The command parameter for AdLoadedCommand. 
        /// </summary>
        public object? AdLoadedCommandParameter
        {
            get => GetValue(AdLoadedCommandParameterProperty);
            set => SetValue(AdLoadedCommandParameterProperty, value);
        }

        /// <summary>
        /// AdFailedCommand is executed when an ad fails to load. The command parameter can be set using AdFailedCommandParameter, which will receive an instance of <see cref="AdFailedEventArgs"/> 
        /// containing the error code and message.
        /// </summary>
        public ICommand? AdFailedCommand
        {
            get => (ICommand?)GetValue(AdFailedCommandProperty);
            set => SetValue(AdFailedCommandProperty, value);
        }

        /// <summary>
        /// The command parameter for AdFailedCommand. 
        /// </summary>
        public object? AdFailedCommandParameter
        {
            get => GetValue(AdFailedCommandParameterProperty);
            set => SetValue(AdFailedCommandParameterProperty, value);
        }

        /// <summary>
        /// AdClickedCommand is executed when the user clicks on the ad. The command parameter can be set using AdClickedCommandParameter.
        /// </summary>
        public ICommand? AdClickedCommand
        {
            get => (ICommand?)GetValue(AdClickedCommandProperty);
            set => SetValue(AdClickedCommandProperty, value);
        }

        /// <summary>
        /// The command parameter for AdClickedCommand.
        /// </summary>
        public object? AdClickedCommandParameter
        {
            get => GetValue(AdClickedCommandParameterProperty);
            set => SetValue(AdClickedCommandParameterProperty, value);
        }

        /// <summary>
        /// AdImpressionCommand is executed when an impression is recorded for the ad. The command parameter can be set using AdImpressionCommandParameter.
        /// </summary>
        public ICommand? AdImpressionCommand
        {
            get => (ICommand?)GetValue(AdImpressionCommandProperty);
            set => SetValue(AdImpressionCommandProperty, value);
        }

        /// <summary>
        /// The command parameter for AdImpressionCommand.
        /// </summary>
        public object? AdImpressionCommandParameter
        {
            get => GetValue(AdImpressionCommandParameterProperty);
            set => SetValue(AdImpressionCommandParameterProperty, value);
        }

        /// <summary>
        /// AdDismissedCommand is executed when the ad is dismissed. The command parameter can be set using AdDismissedCommandParameter.
        /// </summary>
        public ICommand? AdDismissedCommand
        {
            get => (ICommand?)GetValue(AdDismissedCommandProperty);
            set => SetValue(AdDismissedCommandProperty, value);
        }

        /// <summary>
        /// The command parameter for AdDismissedCommand.
        /// </summary>
        public object? AdDismissedCommandParameter
        {
            get => GetValue(AdDismissedCommandParameterProperty);
            set => SetValue(AdDismissedCommandParameterProperty, value);
        }

        #endregion

        #region Methods

        internal void RaiseAdLoaded()
        {
            this.AdLoaded?.Invoke(this, EventArgs.Empty);
            ExecuteCommand(AdLoadedCommand, AdLoadedCommandParameter ?? EventArgs.Empty);
        }

        internal void RaiseAdFailed(int errorCode, string errorMessage)
        {
            AdFailedEventArgs args = new (errorCode, errorMessage);

            this.AdFailed?.Invoke(this, args);
            ExecuteCommand(AdFailedCommand, AdFailedCommandParameter ?? args);
        }

        internal void RaiseAdClicked()
        {
            this.AdClicked?.Invoke(this, EventArgs.Empty);
            ExecuteCommand(AdClickedCommand, AdClickedCommandParameter ?? EventArgs.Empty);
        }

        internal void RaiseAdImpression()
        {
            this.AdImpression?.Invoke(this, EventArgs.Empty);
            ExecuteCommand(AdImpressionCommand, AdImpressionCommandParameter ?? EventArgs.Empty);
        }

        internal void RaiseAdDismissed()
        {
            this.AdDismissed?.Invoke(this, EventArgs.Empty);
            ExecuteCommand(AdDismissedCommand, AdDismissedCommandParameter ?? EventArgs.Empty);
        }

        private static void ExecuteCommand(ICommand? command, object? parameter)
        {
            if (command?.CanExecute(parameter) == true)
                command.Execute(parameter);
        }

        private static object CreateDefaultFallbackTemplate(BindableObject _)
        {
            return new DataTemplate(() => new ContentView());
        }

        #endregion
    }
}
