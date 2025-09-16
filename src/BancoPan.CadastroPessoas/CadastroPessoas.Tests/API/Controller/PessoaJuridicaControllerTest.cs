using CadastroPessoas.API.Controllers;
using CadastroPessoas.API.Models.Requests;
using CadastroPessoas.API.Models.Responses;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
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
    public class PessoaJuridicaControllerTest
    {
        private readonly Mock<IPessoaJuridicaService> _mockService;
        private readonly Mock<ILogger<PessoaJuridicaController>> _mockLogger;
        private readonly PessoaJuridicaController _controller;

        public PessoaJuridicaControllerTest()
        {
            _mockService = new Mock<IPessoaJuridicaService>();
            _mockLogger = new Mock<ILogger<PessoaJuridicaController>>();
            _controller = new PessoaJuridicaController(_mockService.Object, _mockLogger.Object);

        }

        [Fact]
        public async Task Test_CriarPessoaJuridica_Ok()
        {
            var request = new PessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59285411000113",
                CEP = "01310-100",
                Numero = "1374",
                Complemento = ""
            };

            var endereco = new Endereco("01310100", "Paulista", "Bela Vista", "São Paulo", "SP", "1374", "");
            var pessoaJuridica = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59285411000113", "J", endereco)
            {
                Id = 1
            };

            _mockService.Setup(s => s.CreateAsync(
                request.RazaoSocial, request.NomeFantasia, request.CNPJ, request.CEP, request.Numero, request.Complemento))
                .ReturnsAsync(pessoaJuridica);

            var result = await _controller.Create(request);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetByCnpj", createdAtActionResult.ActionName);
            Assert.Equal(pessoaJuridica.CNPJ, createdAtActionResult.RouteValues["cnpj"]);

            var returnValue = Assert.IsType<PessoaJuridicaResponse>(createdAtActionResult.Value);
            Assert.Equal(pessoaJuridica.Id, returnValue.Id);
            Assert.Equal(pessoaJuridica.NomeFantasia, returnValue.NomeFantasia);
            Assert.Equal(pessoaJuridica.CNPJ, returnValue.Documento);
        }
        [Fact]
        public async Task Test_CriarPessoaJuridica_BadRequest()
        {
            _controller.ModelState.AddModelError("RazaoSocial", "Razão Social é obrigatório");
            var request = new PessoaJuridicaRequest();

            var result = await _controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Test_CriarPessoaJuridica_ServerError()
        {
            var request = new PessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59285411000113",
                CEP = "01310-100",
                Numero = "1374",
                Complemento = ""
            };

            _mockService.Setup(s => s.CreateAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.Create(request);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        [Fact]
        public async Task Test_ListaPessoasJuridicas_Ok()
        {
            var endereco = new Endereco("01310100", "Paulista", "Bela Vista", "São Paulo", "SP", "1374", "");
            var pessoas = new List<PessoaJuridica>
            {
                new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59285411000113", "J", endereco) { Id = 2 },
                new PessoaJuridica("BTG Pactual S.A", "BTG Pactual", "30306294000145", "J", endereco) { Id = 3 }
            };

            _mockService.Setup(s => s.ListAsync()).ReturnsAsync(pessoas);


            var result = await _controller.List();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaJuridicaResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }
        [Fact]
        public async Task Test_ListaPessoasJuridicas_PaginaCorreta()
        {
            var endereco = new Endereco("01310100", "Paulista", "Bela Vista", "São Paulo", "SP", "1374", "");
            var pessoas = new List<PessoaJuridica>();

            for (int i = 0; i < 25; i++)
            {
                pessoas.Add(new PessoaJuridica($"RazaoSocial {i}", $"NomeFantasia {i}", $"{i}12345678", "J", endereco) { Id = i++ });
            }

            _mockService.Setup(s => s.ListAsync()).ReturnsAsync(pessoas);

            var result = await _controller.List(2, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaJuridicaResponse>>(okResult.Value);
            Assert.Equal(3, returnValue.Count());
            Assert.Equal("NomeFantasia 20", returnValue.First().NomeFantasia);
        }

        [Fact]
        public async Task Test_ListaPessoaJuridica_ThrowException_ServerError()
        {
            _mockService.Setup(s => s.ListAsync())
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.List();

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaViacnpj_Ok()
        {
            var cnpj = "59285411000113";
            var endereco = new Endereco("01310100", "Paulista", "Bela Vista", "São Paulo", "SP", "1374", "");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", cnpj, "F", endereco) { Id = 3 };

            _mockService.Setup(s => s.GetByCnpjAsync(cnpj)).ReturnsAsync(pessoa);

            var result = await _controller.GetByCnpj(cnpj);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PessoaJuridicaResponse>(okResult.Value);
            Assert.Equal(pessoa.Id, returnValue.Id);
            Assert.Equal(pessoa.NomeFantasia, returnValue.NomeFantasia);
            Assert.Equal(pessoa.CNPJ, returnValue.Documento);
        }

        [Fact]
        public async Task Test_ObterPessoaViacnpj_NotFound()
        {
            var cnpj = "59285411000113";
            _mockService.Setup(s => s.GetByCnpjAsync(cnpj)).ReturnsAsync((PessoaJuridica)null);

            var result = await _controller.GetByCnpj(cnpj);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViacnpj_Ok()
        {
            var cnpj = "59285411000113";
            var request = new PessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Panamericano",
                CNPJ = "59285411000113",
                CEP = "01310-100",
                Numero = "1300",
                Complemento = ""
            };

            _mockService.Setup(s => s.UpdateByCnpjAsync(
                cnpj, request.RazaoSocial, request.NomeFantasia, request.CNPJ, request.CEP, request.Numero, request.Complemento))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateByCnpj(cnpj, request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViacnpj_BadRequest()
        {
            var cnpj = "59285411000113";
            _controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
            var request = new PessoaJuridicaRequest();

            var result = await _controller.UpdateByCnpj(cnpj, request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Test_AtualizarPessoaViacnpj_ServerError()
        {
            var cnpj = "59285411000113";
            var request = new PessoaJuridicaRequest
            {
                RazaoSocial = "BANCO PAN S/A.",
                NomeFantasia = "Banco Pan",
                CNPJ = "59285411000113",
                CEP = "01310-100",
                Numero = "1374",
                Complemento = ""
            };

            _mockService.Setup(s => s.UpdateByCnpjAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.UpdateByCnpj(cnpj, request);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Test_ExcluirPessoaViacnpj_Ok()
        {
            var cnpj = "59285411000113";
            _mockService.Setup(s => s.DeleteByCnpjAsync(cnpj)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteByCnpj(cnpj);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Test_ExcluirPessoaviacnpj_ServerError()
        {
            var cnpj = "59285411000113";
            _mockService.Setup(s => s.DeleteByCnpjAsync(cnpj))
                .ThrowsAsync(new Exception("Erro simulado"));

            var result = await _controller.DeleteByCnpj(cnpj);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
