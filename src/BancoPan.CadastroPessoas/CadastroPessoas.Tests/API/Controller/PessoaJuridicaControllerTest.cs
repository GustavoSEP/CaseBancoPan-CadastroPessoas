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
    public class PessoaJuridicaControllerTest
    {
        private readonly Mock<ICreatePessoaJuridicaUseCase> _mockCreateUseCase;
        private readonly Mock<IGetPessoaJuridicaUseCase> _mockGetUseCase;
        private readonly Mock<IUpdatePessoaJuridicaUseCase> _mockUpdateUseCase;
        private readonly Mock<IDeletePessoaJuridicaUseCase> _mockDeleteUseCase;
        private readonly Mock<ILogger<PessoaJuridicaController>> _mockLogger;
        private readonly PessoaJuridicaController _controller;

        public PessoaJuridicaControllerTest()
        {
            _mockCreateUseCase = new Mock<ICreatePessoaJuridicaUseCase>();
            _mockGetUseCase = new Mock<IGetPessoaJuridicaUseCase>();
            _mockUpdateUseCase = new Mock<IUpdatePessoaJuridicaUseCase>();
            _mockDeleteUseCase = new Mock<IDeletePessoaJuridicaUseCase>();
            _mockLogger = new Mock<ILogger<PessoaJuridicaController>>();
            
            _controller = new PessoaJuridicaController(
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
                Cep = "01310100",
                Logradouro = "Paulista",
                Bairro = "Bela Vista",
                Cidade = "São Paulo",
                Estado = "SP",
                Numero = "1374",
                Complemento = ""
            };
        }

        [Fact]
        public async Task Test_CriarPessoaJuridica_Ok()
        {
            // Arrange
            var request = new PessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59285411000113",
                CEP = "01310-100",
                Numero = "1374",
                Complemento = ""
            };

            var endereco = CriarEnderecoDto();
            var pessoaJuridicaDto = new PessoaJuridicaDto
            {
                Id = 1,
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59.285.411/0001-13",
                Endereco = endereco,
                DataCadastro = DateTime.Now
            };

            _mockCreateUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CreatePessoaJuridicaCommand>()))
                .ReturnsAsync(pessoaJuridicaDto);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetByCnpj", createdAtActionResult.ActionName);
            Assert.Equal(pessoaJuridicaDto.CNPJ, createdAtActionResult.RouteValues["cnpj"]);

            var returnValue = Assert.IsType<PessoaJuridicaResponse>(createdAtActionResult.Value);
            Assert.Equal(pessoaJuridicaDto.Id, returnValue.Id);
            Assert.Equal(pessoaJuridicaDto.NomeFantasia, returnValue.NomeFantasia);
            Assert.Equal(pessoaJuridicaDto.CNPJ, returnValue.CNPJ);
        }
        
        [Fact]
        public async Task Test_CriarPessoaJuridica_BadRequest()
        {
            // Arrange
            var request = new PessoaJuridicaRequest();
            
            _mockCreateUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CreatePessoaJuridicaCommand>()))
                .ThrowsAsync(new ValidationException("Razão Social é obrigatório"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Razão Social é obrigatório", badRequestResult.Value);
        }
        
        [Fact]
        public async Task Test_CriarPessoaJuridica_ServerError()
        {
            // Arrange
            var request = new PessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59285411000113",
                CEP = "01310-100",
                Numero = "1374",
                Complemento = ""
            };

            _mockCreateUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CreatePessoaJuridicaCommand>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
        }
        
        [Fact]
        public async Task Test_ListaPessoasJuridicas_Ok()
        {
            // Arrange
            var endereco = CriarEnderecoDto();
            var pessoas = new List<PessoaJuridicaDto>
            {
                new PessoaJuridicaDto
                {
                    Id = 2,
                    RazaoSocial = "BANCO PAN S/A.",
                    NomeFantasia = "Banco Pan",
                    CNPJ = "59.285.411/0001-13",
                    Endereco = endereco,
                    DataCadastro = DateTime.Now
                },
                new PessoaJuridicaDto
                {
                    Id = 3,
                    RazaoSocial = "BTG Pactual S.A",
                    NomeFantasia = "BTG Pactual",
                    CNPJ = "30.306.294/0001-45",
                    Endereco = endereco,
                    DataCadastro = DateTime.Now
                }
            };

            _mockGetUseCase.Setup(s => s.GetAllAsync()).ReturnsAsync(pessoas);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<PessoaJuridicaResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal(pessoas[0].Id, returnValue[0].Id);
            Assert.Equal(pessoas[0].RazaoSocial, returnValue[0].RazaoSocial);
            Assert.Equal(pessoas[0].CNPJ, returnValue[0].CNPJ);
        }

        [Fact]
        public async Task Test_ListaPessoaJuridica_ThrowException_ServerError()
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
        public async Task Test_ObterPessoaJuridicaViaCnpj_Ok()
        {
            // Arrange
            var cnpj = "59285411000113";
            var endereco = CriarEnderecoDto();
            var pessoa = new PessoaJuridicaDto
            {
                Id = 3,
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59.285.411/0001-13",
                Endereco = endereco,
                DataCadastro = DateTime.Now
            };

            _mockGetUseCase.Setup(s => s.GetByCnpjAsync(cnpj)).ReturnsAsync(pessoa);

            // Act
            var result = await _controller.GetByCnpj(cnpj);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaJuridicaResponse>(okResult.Value);
            Assert.Equal(pessoa.Id, returnValue.Id);
            Assert.Equal(pessoa.NomeFantasia, returnValue.NomeFantasia);
            Assert.Equal(pessoa.CNPJ, returnValue.CNPJ);
        }

        [Fact]
        public async Task Test_ObterPessoaViaCnpj_NotFound()
        {
            // Arrange
            var cnpj = "59285411000113";
            _mockGetUseCase.Setup(s => s.GetByCnpjAsync(cnpj))
                .ThrowsAsync(new ValidationException("Pessoa jurídica não encontrada"));

            // Act
            var result = await _controller.GetByCnpj(cnpj);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa jurídica não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_AtualizarPessoaById_Ok()
        {
            // Arrange
            var id = 1;
            var request = new UpdatePessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Panamericano",
                CEP = "01310-100",
                Numero = "1300",
                Complemento = ""
            };

            var updateResult = new PessoaJuridicaDto
            {
                Id = id,
                RazaoSocial = request.RazaoSocial,
                NomeFantasia = request.NomeFantasia,
                CNPJ = "59.285.411/0001-13",
                Endereco = new EnderecoDto
                {
                    Cep = request.CEP,
                    Logradouro = "Paulista",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Numero = request.Numero,
                    Complemento = request.Complemento
                },
                DataCadastro = DateTime.Now
            };

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaJuridicaCommand>()))
                .ReturnsAsync(updateResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaJuridicaResponse>(okResult.Value);
            Assert.Equal(updateResult.Id, returnValue.Id);
            Assert.Equal(updateResult.NomeFantasia, returnValue.NomeFantasia);
            Assert.Equal(updateResult.CNPJ, returnValue.CNPJ);
        }

        [Fact]
        public async Task Test_AtualizarPessoaById_BadRequest()
        {
            // Arrange
            var id = 1;
            var request = new UpdatePessoaJuridicaRequest();

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaJuridicaCommand>()))
                .ThrowsAsync(new ValidationException("Dados inválidos"));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dados inválidos", badRequestResult.Value);
        }

        [Fact]
        public async Task Test_AtualizarPessoaById_NotFound()
        {
            // Arrange
            var id = 999;
            var request = new UpdatePessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Panamericano",
                CEP = "01310-100",
                Numero = "1300",
                Complemento = ""
            };

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaJuridicaCommand>()))
                .ThrowsAsync(new ValidationException("Pessoa jurídica não encontrada"));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa jurídica não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_AtualizarPessoaById_ServerError()
        {
            // Arrange
            var id = 1;
            var request = new UpdatePessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CEP = "01310-100",
                Numero = "1374",
                Complemento = ""
            };

            _mockUpdateUseCase.Setup(s => s.ExecuteAsync(id, It.IsAny<UpdatePessoaJuridicaCommand>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor", statusCodeResult.Value);
        }

        [Fact]
        public async Task Test_ExcluirPessoaById_Ok()
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
        public async Task Test_ExcluirPessoaById_NotFound()
        {
            // Arrange
            var id = 999;
            _mockDeleteUseCase.Setup(s => s.ExecuteAsync(id))
                .ThrowsAsync(new ValidationException("Pessoa jurídica não encontrada"));

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa jurídica não encontrada", notFoundResult.Value);
        }

        [Fact]
        public async Task Test_ExcluirPessoaById_ServerError()
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

        [Fact]
        public async Task Test_GetById_ReturnsOkWithPessoa()
        {
            // Arrange
            var id = 1;
            var endereco = CriarEnderecoDto();
            var pessoa = new PessoaJuridicaDto
            {
                Id = id,
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59.285.411/0001-13",
                Endereco = endereco,
                DataCadastro = DateTime.Now
            };

            _mockGetUseCase.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(pessoa);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaJuridicaResponse>(okResult.Value);
            Assert.Equal(pessoa.Id, returnValue.Id);
            Assert.Equal(pessoa.NomeFantasia, returnValue.NomeFantasia);
            Assert.Equal(pessoa.CNPJ, returnValue.CNPJ);
        }

        [Fact]
        public async Task Test_GetById_ReturnsNotFound()
        {
            // Arrange
            var id = 999;
            _mockGetUseCase.Setup(s => s.GetByIdAsync(id))
                .ThrowsAsync(new ValidationException("Pessoa jurídica não encontrada"));

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Pessoa jurídica não encontrada", notFoundResult.Value);
        }
    }
}