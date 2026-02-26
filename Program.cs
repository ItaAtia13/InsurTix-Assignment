using InsurTix.Api.Repositories;
using InsurTix.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- כאן אנחנו רושמים את התלויות (Dependencies) שלנו ---
// אנו משתמשים ב-AddScoped, מה שאומר שייווצר מופע אחד של ה-Repository לכל בקשת HTTP (Request).
// זוהי תצורת ברירת המחדל המומלצת ביותר לעבודה עם Repositories וגישה לנתונים ב-Web API.
builder.Services.AddScoped<IBookRepository, XmlBookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
// ---------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();