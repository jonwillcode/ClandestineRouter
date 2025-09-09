namespace ClandestineRouter.Services.DataService;
#region Supporting Classes and Enums

public enum ServiceErrorType
{
    None,
    ValidationError,
    UnauthorizedAccess,
    NotFound,
    DatabaseError,
    ConcurrencyError,
    OperationCancelled,
    UnknownError
}

#endregion