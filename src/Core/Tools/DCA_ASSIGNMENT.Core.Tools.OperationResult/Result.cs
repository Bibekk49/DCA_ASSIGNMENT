namespace DCA_ASSIGNMENT.Core.Tools.OperationResult;

public abstract record Result
{
    public abstract IEnumerable<ResultError> GetErrors();
}

public abstract record Result<T> : Result
{
    public T? Payload => this is Success<T> s ? s.Value : default;

    public static implicit operator Result<T>(ResultError error)
        => new Failure<T>(new[] { error });

    public static implicit operator Result<T>(T value)
        => new Success<T>(value);
}

public record Success<T>(T Value) : Result<T>
{
    public override IEnumerable<ResultError> GetErrors() => Enumerable.Empty<ResultError>();
}

public record Failure<T>(IEnumerable<ResultError> Errors) : Result<T>
{
    public override IEnumerable<ResultError> GetErrors() => Errors;
}

public record ResultError(
    string Code,
    string Message,
    string Type
);

public record None;