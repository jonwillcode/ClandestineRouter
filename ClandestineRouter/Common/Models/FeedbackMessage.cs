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
    public DateTime CreatedDateTimeUtc { get; set; } = DateTime.UtcNow;
}