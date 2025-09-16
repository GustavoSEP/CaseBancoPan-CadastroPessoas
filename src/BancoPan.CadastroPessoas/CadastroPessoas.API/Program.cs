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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Memory cache (para cache de CEPs)
builder.Services.AddMemoryCache();

string? viaCepBase = builder.Configuration.GetValue<string>("ViaCep:BaseUrl");

// Retry + circuit-breaker policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    // Retry 3 vezes com backoff exponencial
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    // Após 5 falhas consecutivas, abrir por 30s
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

// Registrar HttpClient para IViaCepService com políticas
builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    if (!string.IsNullOrWhiteSpace(viaCepBase))
    {
        client.BaseAddress = new Uri(viaCepBase);
    }
    // Timeout razoável para ViaCEP
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();