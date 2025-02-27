

using knowledge_sharing_platform_cloud.Exception;

namespace knowledge_sharing_platform_cloud.Exception
{
    public class BusinessException : System.Exception
    {
        public int ErrorCode { get; }
        public string ErrorMsg { get; }


        public BusinessException(string errorMsg)
           : base(errorMsg)
        {
            this.ErrorCode = (int)CommonErrorEnum.BUSINESS_ERROR;
            this.ErrorMsg = errorMsg;
        }

        public BusinessException(int errorCode, string errorMsg)
            : base(errorMsg)
        {
            this.ErrorCode = errorCode;
            this.ErrorMsg = errorMsg;
        }



    }
}
