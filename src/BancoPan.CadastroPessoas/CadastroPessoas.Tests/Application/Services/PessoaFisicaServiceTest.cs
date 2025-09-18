using CadastroPessoas.Application.Helpers;
using CadastroPessoas.Application.Services;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
            return new PessoaFisica(_idSequencial, "Gustavo M. Santana", CPF_VALIDO, "F", endereco);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComDadosValidos_Ok()
        {
            // Arrange
            var endereco = CriarEnderecoMock();
            var pessoaCriada = new PessoaFisica(_idSequencial++, "Gustavo M. Santana", CPF_VALIDO, "F", endereco);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("05723330"))
                .ReturnsAsync(endereco);

            _mockRepository.Setup(r => r.ExistsPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddPessoaFisicaAsync(It.IsAny<PessoaFisica>()))
                .ReturnsAsync(pessoaCriada);

            // Act
            var resultado = await _service.CreateAsync(
                "Gustavo M. Santana",
                CPF_VALIDO_NORMALIZADO,
                "05723330",
                "390",
                "AP 96B"
                );

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoaCriada.Id, resultado.Id);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
            Assert.Equal(CPF_VALIDO, resultado.CPF);
            Assert.Equal("F", resultado.Tipo);

            Assert.Equal("05723330", resultado.Endereco.Cep);
            Assert.Equal("São Paulo", resultado.Endereco.Cidade);
            Assert.Equal("SP", resultado.Endereco.Estado);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComDadosInvalidos_ExceptionError()
        {
            // Arrange
            var cpfInvalido = "111.111.111-11";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateAsync("Gustavo Moreira", cpfInvalido, "04850280", "09", "Ao lado do 75")
            );

            Assert.Equal("CPF inválido.", exception.Message);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComCpfExistente_ExceptionError()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.CreateAsync("Gustavo Moreira", CPF_VALIDO_NORMALIZADO, "04850280", "09", "Ao lado do 75")
            );

            Assert.Equal("Pessoa física com esse CPF já existe.", exception.Message);
        }

        [Fact]
        public async Task Test_CriarPessoaFisicaComCepInexistente_ExceptionError()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(false);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync("04850280"))
                .ReturnsAsync((Endereco)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.CreateAsync("Gustavo Moreira", CPF_VALIDO_NORMALIZADO, "04850280", "09", "Ao lado do 75")
            );

            Assert.Equal("Endereço não encontrado para o CEP informado.", exception.Message);
        }

        [Fact]
        public async Task Test_ListaTodasAsPessoasFisicas_Ok()
        {
            // Arrange
            var pessoas = new List<PessoaFisica>
            {
                CriarPessoaFisicaMock(),
                CriarPessoaFisicaMock(),
                CriarPessoaFisicaMock()
            };

            _mockRepository.Setup(r => r.ListPessoaFisicaAsync())
                .ReturnsAsync(pessoas);

            // Act
            var resultado = await _service.ListAsync();

            // Assert
            var pessoasList = resultado.ToList();
            Assert.Equal(3, pessoasList.Count);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDoCpf_Ok()
        {
            // Arrange
            var pessoa = CriarPessoaFisicaMock();

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            // Act
            var resultado = await _service.GetByCpfAsync(CPF_VALIDO);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal(pessoa.Nome, resultado.Nome);
            Assert.Equal(pessoa.CPF, resultado.CPF);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaComCpfInexistente_RetornaNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync((PessoaFisica)null);

            // Act
            var resultado = await _service.GetByCpfAsync(CPF_VALIDO);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaComCpfVazio_ExceptionError()
        {
            // Arrange
            string cpfVazio = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.GetByCpfAsync(cpfVazio)
            );

            Assert.Equal("CPF é obrigatório.", exception.Message);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaComCpfInvalido_ExceptionError()
        {
            // Arrange
            string cpfInvalido = "123";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.GetByCpfAsync(cpfInvalido)
            );

            Assert.Equal("CPF inválido.", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaFisica_Ok()
        {
            // Arrange
            var pessoa = CriarPessoaFisicaMock();
            var novoNome = "Gustavo Moreira G. Silva";

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            // Act
            await _service.UpdateByCpfAsync(CPF_VALIDO, novoNome, null, null, null, null);

            // Assert
            _mockRepository.Verify(r => r.UpdatePessoaFisicaAsync(It.Is<PessoaFisica>(p =>
                p.Id == pessoa.Id &&
                p.Nome == novoNome &&
                p.CPF == CPF_VALIDO
            )), Times.Once);
        }

        [Fact]
        public async Task Test_AtualizarCepValido_Ok()
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
            // Arrange
            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync((PessoaFisica)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.UpdateByCpfAsync(CPF_VALIDO, "Novo Nome", null, null, null, null)
            );

            Assert.Equal("Pessoa física não encontrada.", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaComCpfVazio_ExceptionError()
        {
            // Arrange
            string cpfVazio = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateByCpfAsync(cpfVazio, "Novo Nome", null, null, null, null)
            );

            Assert.Equal("CPF é obrigatório para atualização.", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaComCpfInvalido_ExceptionError()
        {
            // Arrange
            string cpfInvalido = "123";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateByCpfAsync(cpfInvalido, "Novo Nome", null, null, null, null)
            );

            Assert.Equal("CPF inválido.", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaComNovoCpfDiferente_ExceptionError()
        {
            // Arrange
            var pessoa = CriarPessoaFisicaMock();
            var cpfDiferente = "529.982.247-25";

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.UpdateByCpfAsync(CPF_VALIDO, "Novo Nome", cpfDiferente, null, null, null)
            );

            Assert.Equal("Não é permitido alterar o CPF (documento).", exception.Message);
        }

        [Fact]
        public async Task Test_AtualizarPessoaComCepInvalido_ExceptionError()
        {
            // Arrange
            var pessoa = CriarPessoaFisicaMock();
            var cepInvalido = "99999999";

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            _mockViaCepService.Setup(s => s.ConsultarEnderecoPorCepAsync(cepInvalido))
                .ReturnsAsync((Endereco)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.UpdateByCpfAsync(CPF_VALIDO, null, null, cepInvalido, null, null)
            );

            Assert.Equal("Endereço não encontrado para o CEP informado.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfExistente_Ok()
        {
            // Arrange
            var pessoa = CriarPessoaFisicaMock();

            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync(pessoa);

            // Act
            await _service.DeleteByCpfAsync(CPF_VALIDO);

            // Assert
            _mockRepository.Verify(r => r.DeletePessoaFisicaAsync(pessoa.Id), Times.Once);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfInexistente_ExceptionError()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetPessoaFisicaByCpfAsync(CPF_VALIDO))
                .ReturnsAsync((PessoaFisica)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.DeleteByCpfAsync(CPF_VALIDO)
            );

            Assert.Equal("Pessoa física não encontrada.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfVazio_ExceptionError()
        {
            // Arrange
            string cpfVazio = "";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.DeleteByCpfAsync(cpfVazio)
            );

            Assert.Equal("CPF é obrigatório para exclusão.", exception.Message);
        }

        [Fact]
        public async Task Test_DeletarPessoaComCpfInvalido_ExceptionError()
        {
            // Arrange
            string cpfInvalido = "123";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(
                () => _service.DeleteByCpfAsync(cpfInvalido)
            );

            Assert.Equal("CPF inválido.", exception.Message);
        }
    }
}