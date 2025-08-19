using ClandestineRouter.Common.Enums;
using ClandestineRouter.Common.Models;

namespace ClandestineRouter.Services;

public class FeedbackService
{
    private readonly List<FeedbackMessage> _messages = [];
    public event Action? OnChange;

    public IReadOnlyList<FeedbackMessage> Messages => _messages.AsReadOnly();

    public void ShowAlert(FeedbackType feedbackType, string message)
    {
        AddAlertMessage(new FeedbackMessage
        {
            Id = Guid.NewGuid(), // Ensure each message has a unique ID
            Title = feedbackType.ToString(),
            Message = message,
            Type = feedbackType,
        });
    }

    public void ShowAlert(FeedbackType feedbackType, string title, string message)
    {
        AddAlertMessage(new FeedbackMessage
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = feedbackType,
        });
    }

    private void AddAlertMessage(FeedbackMessage message)
    {
        _messages.Add(message);
        OnChange?.Invoke();
    }

    public void RemoveMessage(Guid messageId)
    {
        var message = _messages.FirstOrDefault(m => m.Id == messageId);
        if (message != null)
        {
            _messages.Remove(message);
            OnChange?.Invoke();
        }
    }

    public void ClearAllMessages()
    {
        _messages.Clear();
        OnChange?.Invoke();
    }
}