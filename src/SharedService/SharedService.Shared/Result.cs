namespace SharedService.Shared;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public IEnumerable<string>? Error { get; private set; } = new List<string>();
    
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private Result(IEnumerable<string> errors)
    {
        IsSuccess = false;
        Error = errors;
        Value = default;
    }

    public static Result<T> Ok(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Fail(IEnumerable<string> errors)
    {
        return new Result<T>(errors);
    }
}