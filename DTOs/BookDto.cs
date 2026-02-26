using System.ComponentModel.DataAnnotations;

namespace InsurTix.Api.DTOs
{
    public class BookDto
    {
        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 characters.")]
        public string Isbn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "Please provide a valid publication year.")]
        public int Year { get; set; }

        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10,000.")]
        public decimal Price { get; set; }
    }
}