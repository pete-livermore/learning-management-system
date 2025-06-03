namespace Application.Common.Errors;

public sealed record Error(ErrorType Type, string Code, string Description)
{
    public static readonly Error None = new(ErrorType.None, string.Empty, string.Empty);
}
