using ClandestineRouter.Common.Enums;

namespace ClandestineRouter.Common.Models;

//TODO Add comments to this class
public class FeedbackMessage
{
    public FeedbackMessage()
    {
        Id = Guid.NewGuid();
    }

    public FeedbackMessage(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public FeedbackType Type { get; set; } = FeedbackType.Info;
    public int Duration { get; set; } = 5000;
    public bool IsPersistent { get; set; } = false;
    public DateTime CreatedDateTimeUtc { get; set; } = DateTime.UtcNow;
    public bool IsVisible { get; set; } = false;
}