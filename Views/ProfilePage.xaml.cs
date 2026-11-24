using System.Net.Http;
using Iniciosesion.Models;
using Iniciosesion.Services;

namespace Iniciosesion.Views;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    private UserProfile? _initialProfile;

    public ProfilePage(AuthService authService, UserProfile? initialProfile = null)
    {
        InitializeComponent();
        _authService = authService;
        _initialProfile = initialProfile;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialProfile is not null)
        {
            DisplayProfile(_initialProfile);
            _initialProfile = null;
            return;
        }

        await LoadProfileAsync();
    }

    private async void OnRetryClicked(object? sender, EventArgs e)
    {
        await LoadProfileAsync();
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        try
        {
            _authService.Logout();
            ClearProfileData();
            if (Navigation.NavigationStack.FirstOrDefault() is LoginPage loginPage)
            {
                loginPage.ClearCredentials();
            }

            await Navigation.PopToRootAsync();
        }
        catch (Exception)
        {
            ShowError("No se pudo cerrar la sesión de forma segura. Inténtalo de nuevo.");
        }
    }

    private async Task LoadProfileAsync()
    {
        ErrorLabel.IsVisible = false;
        RetryButton.IsVisible = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            var profile = await _authService.GetCurrentUserAsync();
            if (profile is null)
            {
                await Navigation.PopToRootAsync();
                return;
            }

            DisplayProfile(profile);
        }
        catch (HttpRequestException)
        {
            ShowError("No se pudo conectar para consultar el perfil. Comprueba tu conexión.");
            RetryButton.IsVisible = true;
        }
        catch (InvalidOperationException exception)
        {
            ShowError(exception.Message);
            RetryButton.IsVisible = true;
        }
        catch (Exception)
        {
            ShowError("No se pudo cargar el perfil. Inténtalo de nuevo.");
            RetryButton.IsVisible = true;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void DisplayProfile(UserProfile profile)
    {
        FullNameLabel.Text = $"{profile.FirstName} {profile.LastName}".Trim();
        UsernameLabel.Text = $"@{profile.Username}";
        EmailLabel.Text = profile.Email;
        ProfileImage.Source = Uri.TryCreate(profile.Image, UriKind.Absolute, out var imageUri)
            ? ImageSource.FromUri(imageUri)
            : null;
        ErrorLabel.IsVisible = false;
        RetryButton.IsVisible = false;
    }

    private void ClearProfileData()
    {
        FullNameLabel.Text = string.Empty;
        UsernameLabel.Text = string.Empty;
        EmailLabel.Text = string.Empty;
        ProfileImage.Source = null;
        ErrorLabel.Text = string.Empty;
        ErrorLabel.IsVisible = false;
        RetryButton.IsVisible = false;
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}
