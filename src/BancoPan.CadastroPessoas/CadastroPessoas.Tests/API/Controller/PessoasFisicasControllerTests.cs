using CadastroPessoas.API.Controllers;
using CadastroPessoas.API.Models.Requests;
using CadastroPessoas.API.Models.Responses;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Tests.API.Controller
{
    public class PessoasFisicasControllerTests
    {
        private readonly Mock<IPessoaFisicaService> _mockService;
        private readonly Mock<ILogger<PessoasFisicasController>> _mockLogger;
        private readonly PessoasFisicasController _controller;

        public PessoasFisicasControllerTests()
        {
            _mockService = new Mock<IPessoaFisicaService>();
            _mockLogger = new Mock<ILogger<PessoasFisicasController>>();
            _controller = new PessoasFisicasController(_mockService.Object, _mockLogger.Object);

        }

        [Fact]
        public async Task Test_Create_PessoaFisica_ok()
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
        public async Task Test_Create_PessoaFisica_BadRequest()
        {
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            var request = new PessoaFisicaRequest();

            var result = await _controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_Create_PessoaFisica_ThrowsException_ServerError()
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
    }
}
