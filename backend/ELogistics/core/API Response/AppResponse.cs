using Newtonsoft.Json;

namespace core.API_Response
{
    public sealed class AppResponse
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int? StatusCode { get; set; }

        public static AppResponse Response(bool isSuccess, string message, HttpStatusCodes statusCode = HttpStatusCodes.OK)
        {
            return new AppResponse
            {
                IsSuccess = isSuccess,
                Message = message,
                StatusCode = Convert.ToInt32(statusCode)
            };
        }

        public static AppResponse<TModel> Fail<TModel>(TModel data = default, string message = "", HttpStatusCodes statusCode = HttpStatusCodes.InternalServerError)
        {
            return new AppResponse<TModel>
            {
                Data = data,
                IsSuccess = false,
                Message = message,
                StatusCode = Convert.ToInt32(statusCode)
            };
        }

        public static AppResponse<TModel> Success<TModel>(TModel data = default, string message = "", HttpStatusCodes statusCode = HttpStatusCodes.OK)
        {
            return new AppResponse<TModel>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                StatusCode = Convert.ToInt32(statusCode)
            };
        }
    }

    public sealed class AppResponse<TModel>
    {
        public TModel? Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int? StatusCode { get; set; }
    }
}
