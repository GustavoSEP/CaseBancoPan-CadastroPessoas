using CadastroPessoas.Application.Services;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using Castle.Core.Logging;
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
    public class PessoaFisicaServiceTest
    {
        private readonly Mock<IPessoaRepository> _mockRepository;
        private readonly Mock<IViaCepService> _mockViaCepService;
        private readonly Mock<ILogger<PessoaFisicaService>> _mockLogger;
        private readonly PessoaFisicaService _service;

        private int _idSequencial = 0;

        private const string CPF_VALIDO = "496.336.978-83";
        private const string CPF_VALIDO_NORMALIZADO = "49633697883";

        public PessoaFisicaServiceTest()
        {
            _mockRepository = new Mock<IPessoaRepository>();
            _mockViaCepService = new Mock<IViaCepService>();
            _mockLogger = new Mock<ILogger<PessoaFisicaService>>();

            _service = new PessoaFisicaService(
                _mockRepository.Object,
                _mockViaCepService.Object,
                _mockLogger.Object
                );
        }

        private Endereco CriarEnderecoMock()
        {
            return new Endereco(
                "05723330",
                "Vila Andrade",
                "Santo Amaro",
                "São Paulo",
                "SP",
                "390",
                "Ap 96B"
                );
        }

        private PessoaFisica CriarPessoaFisicaMock()
        {
            var endereco = CriarEnderecoMock();
            _idSequencial++;
            return new PessoaFisica("Gustavo M. Santana", CPF_VALIDO, "F", endereco)
            {
                Id = _idSequencial
            };
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComDadosValidos_Ok()
        {
            var endereco = CriarEnderecoMock();
            var pessoaCriada = new PessoaFisica("Gustavo M. Santana", CPF_VALIDO, "F", endereco)
            {
                Id = _idSequencial++
            };

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("05723330"))
                .ReturnsAsync(endereco);

            _mockRepository.Setup(r => r.ExistsPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddPessoaFisicaAsync(It.IsAny<PessoaFisica>()))
                .ReturnsAsync(pessoaCriada);

            var resultado = await _service.CreateAsync(
                "Gustavo M. Santana",
                CPF_VALIDO_NORMALIZADO,
                "05723330",
                "390",
                "AP 96B"
                );

            Assert.NotNull(resultado);
            Assert.Equal(pessoaCriada.Id, resultado.Id);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
            Assert.Equal(CPF_VALIDO, resultado.CPF);
            Assert.Equal("F", resultado.TipoPessoa);

            Assert.Equal("05723330", resultado.Endereco.Cep);
            Assert.Equal("São Paulo", resultado.Endereco.Cidade);
            Assert.Equal("SP", resultado.Endereco.Estado);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComDadosInvalidos_ExceptionError()
        {
            var cpfInvalido = "111.111.111-11";

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateAsync("Gustavo Moreira", cpfInvalido, "04850280", "09", "Ao lado do 75")
            );

            Assert.Equal("CPF inválido.", exception.Message);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComCpfExistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.ExistsPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.CreateAsync("Gustavo Moreira", CPF_VALIDO_NORMALIZADO, "04850280", "09", "Ao lado do 75")
            );

            Assert.Equal("Pessoa física com esse CPF já existe.", exception.Message);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComCepInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.ExistsPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(false);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("04850280"))
                .ReturnsAsync((Endereco)null);

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateAsync("Gustavo Moreira", CPF_VALIDO_NORMALIZADO, "04850280", "09", "Ao lado do 75")
            );

            Assert.Equal("Endereço não encontrado para o CEP informado.", exception.Message);
        }

        [Fact]
        public async Task Test_ListaTodasAsPessoasFisicas_Ok()
        {
            var pessoas = new List<PessoaFisica>
            {
                CriarPessoaFisicaMock(),
                CriarPessoaFisicaMock(),
                CriarPessoaFisicaMock()
            };

            _mockRepository.Setup(r => r.ListPessoaFisicaAsync())
                .ReturnsAsync(pessoas);

            var resultado = await _service.ListAsync();

            var pessoasList = resultado.ToList();
            Assert.Equal(3, pessoasList.Count);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDoCpf_Ok()
        {
            var pessoa = CriarPessoaFisicaMock();

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            var resultado = await _service.GetByCpfAsync(CPF_VALIDO);

            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal(pessoa.Nome, resultado.Nome);
            Assert.Equal(pessoa.CPF, resultado.CPF);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaComCpfInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync((PessoaFisica)null);

            var resultado = await _service.GetByCpfAsync(CPF_VALIDO);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaComCpfVazio_ExceptionError()
        {

            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.GetByCpfAsync("")
            );

            Assert.Equal("CPF é obrigatório.", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaFisica_Ok()
        {
            var pessoa = CriarPessoaFisicaMock();
            var novoNome = "Gustavo Moreira G. Silva";

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            await _service.UpdateByCpfAsync(CPF_VALIDO, novoNome, null, null, null, null);

            _mockRepository.Verify(r => r.UpdatePessoaFisicaAsync(It.Is<PessoaFisica>(p =>
                p.Id == pessoa.Id &&
                p.Nome == novoNome &&
                p.CPF == CPF_VALIDO
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateByCpfAsync_ComCepValido_DeveAtualizarEndereco()
        {
            // Arrange
            var pessoa = CriarPessoaFisicaMock();
            var novoEndereco = new Endereco(
                "04850280",
                "Rua Ourentã",
                "Grajau",
                "São Paulo",
                "SP",
                "110",
                "Fundos"
            );

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("04850280"))
                .ReturnsAsync(novoEndereco);

            // Act
            await _service.UpdateByCpfAsync(CPF_VALIDO, null, null, "04850280", "110", "Fundos");

            // Assert
            _mockRepository.Verify(r => r.UpdatePessoaFisicaAsync(It.Is<PessoaFisica>(p =>
                p.Id == pessoa.Id &&
                p.Endereco.Cep == "04850280" &&
                p.Endereco.Logradouro == "Rua Ourentã" &&
                p.Endereco.Cidade == "São Paulo" &&
                p.Endereco.Numero == "110" &&
                p.Endereco.Complemento == "Fundos"
            )), Times.Once);
        }

        [Fact]
        public async Task Test_AtualizarPessoaComCpfInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync((PessoaFisica)null);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.UpdateByCpfAsync(CPF_VALIDO, "Novo Nome", null, null, null, null)
            );

            Assert.Equal("Pessoa física não encontrada.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfExistente_Ok()
        {
            var pessoa = CriarPessoaFisicaMock();

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            await _service.DeleteByCpfAsync(CPF_VALIDO);

            _mockRepository.Verify(r => r.DeletePessoaFisicaAsync(pessoa.Id), Times.Once);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfInexistente_ExceptionError()
        {
            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync((PessoaFisica)null);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.DeleteByCpfAsync(CPF_VALIDO)
            );

            Assert.Equal("Pessoa física não encontrada.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfVazio_ExceptionError()
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.DeleteByCpfAsync("")
            );

            Assert.Equal("CPF é obrigatório para exclusão.", exception.Message);
        }
    }
}
