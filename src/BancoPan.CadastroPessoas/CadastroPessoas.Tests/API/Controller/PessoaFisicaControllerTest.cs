using CadastroPessoas.Adapters.Input.Api.Controllers;
using CadastroPessoas.Adapters.Input.Api.Models;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;
using CadastroPessoas.Ports.Input.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CadastroPessoas.Tests.API.Controller
{
    public class PessoaFisicaControllerTest
    {
        private readonly Mock<ICreatePessoaFisicaUseCase> _mockCreateUseCase;
        private readonly Mock<IGetPessoaFisicaUseCase> _mockGetUseCase;
        private readonly Mock<IUpdatePessoaFisicaUseCase> _mockUpdateUseCase;
        private readonly Mock<IDeletePessoaFisicaUseCase> _mockDeleteUseCase;
        private readonly Mock<ILogger<PessoaFisicaController>> _mockLogger;
        private readonly PessoaFisicaController _controller;

        public PessoaFisicaControllerTest()
        {
            _mockCreateUseCase = new Mock<ICreatePessoaFisicaUseCase>();
            _mockGetUseCase = new Mock<IGetPessoaFisicaUseCase>();
            _mockUpdateUseCase = new Mock<IUpdatePessoaFisicaUseCase>();
            _mockDeleteUseCase = new Mock<IDeletePessoaFisicaUseCase>();
            _mockLogger = new Mock<ILogger<PessoaFisicaController>>();
            
            _controller = new PessoaFisicaController(
                _mockCreateUseCase.Object,
                _mockGetUseCase.Object,
                _mockUpdateUseCase.Object,
                _mockDeleteUseCase.Object,
                _mockLogger.Object);
        }

        private EnderecoDto CriarEnderecoDto()
        {
            return new EnderecoDto
            {
                Cep = "04850280",
                Logradouro = "Pq. Cocaia",
                Bairro = "Grajau",
                Cidade = "São Paulo",
                Estado = "SP",
                Numero = "35",
                Complemento = "casa verde"
            };
        }

        [Fact]
        public async Task Test_Criar_PessoaFisica_ok()
        {
            // Arrange
            var request = new PessoaFisicaRequest
            {
                Nome = "Gustavo M. Santana",
                CPF = "49633697883",
                CEP = "04850-280",
                Numero = "35",
                Complemento = "casa verde"
            };

            var endereco = CriarEnderecoDto();
            var pessoaFisicaDto = new PessoaFisicaDto
            {
                Id = 1,
                Nome = "Gustavo M Santana",
                CPF = "496.336.978-83",
                Endereco = endereco,
                DataCadastro = DateTime.Now
            };

            _mockCreateUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CreatePessoaFisicaCommand>()))
                .ReturnsAsync(pessoaFisicaDto);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetByCpf", createdAtActionResult.ActionName);
            Assert.Equal(pessoaFisicaDto.CPF, createdAtActionResult.RouteValues["cpf"]);

            var returnValue = Assert.IsType<PessoaFisicaResponse>(createdAtActionResult.Value);
            Assert.Equal(pessoaFisicaDto.Id, returnValue.Id);
            Assert.Equal(pessoaFisicaDto.Nome, returnValue.Nome);
            Assert.Equal(pessoaFisicaDto.CPF, returnValue.CPF);
        }

        [Fact]
        public async Task Test_Criar_PessoaFisica_BadRequest()
        {
            // Arrange
            var request = new PessoaFisicaRequest();
            
            _mockCreateUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CreatePessoaFisicaCommand>()))
                .ThrowsAsync(new ValidationException("Dados inválidos"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dados inválidos", badRequestResult.Value);
        }

        [Fact]
        public async Task Test_Criar_PessoaFisica_ThrowsException_ServerError()
        {
            // Arrange
            var request = new PessoaFisicaRequest
            {
                Nome = "Gustavo M. Santana",
                CPF = "49633697883",
                CEP = "04850-280",
                Numero = "35",
                Complemento = "casa verde"
            };

            _mockCreateUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CreatePessoaFisicaCommand>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
        }

        [Fact]
        public async Task Test_ListaPessoasFisicas_Ok()
        {
            // Arrange
            var endereco = CriarEnderecoDto();
            var pessoas = new List<PessoaFisicaDto>
            {
                new PessoaFisicaDto
                {
                    Id = 1,
                    Nome = "Gustavo Moreira",
                    CPF = "496.336.978-83",
                    Endereco = endereco,
                    DataCadastro = DateTime.Now
                },
                new PessoaFisicaDto
                {
                    Id = 2,
                    Nome = "Laiane Moreira",
                    CPF = "503.804.778-58",
                    Endereco = endereco,
                    DataCadastro = DateTime.Now
                }
            };

            _mockGetUseCase.Setup(s => s.GetAllAsync()).ReturnsAsync(pessoas);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<PessoaFisicaResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal(pessoas[0].Id, returnValue[0].Id);
            Assert.Equal(pessoas[0].Nome, returnValue[0].Nome);
            Assert.Equal(pessoas[0].CPF, returnValue[0].CPF);
        }

        [Fact]
        public async Task Test_ListaPessoas_ThrowException_ServerError()
        {
            // Arrange
            _mockGetUseCase.Setup(s => s.GetAllAsync())
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
        }

        [Fact]
        public async Task Test_ObterPessoaViaCpf_Ok()
        {
            // Arrange
            var cpf = "49633697883";
            var endereco = CriarEnderecoDto();
            var pessoa = new PessoaFisicaDto
            {
                Id = 3,
                Nome = "Gustavo Moreira",
                CPF = "496.336.978-83",
                Endereco = endereco,
                DataCadastro = DateTime.Now
            };

            _mockGetUseCase.Setup(s => s.GetByCpfAsync(cpf)).ReturnsAsync(pessoa);

            // Act
            var result = await _controller.GetByCpf(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaFisicaResponse>(okResult.Value);
            Assert.Equal(pessoa.Id, returnValue.Id);
            Assert.Equal(pessoa.Nome, returnValue.Nome);
            Assert.Equal(pessoa.CPF, returnValue.CPF);
        }

        [Fact]
        public async Task Test_ObterPessoaViaCpf_NotFound()
        {
            // Arrange
            var cpf = "12345678901";
            _mockGetUseCase.Setup(s => s.GetByCpfAsync(cpf))
                .ThrowsAsync(new ValidationException("Pessoa não encontrada"));

            // Act
            var result = await _controller.GetByCpf(cpf);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_GetById_ReturnsOkWithPessoa()
        {
            // Arrange
            var id = 1;
            var endereco = CriarEnderecoDto();
            var pessoa = new PessoaFisicaDto
            {
                Id = id,
                Nome = "Gustavo Moreira",
                CPF = "496.336.978-83",
                Endereco = endereco,
                DataCadastro = DateTime.Now
            };

            _mockGetUseCase.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(pessoa);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaFisicaResponse>(okResult.Value);
            Assert.Equal(pessoa.Id, returnValue.Id);
            Assert.Equal(pessoa.Nome, returnValue.Nome);
            Assert.Equal(pessoa.CPF, returnValue.CPF);
        }

        [Fact]
        public async Task Test_GetById_ReturnsNotFound()
        {
            // Arrange
            var id = 999;
            _mockGetUseCase.Setup(s => s.GetByIdAsync(id))
                .ThrowsAsync(new ValidationException("Pessoa não encontrada"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_Ok()
        {
            // Arrange
            var id = 1;
            var request = new UpdatePessoaFisicaRequest
            {
                Nome = "Gustavo Santana",
                CEP = "05723330",
                Numero = "390",
                Complemento = "AP 96B"
            };

            var updateResult = new PessoaFisicaDto
            {
                Id = id,
                Nome = request.Nome,
                CPF = "496.336.978-83",
                Endereco = new EnderecoDto
                {
                    Cep = request.CEP,
                    Logradouro = "Rua Nova",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Numero = request.Numero,
                    Complemento = request.Complemento
                },
                DataCadastro = DateTime.Now
            };

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaFisicaCommand>()))
                .ReturnsAsync(updateResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaFisicaResponse>(okResult.Value);
            Assert.Equal(updateResult.Id, returnValue.Id);
            Assert.Equal(updateResult.Nome, returnValue.Nome);
            Assert.Equal(updateResult.CPF, returnValue.CPF);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_BadRequest()
        {
            // Arrange
            var id = 1;
            var request = new UpdatePessoaFisicaRequest();

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaFisicaCommand>()))
                .ThrowsAsync(new ValidationException("Dados inválidos"));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dados inválidos", badRequestResult.Value);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_NotFound()
        {
            // Arrange
            var id = 999;
            var request = new UpdatePessoaFisicaRequest
            {
                Nome = "Gustavo Santana",
                CEP = "05723330",
                Numero = "390",
                Complemento = "AP 96B"
            };

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaFisicaCommand>()))
                .ThrowsAsync(new ValidationException("Pessoa não encontrada"));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViaCpf_ServerError()
        {
            // Arrange
            var id = 1;
            var request = new UpdatePessoaFisicaRequest
            {
                Nome = "Gustavo M. Santana",
                CEP = "04850-280",
                Numero = "35",
                Complemento = "casa verde"
            };

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaFisicaCommand>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
        }

        [Fact]
        public async Task Test_ExcluirPessoaViaCpf_Ok()
        {
            // Arrange
            var id = 1;
            _mockDeleteUseCase.Setup(s => s.ExecuteAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Test_ExcluirPessoaViaCpf_NotFound()
        {
            // Arrange
            var id = 999;
            _mockDeleteUseCase.Setup(s => s.ExecuteAsync(id))
                .ThrowsAsync(new ValidationException("Pessoa não encontrada"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_ExcluirPessoaViaCpf_ServerError()
        {
            // Arrange
            var id = 1;
            _mockDeleteUseCase.Setup(s => s.ExecuteAsync(id))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
        }
    }
}