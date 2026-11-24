using System.Net.Http;
using Iniciosesion.Services;

namespace Iniciosesion.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;

        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Introduce el usuario y la contraseña.");
            return;
        }

        LoginButton.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var loginSucceeded = await _authService.LoginAsync(username, password);
            if (!loginSucceeded)
            {
                ShowError("El usuario o la contraseña no son correctos.");
                return;
            }

            await Navigation.PushAsync(new ProfilePage(_authService));
        }
        catch (HttpRequestException)
        {
            ShowError("No se pudo conectar con el servicio. Comprueba tu conexión e inténtalo de nuevo.");
        }
        catch (InvalidOperationException exception)
        {
            ShowError(exception.Message);
        }
        catch (Exception)
        {
            ShowError("Se ha producido un error inesperado. Inténtalo de nuevo.");
        }
        finally
        {
            LoginButton.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    public void ShowStartupError(string message)
    {
        ShowError(message);
    }

    public void ClearCredentials()
    {
        UsernameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        ErrorLabel.Text = string.Empty;
        ErrorLabel.IsVisible = false;
    }
}
