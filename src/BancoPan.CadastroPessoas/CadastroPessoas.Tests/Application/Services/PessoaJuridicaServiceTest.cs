using CadastroPessoas.Application.Services;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Tests.Application.Services
{
    public class PessoaJuridicaServiceTest
    {
        private readonly Mock<IPessoaRepository> _mockRepository;
        private readonly Mock<IViaCepService> _mockViaCepService;
        private readonly Mock<ILogger<PessoaJuridicaService>> _mockLogger;
        private readonly PessoaJuridicaService _service;

        private int _idSequencial = 0;
         
        private const string CNPJ_VALIDO = "59.285.411/0001-13";
        private const string CNPJ_VALIDO_NORMALIZADO = "59285411000113";

        public PessoaJuridicaServiceTest()
        {
            _mockRepository = new Mock<IPessoaRepository>();
            _mockViaCepService = new Mock<IViaCepService>();
            _mockLogger = new Mock<ILogger<PessoaJuridicaService>>();

            _service = new PessoaJuridicaService(
                _mockRepository.Object,
                _mockViaCepService.Object,
                _mockLogger.Object
                );
        }

        private Endereco CriarEnderecoMock()
        {
            return new Endereco(
                "01310100",
                "Av. Paulista",
                "Bela Vista",
                "São Paulo",
                "SP",
                "1374",
                "10 Andar"
                );
        }

        private PessoaJuridica CriarPessoaJuridicaMock()
        {
            var endereco = CriarEnderecoMock();
            _idSequencial++;
            return new PessoaJuridica("BANCO PAN S/A.", "BANCO PAN", CNPJ_VALIDO, "J", endereco)
            {
                Id = _idSequencial
            };
        }

        [Fact]
        public async Task Test_CriarPessoaJuridicaComDadosValidos_Ok()
        {
            var endereco = CriarEnderecoMock();
            var pessoaCriada = new PessoaJuridica("BANCO PAN S/A.", "BANCO PAN", CNPJ_VALIDO, "J", endereco)
            {
                Id = _idSequencial++
            };

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("01310100"))
                .ReturnsAsync(endereco);

            _mockRepository.Setup(r => r.ExistsPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddPessoaJuridicaAsync(It.IsAny<PessoaJuridica>()))
                .ReturnsAsync(pessoaCriada);

            var resultado = await _service.CreateAsync(
                "BANCO PAN S/A.",
                "BANCO PAN",
                CNPJ_VALIDO_NORMALIZADO,
                "01310100",
                "1374",
                "10 Andar"
                );

            Assert.NotNull(resultado);
            Assert.Equal(pessoaCriada.Id, resultado.Id);
            Assert.Equal("BANCO PAN S/A.", resultado.RazaoSocial);
            Assert.Equal("BANCO PAN", resultado.NomeFantasia);
            Assert.Equal(CNPJ_VALIDO, resultado.CNPJ);
            Assert.Equal("J", resultado.TipoPessoa);

            Assert.Equal("01310100", resultado.Endereco.Cep);
            Assert.Equal("São Paulo", resultado.Endereco.Cidade);
            Assert.Equal("SP", resultado.Endereco.Estado);
        }

        [Fact]
        public async Task Test_CriarPessoaJuridicaComDadosInvalidos_ExceptionError()
        {
            var cnpjInvalido = "12.345.678/0009-10";

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateAsync("BANCO PAN S/A.", "BANCO PAN", cnpjInvalido, "01310100", "1374", "10 Andar")
            );

            Assert.Equal("CNPJ inválido.", exception.Message);
        }

        [Fact]
        public async Task Test_CriarPessoaJuridicaComCNPJExistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.ExistsPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.CreateAsync("BANCO PAN S/A.", "BANCO PAN", CNPJ_VALIDO_NORMALIZADO, "01310100", "1374", "10 Andar")
            );

            Assert.Equal("Pessoa jurídica com esse CNPJ já existe.", exception.Message);
        }

        [Fact]
        public async Task Test_CriarPessoaJuridicaComCepInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.ExistsPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(false);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("01310100"))
                .ReturnsAsync((Endereco)null);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateAsync("BANCO PAN S/A.", "BANCO PAN", CNPJ_VALIDO_NORMALIZADO, "01310100", "1374", "10 Andar")
            );

            Assert.Equal("Endereço não encontrado para o CEP informado.", exception.Message);
        }

        [Fact]
        public async Task Test_ListaTodasAsPessoasFisicas_Ok()
        {
            var pessoas = new List<PessoaJuridica>
            {
                CriarPessoaJuridicaMock(),
                CriarPessoaJuridicaMock(),
                CriarPessoaJuridicaMock()
            };

            _mockRepository.Setup(r => r.ListPessoaJuridicaAsync())
                .ReturnsAsync(pessoas);

            var resultado = await _service.ListAsync();

            var pessoasList = resultado.ToList();
            Assert.Equal(3, pessoasList.Count);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDoCNPJ_Ok()
        {
            var pessoa = CriarPessoaJuridicaMock();

            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(pessoa);

            var resultado = await _service.GetByCnpjAsync(CNPJ_VALIDO);

            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal(pessoa.RazaoSocial, resultado.RazaoSocial);
            Assert.Equal(pessoa.NomeFantasia, resultado.NomeFantasia);
            Assert.Equal(pessoa.CNPJ, resultado.CNPJ);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaComCNPJInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync((PessoaJuridica)null);

            var resultado = await _service.GetByCnpjAsync(CNPJ_VALIDO);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaComCNPJVazio_ExceptionError()
        {

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.GetByCnpjAsync("")
            );

            Assert.Equal("CNPJ é obrigatório.", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaJuridica_Ok()
        {
            var pessoa = CriarPessoaJuridicaMock();
            var novaRazaoSocial = "BTG Pactual S/A.";
            var novoNomeFantasia = "BTG Pactual";

            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(pessoa);

            await _service.UpdateByCnpjAsync(CNPJ_VALIDO, novaRazaoSocial, novoNomeFantasia, null, null, null, null);

            _mockRepository.Verify(r => r.UpdatePessoaJuridicaAsync(It.Is<PessoaJuridica>(p =>
                p.Id == pessoa.Id &&
                p.NomeFantasia == novoNomeFantasia &&
                p.CNPJ == CNPJ_VALIDO
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateByCNPJAsync_ComCepValido_DeveAtualizarEndereco()
        {
            var pessoa = CriarPessoaJuridicaMock();
            var novoEndereco = new Endereco(
                "04538133",
                "Av. Brig. Faria Lima",
                "Itaim Bibi",
                "São Paulo",
                "SP",
                "3477",
                "14º Andar"
            );

            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(pessoa);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("04538133"))
                .ReturnsAsync(novoEndereco);

            await _service.UpdateByCnpjAsync(CNPJ_VALIDO, null, null, null, "04538133", "3477", "14º Andar");

            _mockRepository.Verify(r => r.UpdatePessoaJuridicaAsync(It.Is<PessoaJuridica>(p =>
                p.Id == pessoa.Id &&
                p.Endereco.Cep == "04538133" &&
                p.Endereco.Logradouro == "Av. Brig. Faria Lima" &&
                p.Endereco.Cidade == "São Paulo" &&
                p.Endereco.Numero == "3477" &&
                p.Endereco.Complemento == "14º Andar"
            )), Times.Once);
        }

        [Fact]
        public async Task Test_AtualizarPessoaComCNPJInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync((PessoaJuridica)null);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.UpdateByCnpjAsync(CNPJ_VALIDO, "Nova RazaoSocial", "Novo NomeFantasia", null, null, null, null)
            );

            Assert.Equal("Pessoa jurídica não encontrada.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCNPJExistente_Ok()
        {
            var pessoa = CriarPessoaJuridicaMock();

            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync(pessoa);

            await _service.DeleteByCnpjAsync(CNPJ_VALIDO);

            _mockRepository.Verify(r => r.DeletePessoaJuridicaAsync(pessoa.Id), Times.Once);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCNPJInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.GetPessoaJuridicaByCnpjAsync(CNPJ_VALIDO))
                .ReturnsAsync((PessoaJuridica)null);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.DeleteByCnpjAsync(CNPJ_VALIDO)
            );

            Assert.Equal("Pessoa jurídica não encontrada.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCNPJVazio_ExceptionError()
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.DeleteByCnpjAsync("")
            );

            Assert.Equal("CNPJ é obrigatório para exclusão.", exception.Message);
        }
    }
}
