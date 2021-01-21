using Etimo.Id.Dtos;

namespace Etimo.Id.Client
{
    public class ResponseDto<T> where T : class
    {
        public ResponseDto() { }

        public ResponseDto(T data)
        {
            Data = data;
        }

        public ResponseDto(ErrorResponseDto error)
        {
            Data             = null;
            ErrorCode        = error?.error;
            ErrorDescription = error?.error_description;
        }

        public bool   Success          { get => StatusCode >= 200 && StatusCode < 300; }
        public T      Data             { get; set; }
        public int    StatusCode       { get; set; }
        public string ErrorCode        { get; set; }
        public string ErrorDescription { get; set; }
    }
}
