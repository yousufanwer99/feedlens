namespace FeedLens.Helpers
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success") => new()
        {
            Data = data,
            IsSuccess = true,
            Message = message
        };

        public static ApiResponse<T> Failure(string message, List<string>? errors = null) => new()
        {
            Data = default,
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }
}
