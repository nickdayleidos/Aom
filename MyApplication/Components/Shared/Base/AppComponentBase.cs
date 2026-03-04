using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using MyApplication.Components.Common.Auth;

namespace MyApplication.Components.Shared.Base;

public abstract class AppComponentBase : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    // Helper methods
    protected void ShowSuccess(string message)
    {
        Snackbar.Add(message, Severity.Success);
    }

    protected void ShowError(string message)
    {
        Snackbar.Add(message, Severity.Error);
    }

    protected void ShowWarning(string message)
    {
        Snackbar.Add(message, Severity.Warning);
    }

    protected void ShowInfo(string message)
    {
        Snackbar.Add(message, Severity.Info);
    }

    protected void NavigateTo(string uri, bool forceLoad = false)
    {
        Navigation.NavigateTo(uri, forceLoad);
    }

    protected Task<UserRoles> GetUserRolesAsync()
        => AuthStateProvider.GetUserRolesAsync();

    // Loading state — bind @if (IsLoading) in markup; wrap data loads with RunAsync
    protected bool IsLoading { get; private set; }

    protected async Task RunAsync(Func<Task> operation)
    {
        IsLoading = true;
        StateHasChanged();
        try   { await operation(); }
        finally { IsLoading = false; StateHasChanged(); }
    }

    protected async Task<bool> ConfirmAsync(string message, string title = "Confirm")
    {
        var parameters = new DialogParameters<MudBlazor.DialogOptions>
        {
            { "ContentText", message },
            { "ButtonText", "Confirm" },
            { "Color", Color.Primary }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small };

        var dialog = DialogService.Show<MudMessageBox>(title, parameters, options);
        var result = await dialog.Result;
        return result != null && !result.Canceled;
    }
}
