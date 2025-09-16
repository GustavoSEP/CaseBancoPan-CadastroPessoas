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
    }
}
