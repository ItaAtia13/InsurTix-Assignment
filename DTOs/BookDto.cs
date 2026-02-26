namespace InsurTix.Api.DTOs
{
    public class BookDto
    {
        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        
        // כאן אנחנו מחזיקים את המחברים כמחרוזת אחת מופרדת בפסיקים
        public string Author { get; set; } = string.Empty; 
        
        public int Year { get; set; }
        public decimal Price { get; set; }
    }
}