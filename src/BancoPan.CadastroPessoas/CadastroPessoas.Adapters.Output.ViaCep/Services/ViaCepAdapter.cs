using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Ports.Output.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Adapters.Output.ViaCep.Services
{
    public class ViaCepAdapter : IEnderecoPorCepProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ViaCepAdapter> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        public ViaCepAdapter(HttpClient httpClient, IMemoryCache cache, ILogger<ViaCepAdapter> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configurar o httpClient se não foi feito na injeção de dependência
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://viacep.com.br/ws/");
            }
        }

        public async Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                throw new ArgumentException("CEP não pode ser nulo ou vazio", nameof(cep));

            // Remover caracteres não numéricos
            string cepNormalizado = new string(cep.Where(char.IsDigit).ToArray());
            if (cepNormalizado.Length != 8)
                throw new ArgumentException("CEP deve conter 8 dígitos", nameof(cep));

            // Verificar cache
            string cacheKey = $"ViaCep_{cepNormalizado}";
            if (_cache.TryGetValue(cacheKey, out Endereco? enderecoCached))
            {
                _logger.LogInformation("Endereço para CEP {Cep} retornado do cache", cepNormalizado);
                return enderecoCached;
            }

            try
            {
                // Fazer a requisição para a API
                string url = $"{cepNormalizado}/json/";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ViaCepResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null || !string.IsNullOrWhiteSpace(result.Erro))
                {
                    _logger.LogWarning("CEP {Cep} não encontrado na API ViaCEP", cepNormalizado);
                    return null;
                }

                var endereco = new Endereco(
                    result.Cep,
                    result.Logradouro,
                    result.Bairro,
                    result.Localidade, // Cidade
                    result.Uf,         // Estado
                    string.Empty,      // Número não vem na resposta da API
                    string.Empty       // Complemento não preenchemos aqui
                );

                // Armazenar no cache
                _cache.Set(cacheKey, endereco, _cacheDuration);
                _logger.LogInformation("Endereço para CEP {Cep} obtido da API e armazenado em cache", cepNormalizado);

                return endereco;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao consultar o CEP {Cep} na API ViaCEP", cepNormalizado);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao consultar o CEP {Cep}", cepNormalizado);
                throw;
            }
        }

        // Classe interna para deserialização da resposta da API
        private class ViaCepResponse
        {
            public string? Cep { get; set; }
            public string? Logradouro { get; set; }
            public string? Complemento { get; set; }
            public string? Bairro { get; set; }
            public string? Localidade { get; set; }
            public string? Uf { get; set; }
            public string? Ibge { get; set; }
            public string? Gia { get; set; }
            public string? Ddd { get; set; }
            public string? Siafi { get; set; }
            public string? Erro { get; set; }
        }
    }
}