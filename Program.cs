using BusinessSearchTool;
using dotenv.net;
using OfficeOpenXml;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
DotEnv.Load();
builder.Services.AddSingleton(new OpenAIService(Environment.GetEnvironmentVariable("OPENAI_API_KEY")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/", () => "Hello World");

ExcelPackage.License.SetNonCommercialPersonal("Leonardo Ricotti");

app.Run();
