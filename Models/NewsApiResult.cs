using System.Net;

namespace NewsExplorerApp.Models
{
    public class NewsApiResult<T>
    {
        public bool IsSuccess { get; init; }
        public HttpStatusCode? StatusCode { get; init; }
        public T? Data { get; init; }

        public static NewsApiResult<T> Success(T data) =>
            new() { IsSuccess = true, Data = data };

        public static NewsApiResult<T> Fail(HttpStatusCode statusCode) =>
            new() { IsSuccess = false, StatusCode = statusCode };
    }
}
