namespace MapeamentoUrbano.Domain.DTOs.Base
{
    public class ResponseDTO<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Entity { get; set; }
        public List<string>? ValidationErrors { get; set; }

        public ResponseDTO()
        {
            ValidationErrors = new List<string>();
        }

        public ResponseDTO(int statusCode)
            : this()
        {
            StatusCode = statusCode;
        }

        public ResponseDTO(int statusCode, string? message = default, T? entity = default)
            : this()
        {
            StatusCode = statusCode;
            Message = message;
            Entity = entity;
        }

        public ResponseDTO(int statusCode, T? entity = default)
            : this()
        {
            StatusCode = statusCode;
            Entity = entity;
        }

        public ResponseDTO(int statusCode, List<string> validationErrors = default)
            : this()
        {
            StatusCode = statusCode;
            ValidationErrors = validationErrors;
        }

        public ResponseDTO(int statusCode, List<string> validationErrors, string? message = default)
            : this(statusCode, message)
        {
            ValidationErrors = validationErrors;
        }
    }
}
