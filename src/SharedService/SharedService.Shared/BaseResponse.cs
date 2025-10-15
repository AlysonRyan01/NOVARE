namespace SharedService.Shared;

public record BaseResponse<T>(T? Data, int StatusCode, string? Message = null)
{
    public bool IsSuccess =>  StatusCode >= 200 && StatusCode <= 299;
}