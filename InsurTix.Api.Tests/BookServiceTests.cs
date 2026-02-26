using InsurTix.Api.Models;
using InsurTix.Api.Repositories;
using InsurTix.Api.Services;
using Moq;
using Xunit;

namespace InsurTix.Api.Tests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockRepo;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockRepo = new Mock<IBookRepository>();
            _bookService = new BookService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllBooksAsync_WithMultipleAuthors_FormatsAuthorStringCorrectly()
        {
            // Arrange
            var fakeBooks = new List<Book>
            {
                new Book 
                { 
                    Isbn = "123", 
                    Title = "Unit Testing Masterclass", 
                    Authors = new List<string> { "John Doe", "Jane Smith" } 
                }
            };
            
            _mockRepo.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(fakeBooks);

            // Act
            var result = await _bookService.GetAllBooksAsync();

            // Assert
            var bookDto = Assert.Single(result);
            Assert.Equal("John Doe, Jane Smith", bookDto.Author); 
        }

        [Fact]
        public async Task GetBookByIsbnAsync_WhenBookDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetBookByIsbnAsync(It.IsAny<string>()))
                     .ReturnsAsync((Book?)null);

            // Act
            var result = await _bookService.GetBookByIsbnAsync("99999");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GenerateHtmlReportAsync_ReturnsValidHtmlStructure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllBooksAsync()).ReturnsAsync(new List<Book>());

            // Act
            var htmlResult = await _bookService.GenerateHtmlReportAsync();

            // Assert
            Assert.Contains("<!DOCTYPE html>", htmlResult);
            Assert.Contains("<th>title</th>", htmlResult);
            Assert.Contains("<th>author</th>", htmlResult);
            Assert.Contains("<th>price</th>", htmlResult);
        }
    }
}