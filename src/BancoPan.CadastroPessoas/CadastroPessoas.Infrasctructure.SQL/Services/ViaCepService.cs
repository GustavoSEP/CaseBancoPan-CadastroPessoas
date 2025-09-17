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
    /// <summary>
    /// Implementação do serviço de consulta de CEP através da API ViaCEP.
    /// Fornece funcionalidade para buscar endereços completos a partir de um CEP,
    /// com suporte a cache para melhorar a performance e reduzir chamadas à API externa.
    /// </summary>
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ViaCepService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ViaCepService"/>.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP utilizado para fazer requisições à API ViaCEP.</param>
        /// <param name="cache">Provedor de cache em memória para armazenar resultados de consultas.</param>
        /// <param name="logger">Serviço de log para registrar informações e erros.</param>
        /// <exception cref="ArgumentNullException">Lançada quando alguma das dependências é nula.</exception>
        /// <remarks>
        /// O HttpClient deve estar configurado com a URL base da API ViaCEP: https://viacep.com.br/ws/
        /// </remarks>
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

        /// <summary>
        /// Consulta um endereço completo a partir de um CEP.
        /// </summary>
        /// <param name="cep">O CEP a ser consultado (formato: 00000-000 ou 00000000).</param>
        /// <returns>
        /// Um objeto <see cref="Endereco"/> contendo os dados do endereço correspondente ao CEP informado,
        /// ou null caso o CEP seja inválido ou não seja encontrado.
        /// </returns>
        /// <exception cref="ArgumentException">Lançada quando o CEP fornecido é nulo ou vazio.</exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro inesperado durante a consulta.</exception>
        /// <remarks>
        /// Os resultados são armazenados em cache por 6 horas para melhorar a performance.
        /// Erros de timeout ou conexão com a API externa não causam exceções, apenas retornam null.
        /// </remarks>
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

        /// <summary>
        /// Normaliza um CEP removendo caracteres especiais e espaços.
        /// </summary>
        /// <param name="cep">O CEP a ser normalizado.</param>
        /// <returns>CEP contendo apenas dígitos (formato: 00000000).</returns>
        private static string NormalizeCep(string cep)
        {
            return cep.Replace("-", "").Trim();
        }

        /// <summary>
        /// Classe interna para deserialização da resposta da API ViaCEP.
        /// </summary>
        private class ViaCepResponse
        {
            /// <summary>
            /// CEP no formato 00000-000.
            /// </summary>
            public string? Cep { get; set; }

            /// <summary>
            /// Nome da rua, avenida, etc.
            /// </summary>
            public string? Logradouro { get; set; }

            /// <summary>
            /// Nome do bairro.
            /// </summary>
            public string? Bairro { get; set; }

            /// <summary>
            /// Nome da cidade.
            /// </summary>
            public string? Localidade { get; set; }

            /// <summary>
            /// Sigla do estado (UF).
            /// </summary>
            public string? Uf { get; set; }

            /// <summary>
            /// Indica se houve erro na consulta do CEP.
            /// </summary>
            public bool Erro { get; set; } = false;
        }
    }
}