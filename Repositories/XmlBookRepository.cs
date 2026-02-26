using System.Xml.Linq;
using InsurTix.Api.Models;
using System.Globalization;

namespace InsurTix.Api.Repositories
{
    public class XmlBookRepository : IBookRepository
    {
        private readonly string _filePath;
        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);

        public XmlBookRepository(IConfiguration configuration)
        {
            _filePath = configuration["XmlFilePath"] ?? "Data/bookstore.xml";
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                var doc = new XDocument(new XElement("bookstore"));
                doc.Save(_filePath);
            }
        }

        private Book MapToBook(XElement element)
        {
            return new Book
            {
                Isbn = element.Element("isbn")?.Value ?? string.Empty,
                Title = element.Element("title")?.Value ?? string.Empty,
                Year = int.TryParse(element.Element("year")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int year) ? year : 0,
                Price = decimal.TryParse(element.Element("price")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) ? price : 0,
                Category = element.Attribute("category")?.Value ?? string.Empty,
                Authors = element.Elements("author").Select(a => a.Value).ToList()
            };
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                using var stream = File.OpenRead(_filePath);
                var doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

                return doc.Descendants("book").Select(MapToBook).ToList();
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task<Book?> GetBookByIsbnAsync(string isbn)
        {
            await _fileLock.WaitAsync();
            try
            {
                using var stream = File.OpenRead(_filePath);
                var doc = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

                var element = doc.Descendants("book")
                                 .FirstOrDefault(x => x.Element("isbn")?.Value == isbn);

                return element != null ? MapToBook(element) : null;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task AddBookAsync(Book book)
        {
            await _fileLock.WaitAsync();
            try
            {
                var doc = XDocument.Load(_filePath);

                if (doc.Descendants("book").Any(x => x.Element("isbn")?.Value == book.Isbn))
                {
                    throw new InvalidOperationException($"Book with ISBN {book.Isbn} already exists.");
                }

                var newBookElement = new XElement("book",
                    new XAttribute("category", book.Category),
                    new XElement("isbn", book.Isbn),
                    new XElement("title", book.Title, new XAttribute("lang", "en")),
                    new XElement("year", book.Year),
                    new XElement("price", book.Price)
                );

                foreach (var author in book.Authors)
                {
                    newBookElement.Add(new XElement("author", author));
                }

                doc.Root?.Add(newBookElement);
                doc.Save(_filePath);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _fileLock.WaitAsync();
            try
            {
                var doc = XDocument.Load(_filePath);
                var element = doc.Descendants("book")
                                 .FirstOrDefault(x => x.Element("isbn")?.Value == book.Isbn);

                if (element != null)
                {
                    element.SetAttributeValue("category", book.Category);
                    element.SetElementValue("title", book.Title);
                    element.SetElementValue("year", book.Year);
                    element.SetElementValue("price", book.Price);

                    element.Elements("author").Remove();
                    var yearElement = element.Element("year");

                    foreach (var author in book.Authors)
                    {
                        if (yearElement != null)
                        {
                            yearElement.AddBeforeSelf(new XElement("author", author));
                        }
                        else
                        {
                            element.Add(new XElement("author", author));
                        }
                    }

                    doc.Save(_filePath);
                }
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public async Task DeleteBookAsync(string isbn)
        {
            await _fileLock.WaitAsync();
            try
            {
                var doc = XDocument.Load(_filePath);
                var element = doc.Descendants("book")
                                 .FirstOrDefault(x => x.Element("isbn")?.Value == isbn);

                if (element != null)
                {
                    element.Remove();
                    doc.Save(_filePath);
                }
            }
            finally
            {
                _fileLock.Release();
            }
        }
    }
}