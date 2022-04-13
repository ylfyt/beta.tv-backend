using src.Dtos;

public class ResponseGetter<T> : IResponseGetter<T>
{
    public ResponseDto<T> Success(T data)
    {
        return new ResponseDto<T>
        {
            success = true,
            data = data
        };
    }

    public ResponseDto<T> Error(string? msg = null)
    {
        return new ResponseDto<T>
        {
            message = msg ?? ""
        };
    }
}