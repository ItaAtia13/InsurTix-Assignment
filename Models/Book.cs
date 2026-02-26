namespace InsurTix.Api.Models
{
    public class Book
    {
        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        
        // כיוון שיכולים להיות כמה מחברים, נשמור אותם כרשימה
        public List<string> Authors { get; set; } = new List<string>();
        
        public int Year { get; set; }
        public decimal Price { get; set; }
    }
}