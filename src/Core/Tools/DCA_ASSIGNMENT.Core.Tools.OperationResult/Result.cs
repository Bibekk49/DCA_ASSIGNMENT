namespace DCA_ASSIGNMENT.Core.Tools.OperationResult;

public abstract record Result;
public abstract record Success : Result;
public abstract record Failure : Result;

public abstract record Result<T> : Result
 {
     public static implicit operator Result<T>(ResultError error) => new Failure<T>(new[] { error });
     public static implicit operator Result<T>(T value) => new Success<T>(value);
 }
 
public record Success<T>(T Value) : Result<T>;
public record Failure<T>(IEnumerable<ResultError> Errors) : Result<T>;
public record ResultError(string code, string Message);
public record None;