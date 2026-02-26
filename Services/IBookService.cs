using InsurTix.Api.DTOs;

namespace InsurTix.Api.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIsbnAsync(string isbn);
        Task AddBookAsync(BookDto bookDto);
        Task UpdateBookAsync(BookDto bookDto);
        Task DeleteBookAsync(string isbn);
        
        // הפונקציה המיוחדת להפקת דוח ה-HTML
        Task<string> GenerateHtmlReportAsync();
    }
}