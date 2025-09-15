using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AppDbContext
builder.Services.AddDbContext<CadastroPessoas.Infrastructure.SQL.Data.AppDbContext>(options =>
    options.UseInMemoryDatabase("CadastroPessoa")); 

// Register MemoryCache
builder.Services.AddMemoryCache();

// Register your services
builder.Services.AddScoped<CadastroPessoas.Domain.Interfaces.IPessoaRepository, CadastroPessoas.Infrastructure.SQL.Repositories.PessoaRepositorySQL>();
builder.Services.AddScoped<CadastroPessoas.Domain.Interfaces.IViaCepService, CadastroPessoas.Infrastructure.SQL.Services.ViaCepService>();
builder.Services.AddScoped<CadastroPessoas.Application.Interfaces.IPessoaFisicaService, CadastroPessoas.Application.Services.PessoaFisicaService>();

// Register HttpClient for ViaCepService
builder.Services.AddHttpClient();

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