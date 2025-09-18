using CadastroPessoas.Adapters.Output.ViaCep.Services;
using CadastroPessoas.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CadastroPessoas.Tests.Adapters.Output.ViaCep.Services
{
    public class ViaCepAdapterTests
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ICacheEntry> _mockCacheEntry;
        private readonly Mock<ILogger<ViaCepAdapter>> _mockLogger;
        private object _cacheValue;
        private const string BaseUrl = "https://viacep.com.br/ws/";

        public ViaCepAdapterTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockCacheEntry = new Mock<ICacheEntry>();
            _mockLogger = new Mock<ILogger<ViaCepAdapter>>();

            _mockCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(_mockCacheEntry.Object);

            _mockCacheEntry
                .Setup(m => m.Value)
                .Returns(null);

            _mockCacheEntry
                .SetupSet(m => m.Value = It.IsAny<object>())
                .Callback<object>(v => _cacheValue = v);
        }

        [Fact]
        public async Task ConsultarEnderecoPorCepAsync_ComCepValido_RetornaEndereco()
        {
            // Arrange
            var cep = "05723330";
            var jsonResponse = @"{
                ""cep"": ""05723-330"",
                ""logradouro"": ""Rua Afonso Vidal"",
                ""bairro"": ""Vila Andrade"",
                ""localidade"": ""São Paulo"",
                ""uf"": ""SP"",
                ""ibge"": ""3550308"",
                ""gia"": ""1004"",
                ""ddd"": ""11"",
                ""siafi"": ""7107""
                }";

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
            
            var adapter = new ViaCepAdapter(httpClient, _mockCache.Object, _mockLogger.Object);

            _mockCache
                .Setup(m => m.TryGetValue(It.IsAny<string>(), out _cacheValue))
                .Returns(false);

            // Act
            var resultado = await adapter.ConsultarEnderecoPorCepAsync(cep);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("05723-330", resultado.Cep);
            Assert.Equal("Rua Afonso Vidal", resultado.Logradouro);
            Assert.Equal("Vila Andrade", resultado.Bairro);
            Assert.Equal("São Paulo", resultado.Cidade);
            Assert.Equal("SP", resultado.Estado);
            Assert.Equal(string.Empty, resultado.Numero);
            Assert.Equal(string.Empty, resultado.Complemento);
        }

        [Fact]
        public async Task ConsultarEnderecoPorCepAsync_QuandoCepEmCache_RetornaEnderecoDoCache()
        {
            // Arrange
            var cep = "01001000";
            var enderecoCache = new Endereco(
                "01001-000",
                "Praça da Sé",
                "Sé",
                "São Paulo",
                "SP",
                "",
                ""
            );

            // Fix for issue 1: Set up _cacheValue before the TryGetValue setup
            _cacheValue = enderecoCache;

            _mockCache
                .Setup(m => m.TryGetValue(It.IsAny<string>(), out _cacheValue))
                .Returns(true);

            var mockHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
            
            var adapter = new ViaCepAdapter(httpClient, _mockCache.Object, _mockLogger.Object);

            // Act
            var resultado = await adapter.ConsultarEnderecoPorCepAsync(cep);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("01001-000", resultado.Cep);
            Assert.Equal("Praça da Sé", resultado.Logradouro);
            
            // Verificar que a API não foi chamada
            mockHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task ConsultarEnderecoPorCepAsync_ComCepNulo_LancaArgumentException()
        {
            // Arrange
            string cep = null;
            var mockHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl) 
            };
            
            var adapter = new ViaCepAdapter(httpClient, _mockCache.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                adapter.ConsultarEnderecoPorCepAsync(cep));
        }

        [Fact]
        public async Task ConsultarEnderecoPorCepAsync_ComCepInvalido_LancaArgumentException()
        {
            // Arrange
            string cep = "123456"; // CEP com menos de 8 dígitos
            var mockHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl) 
            };
            
            var adapter = new ViaCepAdapter(httpClient, _mockCache.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                adapter.ConsultarEnderecoPorCepAsync(cep));
        }

        [Fact]
        public async Task ConsultarEnderecoPorCepAsync_QuandoCepNaoEncontrado_RetornaNulo()
        {
            // Arrange
            var cep = "00000000";
            // Fix for issue 2: Change the JSON to use a string for "erro" instead of a boolean
            var jsonResponse = @"{""erro"": ""true""}";

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
            
            var adapter = new ViaCepAdapter(httpClient, _mockCache.Object, _mockLogger.Object);

            _mockCache
                .Setup(m => m.TryGetValue(It.IsAny<string>(), out _cacheValue))
                .Returns(false);

            // Act
            var resultado = await adapter.ConsultarEnderecoPorCepAsync(cep);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ConsultarEnderecoPorCepAsync_QuandoApiRetornaErro_LancaHttpRequestException()
        {
            // Arrange
            var cep = "01001000";

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl)
            };
            
            var adapter = new ViaCepAdapter(httpClient, _mockCache.Object, _mockLogger.Object);

            _mockCache
                .Setup(m => m.TryGetValue(It.IsAny<string>(), out _cacheValue))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() =>
                adapter.ConsultarEnderecoPorCepAsync(cep));
        }
    }
}