using System.Text;
using InsurTix.Api.DTOs;
using InsurTix.Api.Models;
using InsurTix.Api.Repositories;

namespace InsurTix.Api.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository)
        {
            _repository = repository;
        }

        private BookDto MapToDto(Book book)
        {
            return new BookDto
            {
                Isbn = book.Isbn,
                Title = book.Title,
                Category = book.Category,
                Year = book.Year,
                Price = book.Price,

                Author = book.Authors != null && book.Authors.Any() 
                         ? string.Join(", ", book.Authors) 
                         : string.Empty
            };
        }

        private Book MapToModel(BookDto dto)
        {
            return new Book
            {
                Isbn = dto.Isbn,
                Title = dto.Title,
                Category = dto.Category,
                Year = dto.Year,
                Price = dto.Price,

                Authors = string.IsNullOrWhiteSpace(dto.Author) 
                          ? new List<string>() 
                          : dto.Author.Split(',').Select(a => a.Trim()).ToList()
            };
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _repository.GetAllBooksAsync();
            return books.Select(MapToDto);
        }

        public async Task<BookDto?> GetBookByIsbnAsync(string isbn)
        {
            var book = await _repository.GetBookByIsbnAsync(isbn);
            return book != null ? MapToDto(book) : null;
        }

        public async Task AddBookAsync(BookDto bookDto)
        {
            var book = MapToModel(bookDto);
            await _repository.AddBookAsync(book);
        }

        public async Task UpdateBookAsync(BookDto bookDto)
        {
            var book = MapToModel(bookDto);
            await _repository.UpdateBookAsync(book);
        }

        public async Task DeleteBookAsync(string isbn)
        {
            await _repository.DeleteBookAsync(isbn);
        }

public async Task<string> GenerateHtmlReportAsync()
{
    var books = await GetAllBooksAsync();

    var html = new StringBuilder();
    html.AppendLine("<!DOCTYPE html>");
    html.AppendLine("<html lang=\"en\">");
    html.AppendLine("<head>");
    html.AppendLine("    <meta charset=\"UTF-8\">");
    html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
    html.AppendLine("    <title>InsurTIX Book Report</title>");
    html.AppendLine("    <style>");
    html.AppendLine("        :root { --primary-color: #2563eb; --bg-color: #f3f4f6; --text-main: #1f2937; --text-muted: #6b7280; --border-color: #e5e7eb; }");
    html.AppendLine("        body { font-family: 'Segoe UI', system-ui, -apple-system, sans-serif; background-color: var(--bg-color); color: var(--text-main); margin: 0; padding: 40px 20px; display: flex; justify-content: center; }");
    html.AppendLine("        .report-container { background: #ffffff; border-radius: 10px; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06); width: 100%; max-width: 1000px; padding: 30px; overflow-x: auto; }");
    html.AppendLine("        .header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid var(--border-color); padding-bottom: 20px; margin-bottom: 20px; }");
    html.AppendLine("        h2 { margin: 0; color: var(--primary-color); font-size: 24px; font-weight: 600; }");
    html.AppendLine("        .timestamp { color: var(--text-muted); font-size: 14px; }");
    html.AppendLine("        table { width: 100%; border-collapse: collapse; text-align: left; }");
    html.AppendLine("        th { background-color: #f9fafb; color: var(--text-muted); font-weight: 600; text-transform: uppercase; font-size: 12px; letter-spacing: 0.05em; padding: 12px 16px; border-bottom: 1px solid var(--border-color); }");
    html.AppendLine("        td { padding: 16px; border-bottom: 1px solid var(--border-color); font-size: 14px; vertical-align: middle; }");
    html.AppendLine("        tbody tr:hover { background-color: #f9fafb; transition: background-color 0.2s ease; }");
    html.AppendLine("        .category-badge { background-color: #e0e7ff; color: #4338ca; padding: 4px 10px; border-radius: 9999px; font-size: 12px; font-weight: 500; display: inline-block; }");
    html.AppendLine("        .price { font-weight: 600; color: #059669; }");
    html.AppendLine("    </style>");
    html.AppendLine("</head>");
    html.AppendLine("<body>");
    html.AppendLine("    <div class=\"report-container\">");
    html.AppendLine("        <div class=\"header\">");
    html.AppendLine("            <h2>📚 InsurTIX Inventory Report</h2>");
    html.AppendLine($"            <span class=\"timestamp\">Generated: {DateTime.Now:MMM dd, yyyy HH:mm}</span>");
    html.AppendLine("        </div>");
    html.AppendLine("        <table>");
    html.AppendLine("            <thead>");
    html.AppendLine("                <tr>");
    html.AppendLine("                    <th>Title</th>");
    html.AppendLine("                    <th>Author</th>");
    html.AppendLine("                    <th>Category</th>");
    html.AppendLine("                    <th>Year</th>");
    html.AppendLine("                    <th>Price</th>");
    html.AppendLine("                </tr>");
    html.AppendLine("            </thead>");
    html.AppendLine("            <tbody>");

    foreach (var book in books)
    {
        html.AppendLine("                <tr>");
        html.AppendLine($"                    <td style=\"font-weight: 500;\">{book.Title}</td>");
        html.AppendLine($"                    <td style=\"color: var(--text-muted);\">{book.Author}</td>");
        html.AppendLine($"                    <td><span class=\"category-badge\">{book.Category}</span></td>");
        html.AppendLine($"                    <td>{book.Year}</td>");
        html.AppendLine($"                    <td class=\"price\">${book.Price}</td>");
        html.AppendLine("                </tr>");
    }

    html.AppendLine("            </tbody>");
    html.AppendLine("        </table>");
    html.AppendLine("    </div>");
    html.AppendLine("</body>");
    html.AppendLine("</html>");

    return html.ToString();
}
    }
}