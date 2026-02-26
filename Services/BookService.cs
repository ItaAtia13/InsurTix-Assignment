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
            html.AppendLine("    <title>InsurTIX Book Report</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 40px; }");
            html.AppendLine("        table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            html.AppendLine("        th, td { border: 1px solid #dddddd; text-align: left; padding: 8px; }");
            html.AppendLine("        th { background-color: #f2f2f2; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <h2>InsurTIX Book Inventory Report</h2>");
            html.AppendLine("    <table>");
            html.AppendLine("        <thead>");
            html.AppendLine("            <tr>");
            html.AppendLine("                <th>title</th>");
            html.AppendLine("                <th>author</th>");
            html.AppendLine("                <th>category</th>");
            html.AppendLine("                <th>Year</th>");
            html.AppendLine("                <th>price</th>");
            html.AppendLine("            </tr>");
            html.AppendLine("        </thead>");
            html.AppendLine("        <tbody>");

            foreach (var book in books)
            {
                html.AppendLine("            <tr>");
                html.AppendLine($"                <td>{book.Title}</td>");
                html.AppendLine($"                <td>{book.Author}</td>");
                html.AppendLine($"                <td>{book.Category}</td>");
                html.AppendLine($"                <td>{book.Year}</td>");
                html.AppendLine($"                <td>{book.Price}</td>");
                html.AppendLine("            </tr>");
            }

            html.AppendLine("        </tbody>");
            html.AppendLine("    </table>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
}