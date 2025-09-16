using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Infrastructure.SQL.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ViaCepService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        public ViaCepService(HttpClient httpClient, IMemoryCache cache, ILogger<ViaCepService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                throw new ArgumentException("CEP é obrigatório.");

            var cepNormalized = NormalizeCep(cep);

            if (_cache.TryGetValue<Endereco>(cepNormalized, out var cached))
            {
                _logger.LogDebug("ViaCepService: retornando CEP {Cep} do cache", cepNormalized);
                return cached;
            }

            try
            {
                _logger.LogDebug("ViaCepService: consultando ViaCEP para {Cep}", cepNormalized);
                using var response = await _httpClient.GetAsync($"{cepNormalized}/json/");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("ViaCepService: ViaCEP retornou status {Status} para {Cep}", (int)response.StatusCode, cepNormalized);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponse>(content, _jsonOptions);

                if (viaCepResponse == null)
                {
                    _logger.LogWarning("ViaCepService: resposta nula do ViaCEP para {Cep}", cepNormalized);
                    return null;
                }

                if (viaCepResponse.Erro)
                {
                    _logger.LogInformation("ViaCepService: ViaCEP informou erro para {Cep}", cepNormalized);
                    return null;
                }

                var endereco = new Endereco(
                    viaCepResponse.Cep ?? string.Empty,
                    viaCepResponse.Logradouro ?? string.Empty,
                    viaCepResponse.Bairro ?? string.Empty,
                    viaCepResponse.Localidade ?? string.Empty,
                    viaCepResponse.Uf ?? string.Empty
                );

                _cache.Set(cepNormalized, endereco, _cacheDuration);

                return endereco;
            }
            catch (TaskCanceledException tex)
            {
                _logger.LogWarning(tex, "ViaCepService: timeout ao consultar ViaCEP para {Cep}", cepNormalized);
                return null;
            }
            catch (HttpRequestException hrex)
            {
                _logger.LogWarning(hrex, "ViaCepService: falha de requisição ao ViaCEP para {Cep}", cepNormalized);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ViaCepService: erro inesperado ao consultar ViaCEP para {Cep}", cepNormalized);
                throw;
            }
        }

        private static string NormalizeCep(string cep)
        {
            return cep.Replace("-", "").Trim();
        }

        private class ViaCepResponse
        {
            public string? Cep { get; set; }
            public string? Logradouro { get; set; }
            public string? Bairro { get; set; }
            public string? Localidade { get; set; }
            public string? Uf { get; set; }
            public bool Erro { get; set; } = false;
        }
    }
}