namespace StartupCentral.Code.Model
{
    public class ApiResponse
    {
        public ApiResponse()
        {
        }

        public ApiResponse(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public ApiResponse(bool success, string message, object data)
        {
            this.Success = success;
            this.Message = message;
            this.Data = data;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}