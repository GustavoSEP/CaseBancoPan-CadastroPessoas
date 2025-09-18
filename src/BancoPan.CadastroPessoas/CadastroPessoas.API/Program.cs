using CadastroPessoas.Adapters.Output.SQL.Repositories;
using CadastroPessoas.Adapters.Output.ViaCep.Services;
using CadastroPessoas.Application.Services;
using CadastroPessoas.Application.UseCases;
using CadastroPessoas.Adapters.Output.SQL.Data;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using CadastroPessoas.Ports.Output.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Cadastro de Pessoas",
        Version = "v1",
        Description = "API para cadastro de pessoas físicas e jurídicas",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Gustavo",
            Email = "gustamoreira26@gmail.com",
            Url = new Uri("https://github.com/gustavoSEP"),
        }
    });

    // Configurar o Swagger para usar o arquivo XML de documentação
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});

// Configurar o banco de dados
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Adicionar cache em memória
builder.Services.AddMemoryCache();

// Políticas de Retry e Circuit Breaker para HttpClient
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

// Primeiro registre o ViaCepAdapter como implementação de IEnderecoPorCepProvider
builder.Services.AddHttpClient<ViaCepAdapter>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddScoped<IEnderecoPorCepProvider, ViaCepAdapter>();

// Depois registre o adaptador para o serviço legado
builder.Services.AddScoped<IViaCepService, ViaCepServiceAdapter>();

// Registrar Use Cases
builder.Services.AddScoped<ICreatePessoaFisicaUseCase, CreatePessoaFisicaUseCase>();
builder.Services.AddScoped<IGetPessoaFisicaUseCase, GetPessoaFisicaUseCase>();
builder.Services.AddScoped<IUpdatePessoaFisicaUseCase, UpdatePessoaFisicaUseCase>();
builder.Services.AddScoped<IDeletePessoaFisicaUseCase, DeletePessoaFisicaUseCase>();

builder.Services.AddScoped<ICreatePessoaJuridicaUseCase, CreatePessoaJuridicaUseCase>();
builder.Services.AddScoped<IGetPessoaJuridicaUseCase, GetPessoaJuridicaUseCase>();
builder.Services.AddScoped<IUpdatePessoaJuridicaUseCase, UpdatePessoaJuridicaUseCase>();
builder.Services.AddScoped<IDeletePessoaJuridicaUseCase, DeletePessoaJuridicaUseCase>();

// Registrar Repositories
builder.Services.AddScoped<IPessoaRepository, PessoaRepositorySqlAdapter>();

// Registrar serviços da aplicação legados (para compatibilidade durante a transição)
builder.Services.AddScoped<IPessoaFisicaService, PessoaFisicaService>();
builder.Services.AddScoped<IPessoaJuridicaService, PessoaJuridicaService>();

var app = builder.Build();

// Configure o pipeline de HTTP request.
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

// Mapear controllers
app.MapControllers();

app.Run();