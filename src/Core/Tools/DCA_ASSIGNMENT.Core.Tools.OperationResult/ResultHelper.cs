namespace DCA_ASSIGNMENT.Core.Tools.OperationResult;

public static class ResultHelper
{

    public static Result<T> Success<T>(T value)
        => new Success<T>(value);

    public static Result<None> Success()
        => new Success<None>(new None());

    public static Result<T> Failure<T>(ResultError error)
        => new Failure<T>(new[] { error });

    public static Result<T> Failure<T>(IEnumerable<ResultError> errors)
        => new Failure<T>(errors);
    

    public static Result<None> Combine(params Result<None>[] results)
    {
        var errors = new List<ResultError>();

        foreach (var result in results)
        {
            if (result is Failure<None> failure)
                errors.AddRange(failure.Errors);
        }

        if (errors.Count > 0)
            return new Failure<None>(errors);

        return new Success<None>(new None());
    }

    public static Result<None> Combine<T>(params Result<T>[] results)
    {
        var errors = new List<ResultError>();

        foreach (var result in results)
        {
            if (result is Failure<T> failure)
                errors.AddRange(failure.Errors);
        }

        if (errors.Count > 0)
            return new Failure<None>(errors);

        return new Success<None>(new None());
    }
    
    
    public class ResultCombiner<T>
    {
        private readonly List<ResultError> _errors;

        internal ResultCombiner(List<ResultError> errors) => _errors = errors;

        public Result<T> WithPayloadIfSuccess(Func<T> factory)
            => _errors.Count > 0
                ? new Failure<T>(_errors)
                : new Success<T>(factory());
    }

    public static ResultCombiner<T> CombineResultsInto<T>(params Result[] results)
    {
        var errors = results.SelectMany(r => r.GetErrors()).ToList();
        return new ResultCombiner<T>(errors);
    }
}