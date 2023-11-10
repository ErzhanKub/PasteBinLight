using System.ComponentModel.DataAnnotations;

namespace WebApi.Dtos
{
    public class ErrorDto
    {
        // HTTP status code
        public int StatusCode { get; set; }

        // Error message
        [Required]
        public string? Message { get; set; }
    }
}
