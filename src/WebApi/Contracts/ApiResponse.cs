namespace WebApi.Contracts;

public class ApiResponse
{
    public object Data { get; set; }

    public ApiResponse(object data)
    {
        Data = data;
    }
}
