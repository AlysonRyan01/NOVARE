namespace SharedService.Shared;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public IEnumerable<string>? Errors { get; private set; }
    
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Errors = null;
    }

    private Result(IEnumerable<string> errors)
    {
        IsSuccess = false;
        Errors = errors;
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