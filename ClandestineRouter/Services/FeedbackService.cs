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
        Console.WriteLine("Showing success toast: " + message); 
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
        Console.WriteLine("Showing error toast: " + message);
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
        Console.WriteLine("Showing warning toast: " + message);
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
        Console.WriteLine("Showing info toast: " + message);
        AddToastMessage(new FeedbackMessage
        {
            Title = title,
            Message = message,
            Type = FeedbackType.Info,
            Duration = duration
        });
    }

    // Alert Bar Methods (for top of page)
    public void ShowSuccessAlert(string message, string title = "Success", bool persistent = false)
    {
        Console.WriteLine("Showing success alert: " + message);
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
        Console.WriteLine("Showing error alert: " + message);
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
        Console.WriteLine("Showing warning alert: " + message);
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
        Console.WriteLine("Showing info alert: " + message);
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
        Console.WriteLine("Adding toast message: " + message.Message);
        _messages.Add(message);
        OnChange?.Invoke();

        if (!message.IsPersistent && message.Duration > 0)
        {
            _ = Task.Delay(message.Duration).ContinueWith(_ => RemoveMessage(message.Id));
        }
    }

    private void AddAlertMessage(FeedbackMessage message)
    {
        Console.WriteLine("Adding alert message: " + message.Message);
        _alertMessages.Add(message);
        OnChange?.Invoke();

        if (!message.IsPersistent && message.Duration > 0)
        {
            _ = Task.Delay(message.Duration).ContinueWith(_ => RemoveAlertMessage(message.Id));
        }
    }

    public void RemoveMessage(Guid id)
    {
        Console.WriteLine("Removing message with ID: " + id);
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _messages.Remove(message);
            OnChange?.Invoke();
        }
    }

    public void RemoveAlertMessage(Guid id)
    {
        Console.WriteLine("Removing alert message with ID: " + id);
        var message = _alertMessages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _alertMessages.Remove(message);
            OnChange?.Invoke();
        }
    }

    public void ClearAll()
    {
        Console.WriteLine("Clearing all messages and alerts");
        _messages.Clear();
        _alertMessages.Clear();
        OnChange?.Invoke();
    }

    public void ClearAlerts()
    {
        Console.WriteLine("Clearing all alert messages");
        _alertMessages.Clear();
        OnChange?.Invoke();
    }
}