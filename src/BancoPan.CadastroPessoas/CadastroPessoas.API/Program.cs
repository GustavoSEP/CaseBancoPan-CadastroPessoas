using Microsoft.EntityFrameworkCore;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Application.Services;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Infrastructure.SQL.Data;
using CadastroPessoas.Infrastructure.SQL.Repositories;
using CadastroPessoas.Infrastructure.SQL.Services;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using System.Reflection;
using System.IO;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Configurar o Swagger para incluir a documentação XML
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cadastro de Pessoas",
        Version = "v1",
        Description = "API de cadastro de pessoas físicas e jurídicas para desafio técnico do Banco Pan.",
        Contact = new OpenApiContact
        {
            Name = "Gustavo Moreira Santana",
            Email = "gustamoreira26@gmail.com",
            Url = new Uri("https://github.com/gustavoSEP"),
        }
    });

    // Configurar o Swagger para usar o arquivo XML de documentação
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddMemoryCache();

string? viaCepBase = builder.Configuration.GetValue<string>("ViaCep:BaseUrl");

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    if (!string.IsNullOrWhiteSpace(viaCepBase))
    {
        client.BaseAddress = new Uri(viaCepBase);
    }
    client.Timeout = TimeSpan.FromSeconds(6);
})

.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy())
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddScoped<IPessoaFisicaService, PessoaFisicaService>();
builder.Services.AddScoped<IPessoaJuridicaService, PessoaJuridicaService>();

builder.Services.AddScoped<IPessoaRepository, PessoaRepositorySQL>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Cadastro de Pessoas v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();