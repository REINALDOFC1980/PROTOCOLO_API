namespace Triagem.Model
{
    public class ErrorResponseModel
    {

        public DateTimeOffset Timestamp { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
    }
}
