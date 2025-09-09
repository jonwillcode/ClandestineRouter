namespace ClandestineRouter.Services.DataService;
#region Supporting Classes and Enums

public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public ServiceErrorType ErrorType { get; private set; }

    private ServiceResult(bool isSuccess, T? data, string? errorMessage, ServiceErrorType errorType)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static ServiceResult<T> Success(T data) => new(true, data, null, ServiceErrorType.None);
    public static ServiceResult<T> Failure(string errorMessage, ServiceErrorType errorType) => new(false, default, errorMessage, errorType);
}

#endregion