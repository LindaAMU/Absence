namespace Abence.WEB.Models
{
    public class StandardResponse
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
