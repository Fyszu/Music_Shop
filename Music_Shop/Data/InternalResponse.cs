namespace Music_Shop.Data
{
    public class InternalResponse
    {
        public enum StatusCode
        {
            Success,
            DeserializeProblem,
            Unauthorized,
            InvalidRequestValue,
            OtherError
        }
        public string? Message { get; set; }
        public StatusCode Status { get; set; }
    }
}
