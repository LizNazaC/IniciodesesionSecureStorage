using System.Net.Http;
using Iniciosesion.Services;
using Iniciosesion.Views;

namespace Iniciosesion
{
    public partial class App : Application
    {
        private readonly AuthService _authService;

        public App(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var loginPage = new LoginPage(_authService);
            var navigationPage = new NavigationPage(loginPage);
            var window = new Window(navigationPage);
            window.Created += async (_, _) => await RestoreSessionAsync(navigationPage, loginPage);
            return window;
        }

        private async Task RestoreSessionAsync(NavigationPage navigationPage, LoginPage loginPage)
        {
            try
            {
                var profile = await _authService.GetCurrentUserAsync();
                if (profile is not null)
                {
                    await navigationPage.PushAsync(new ProfilePage(_authService, profile));
                }
            }
            catch (HttpRequestException)
            {
                loginPage.ShowStartupError("No se pudo comprobar la sesión. Comprueba tu conexión e inténtalo de nuevo.");
            }
            catch (InvalidOperationException exception)
            {
                loginPage.ShowStartupError(exception.Message);
            }
            catch (Exception)
            {
                loginPage.ShowStartupError("No se pudo restaurar la sesión. Inicia sesión de nuevo.");
            }
        }
    }
}