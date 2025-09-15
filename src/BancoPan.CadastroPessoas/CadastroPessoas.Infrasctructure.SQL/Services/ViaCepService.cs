using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CadastroPessoas.Infrastructure.SQL.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        public ViaCepService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<Endereco?> ConsultaEnderecoPorCepAsync(string cep)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cep))
                    throw new ArgumentException("CEP não pode ser nulo ou vazio.", nameof(cep));

                var cepNormalized = NormalizeCep(cep);
                if (_cache.TryGetValue<Endereco>(cepNormalized, out var cached))
                {
                    return cached;
                }

                using var response = await _httpClient.GetAsync($"{cepNormalized}/json/");
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Erro ao consultar o CEP: {response.ReasonPhrase}");

                var content = await response.Content.ReadAsStringAsync();
                var viaCepResponse = JsonSerializer.Deserialize<ViaCepResponse>(content, _jsonOptions);

                if (viaCepResponse == null || viaCepResponse.Erro)
                    return null;

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
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                throw new Exception($"ViaCepService: erro inesperado ao consultar ViaCEP");
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
