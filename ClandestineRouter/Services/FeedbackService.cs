using ClandestineRouter.Common.Enums;
using ClandestineRouter.Common.Models;

namespace ClandestineRouter.Services;

public class FeedbackService
{
    private readonly List<FeedbackMessage> _messages = [];
    private readonly List<FeedbackMessage> _alertMessages = [];

    public event Action? OnChange;

    public IReadOnlyList<FeedbackMessage> Messages => _messages.AsReadOnly();
    public IReadOnlyList<FeedbackMessage> AlertMessages => _alertMessages.AsReadOnly();

    // Toast Methods (popup notifications)
    public void ShowSuccess(string message, string title = "Success", int duration = 5000)
    {
        AddToastMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Success,
            Duration = duration
        });
    }

    public void ShowError(string message, string title = "Error", int duration = 7000)
    {
        AddToastMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Error,
            Duration = duration
        });
    }

    public void ShowWarning(string message, string title = "Warning", int duration = 6000)
    {
        AddToastMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Warning,
            Duration = duration
        });
    }

    public void ShowInfo(string message, string title = "Info", int duration = 5000)
    {
        AddToastMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Info,
            Duration = duration
        });
    }

    public void ShowSuccessAlert(string message, string title = "Success", bool persistent = false)
    {
        AddAlertMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Success,
            IsPersistent = persistent,
            Duration = persistent ? 0 : 10000
        });
    }

    public void ShowErrorAlert(string message, string title = "Error", bool persistent = true)
    {
        AddAlertMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Error,
            IsPersistent = persistent,
            Duration = persistent ? 0 : 15000
        });
    }

    public void ShowWarningAlert(string message, string title = "Warning", bool persistent = false)
    {
        AddAlertMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Warning,
            IsPersistent = persistent,
            Duration = persistent ? 0 : 12000
        });
    }

    public void ShowInfoAlert(string message, string title = "Info", bool persistent = false)
    {
        AddAlertMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Info,
            IsPersistent = persistent,
            Duration = persistent ? 0 : 8000
        });
    }

    private void AddToastMessage(FeedbackMessage message)
    {
        _messages.Add(message);
        OnChange?.Invoke();
    }

    private void AddAlertMessage(FeedbackMessage message)
    {
        _alertMessages.Add(message);
        OnChange?.Invoke();
    }
}