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
    /// <summary>
    /// Adaptador para integração com a API ViaCEP que implementa a porta <see cref="IEnderecoPorCepProvider"/>.
    /// </summary>
    /// <remarks>
    /// Este adaptador realiza consultas à API pública ViaCEP (https://viacep.com.br) e converte as respostas
    /// para o formato de domínio da aplicação. Implementa cache em memória para otimizar o desempenho e
    /// reduzir chamadas desnecessárias à API externa.
    /// </remarks>
    public class ViaCepAdapter : IEnderecoPorCepProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ViaCepAdapter> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(6);

        /// <summary>
        /// Inicializa uma nova instância do adaptador ViaCEP.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP para comunicação com a API externa.</param>
        /// <param name="cache">Serviço de cache em memória para armazenar resultados.</param>
        /// <param name="logger">Serviço de log para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        /// <remarks>
        /// Se o cliente HTTP não tiver um endereço base configurado, este será definido como 
        /// "https://viacep.com.br/ws/".
        /// </remarks>
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

        /// <summary>
        /// Consulta um endereço pelo CEP informado.
        /// </summary>
        /// <param name="cep">CEP a ser consultado (formato com ou sem hífen).</param>
        /// <returns>
        /// Objeto <see cref="Endereco"/> contendo os dados do endereço, ou null se o CEP não for encontrado.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Lançada quando o CEP é nulo, vazio ou não contém 8 dígitos após normalização.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// Lançada quando ocorre um erro na comunicação com a API ViaCEP.
        /// </exception>
        /// <exception cref="Exception">
        /// Lançada quando ocorre um erro inesperado durante o processamento.
        /// </exception>
        /// <remarks>
        /// Este método primeiro verifica o cache antes de fazer uma chamada à API.
        /// Os resultados são armazenados em cache por 6 horas para reduzir o número de chamadas à API.
        /// </remarks>
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

        /// <summary>
        /// Classe interna para deserialização da resposta da API ViaCEP.
        /// </summary>
        /// <remarks>
        /// Esta classe mapeia todos os campos retornados pela API ViaCEP para facilitar
        /// a deserialização do JSON de resposta.
        /// </remarks>
        private class ViaCepResponse
        {
            /// <summary>CEP formatado</summary>
            public string? Cep { get; set; }
            
            /// <summary>Nome da rua, avenida, etc.</summary>
            public string? Logradouro { get; set; }
            
            /// <summary>Complemento do endereço</summary>
            public string? Complemento { get; set; }
            
            /// <summary>Bairro</summary>
            public string? Bairro { get; set; }
            
            /// <summary>Cidade</summary>
            public string? Localidade { get; set; }
            
            /// <summary>Estado (UF)</summary>
            public string? Uf { get; set; }
            
            /// <summary>Código IBGE do município</summary>
            public string? Ibge { get; set; }
            
            /// <summary>Código GIA (usado em São Paulo)</summary>
            public string? Gia { get; set; }
            
            /// <summary>Código DDD</summary>
            public string? Ddd { get; set; }
            
            /// <summary>Código SIAFI</summary>
            public string? Siafi { get; set; }
            
            /// <summary>Erro retornado pela API (presente apenas quando o CEP não é encontrado)</summary>
            public string? Erro { get; set; }
        }
    }
}