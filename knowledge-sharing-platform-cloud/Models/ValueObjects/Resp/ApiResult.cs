using System.Numerics;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp
{
    /**
     * Api Response
     */
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public string ErrorMsg { get; set; } = string.Empty;
        public int ErrorCode { get; set; }
        public T Data { get; set; }

        public ApiResult() { }

        public ApiResult(bool success, T data = default, int errorCode = 0, string errorMsg = "")
        {
            Success = success;
            Data = data;
            ErrorCode = errorCode;
            ErrorMsg = errorMsg;
        }

        public static ApiResult<T> ServiceSucess()
            => new ApiResult<T> { Success = true };

        public static ApiResult<T> ServiceSucess(T data)
            => new ApiResult<T> { Success = true, Data = data };

        public static ApiResult<T> ServiceFail(int errorCode, string msg)
            => new ApiResult<T> { Success = false, ErrorCode = errorCode, ErrorMsg = msg };
    }
}
