using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AMDevIT.Admob.Wrapper.MAUITestApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            MainPage? mainPage = IPlatformApplication.Current?.Services.GetRequiredService<MainPage>();
            NavigationPage navigationPage = new (mainPage);
            return new Window(navigationPage);
        }
    }
}