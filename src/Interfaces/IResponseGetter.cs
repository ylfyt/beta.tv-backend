using src.Dtos;

public interface IResponseGetter<T>
{
    public ResponseDto<T> Success(T data);

    public ResponseDto<T> Error(string? msg = null);
}