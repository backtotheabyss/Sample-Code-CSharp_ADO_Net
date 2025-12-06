using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class DTO
    {
        public class Response<T>
        {
            [Required]
            public int TotalCount { get; set; }
            [Required]
            public List<T> Results { get; set; } = new()!;
            [Required] public bool Success { get; set; }
            [Required] public string ErrorMessage { get; set; } = "";
        }
        public class GeneralResponse<T>
        {
            [Required]
            public T Response { get; set; } = default!;
        }

        public class ErrorResponse
        {
            [Required]
            public string ErrorCode { get; set; } = "0";
            [Required]
            public string ErrorMessage { get; set; } = string.Empty;
            [Required]
            public List<ErrorDetail> ErrorDetails { get; set; } = new List<ErrorDetail>(); //[];
        }

        public class ErrorDetail
        {
            [Required]
            public int InternalErrorCode { get; set; }
            [Required]
            public string Detail { get; set; } = string.Empty;
        }
    }
}