using CadastroPessoas.Adapters.Input.Api.Controllers;
using CadastroPessoas.Adapters.Input.Api.Models;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CadastroPessoas.Tests.API.Controller
{
    public class PessoaFisicaControllerTest
    {
        private readonly Mock<IPessoaFisicaService> _mockService;
        private readonly Mock<ILogger<PessoaFisicaController>> _mockLogger;
        private readonly PessoaFisicaController _controller;

        public PessoaFisicaControllerTest()
        {
            _mockService = new Mock<IPessoaFisicaService>();
            _mockLogger = new Mock<ILogger<PessoaFisicaController>>();
            _controller = new PessoaFisicaController(_mockService.Object, _mockLogger.Object);

        }

        [Fact]
        public async Task Test_Criar_PessoaFisica_ok()
        {
            var request = new PessoaFisicaRequest
            {
                Nome = "Gustavo M. Santana",
                CPF = "49633697883",
                CEP = "04850-280",
                Numero = "35",
                Complemento = "casa verde"
            };

            var endereco = new Endereco("04850280", "Pq. Cocaia", "Grajau", "São Paulo", "SP", "35", "casa verde");
            var pessoaFisica = new PessoaFisica("Gustavo M Santana", "49633697883", "F", endereco)
            {
                Id = 1
            };

            _mockService.Setup(s => s.CreateAsync(
                request.Nome, request.CPF, request.CEP, request.Numero, request.Complemento))
                .ReturnsAsync(pessoaFisica);

            var result = await _controller.Create(request);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetByCpf", createdAtActionResult.ActionName);
            Assert.Equal(pessoaFisica.CPF, createdAtActionResult.RouteValues["cpf"]);

            var returnValue = Assert.IsType<PessoaFisicaResponse>(createdAtActionResult.Value);
            Assert.Equal(pessoaFisica.Id, returnValue.Id);
            Assert.Equal(pessoaFisica.Nome, returnValue.Nome);
            Assert.Equal(pessoaFisica.CPF, returnValue.Documento);
        }

        [Fact]
        public async Task Test_Criar_PessoaFisica_BadRequest()
        {
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            var request = new PessoaFisicaRequest();

            var result = await _controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_Criar_PessoaFisica_ThrowsException_ServerError()
        {
            var request = new PessoaFisicaRequest
            {
                Nome = "Gustavo M. Santana",
                CPF = "49633697883",
                CEP = "04850-280",
                Numero = "35",
                Complemento = "casa verde"
            };

            _mockService.Setup(s => s.CreateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.Create(request);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Test_ListaPessoasFisicas_Ok()
        {
            var endereco = new Endereco("04850280", "Pq. Cocaia", "Grajau", "São Paulo", "SP", "35", "casa verde");
            var pessoas = new List<PessoaFisica>
            {
                new PessoaFisica("Gustavo Moreira", "49633697883", "F", endereco) {Id = 1},
                new PessoaFisica("Laiane Moreira", "50380477858", "F", endereco) {Id = 2}
            };

            _mockService.Setup(s => s.ListAsync()).ReturnsAsync(pessoas);

            var result = await _controller.List();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaFisicaResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task Test_ListaPessoas_RetornoListaCorreta()
        {
            var endereco = new Endereco("04850280", "Pq. Cocaia", "Grajau", "São Paulo", "SP", "35", "casa verde");
            var pessoas = new List<PessoaFisica>();

            for (int i = 0; i < 25; i++)
            {
                pessoas.Add(new PessoaFisica($"Pessoa {i}", $"{i}12345678", "F", endereco) { Id = i++ });
            }

            _mockService.Setup(s => s.ListAsync()).ReturnsAsync(pessoas);

            var result = await _controller.List(2, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaFisicaResponse>>(okResult.Value);
            Assert.Equal(3, returnValue.Count());
            Assert.Equal("Pessoa 20", returnValue.First().Nome);
        }

        [Fact]
        public async Task Test_ListaPessoas_ThrowException_ServerError()
        {
            _mockService.Setup(s => s.ListAsync())
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.List();

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Test_ObterPessoaViaCpf_Ok()
        {
            var cpf = "49633697883";
            var endereco = new Endereco("04850280", "Pq. Cocaia", "Grajau", "São Paulo", "SP", "35", "casa verde");
            var pessoa = new PessoaFisica("Gustavo Moreira", cpf, "F", endereco) { Id = 3 };

            _mockService.Setup(s => s.GetByCpfAsync(cpf)).ReturnsAsync(pessoa);

            var result = await _controller.GetByCpf(cpf);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaFisicaResponse>(okResult.Value);
            Assert.Equal(pessoa.Id, returnValue.Id);
            Assert.Equal(pessoa.Nome, returnValue.Nome);
            Assert.Equal(pessoa.CPF, returnValue.Documento);
        }

        [Fact]
        public async Task Test_ObterPessoaViaCpf_NotFound()
        {
            var cpf = "12345678901";
            _mockService.Setup(s => s.GetByCpfAsync(cpf)).ReturnsAsync((PessoaFisica)null);

            var result = await _controller.GetByCpf(cpf);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_Ok()
        {
            var cpf = "49633697883";
            var request = new PessoaFisicaRequest
            {
                Nome = "Gustavo Santana",
                CPF = "05723330",
                Numero = "390",
                Complemento = "AP 96B"
            };

            _mockService.Setup(s => s.UpdateByCpfAsync(
                cpf, request.Nome, request.CPF, request.CEP, request.Numero, request.Complemento))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateByCpf(cpf, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_BadRequest()
        {
            var cpf = "49633697883";
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            var request = new PessoaFisicaRequest();

            var result = await _controller.UpdateByCpf(cpf, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_ServerError()
        {
            var cpf = "49633697883";
            var request = new PessoaFisicaRequest
            {
                Nome = "Gustavo M. Santana",
                CPF = "49633697353",
                CEP = "04850-280",
                Numero = "35",
                Complemento = "casa verde"
            };

            _mockService.Setup(s => s.UpdateByCpfAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.UpdateByCpf(cpf, request);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Test_ExcluirPessoaViaCpf_Ok()
        {
            var cpf = "49633697883";
            _mockService.Setup(s => s.DeleteByCpfAsync(cpf)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteByCpf(cpf);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Test_ExcluirPessoaviaCpf_ServerError()
        {
            var cpf = "49633697883";
            _mockService.Setup(s => s.DeleteByCpfAsync(cpf))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.DeleteByCpf(cpf);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
