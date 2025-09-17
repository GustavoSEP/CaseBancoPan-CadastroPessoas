using CadastroPessoas.Infrastructure.SQL.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
namespace CadastroPessoas.Tests.Infrastructure.Services
{
    public class ViaCepServiceTests
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ICacheEntry> _mockCacheEntry;
        private readonly Mock<ILogger<ViaCepService>> _mockLogger;
        private object _cacheValue;
        private const string BaseUrl = "https://viacep.com.br/ws/";

        public ViaCepServiceTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockCacheEntry = new Mock<ICacheEntry>();
            _mockLogger = new Mock<ILogger<ViaCepService>>();

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
        public async Task Test_ConsultarenderecoPorCepAsync_Ok()
        {
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
            
            var service = new ViaCepService(httpClient, _mockCache.Object, _mockLogger.Object);

            _mockCache
                .Setup(m => m.TryGetValue(It.IsAny<string>(), out _cacheValue))
                .Returns(false);

            var resultado = await service.ConsultarEnderecoPorCepAsync(cep);

            Assert.NotNull(resultado);
            Assert.Equal("05723-330", resultado.Cep);
            Assert.Equal("Rua Afonso Vidal", resultado.Logradouro);
            Assert.Equal("Vila Andrade", resultado.Bairro);
            Assert.Equal("São Paulo", resultado.Cidade);
            Assert.Equal("SP", resultado.Estado);
        }

        [Fact]
        public async Task Test_ConsultarCepNulo_ExceptionError()
        {
            string cep = null;
            var mockHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri(BaseUrl) 
            };
            
            var service = new ViaCepService(httpClient, _mockCache.Object, _mockLogger.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.ConsultarEnderecoPorCepAsync(cep));
        }
    }
}