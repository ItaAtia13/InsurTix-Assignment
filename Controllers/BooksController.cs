using InsurTix.Api.DTOs;
using InsurTix.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsurTix.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // GET: api/books/{isbn}
        [HttpGet("{isbn}")]
        public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
        {
            var book = await _bookService.GetBookByIsbnAsync(isbn);
            if (book == null)
            {
                return NotFound($"Book with ISBN {isbn} was not found.");
            }
            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult> AddBook([FromBody] BookDto bookDto)
        {
            try
            {
                var existingBook = await _bookService.GetBookByIsbnAsync(bookDto.Isbn);
                if (existingBook != null)
                {
                    return Conflict($"A book with ISBN {bookDto.Isbn} already exists.");
                }

                await _bookService.AddBookAsync(bookDto);

                return CreatedAtAction(nameof(GetBookByIsbn), new { isbn = bookDto.Isbn }, bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/books/{isbn}
        [HttpPut("{isbn}")]
        public async Task<ActionResult> UpdateBook(string isbn, [FromBody] BookDto bookDto)
        {
            if (isbn != bookDto.Isbn)
            {
                return BadRequest("ISBN in the URL does not match the ISBN in the body.");
            }

            var existingBook = await _bookService.GetBookByIsbnAsync(isbn);
            if (existingBook == null)
            {
                return NotFound($"Book with ISBN {isbn} was not found.");
            }

            await _bookService.UpdateBookAsync(bookDto);
            return NoContent();
        }

        // DELETE: api/books/{isbn}
        [HttpDelete("{isbn}")]
        public async Task<ActionResult> DeleteBook(string isbn)
        {
            var existingBook = await _bookService.GetBookByIsbnAsync(isbn);
            if (existingBook == null)
            {
                return NotFound($"Book with ISBN {isbn} was not found.");
            }

            await _bookService.DeleteBookAsync(isbn);
            return NoContent();
        }

        // GET: api/books/report
        [HttpGet("report")]
        public async Task<IActionResult> GetHtmlReport()
        {
            var htmlContent = await _bookService.GenerateHtmlReportAsync();


            return Content(htmlContent, "text/html", System.Text.Encoding.UTF8);
        }
    }
}