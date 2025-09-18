using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Adapters.Output.SQL.Data;
using CadastroPessoas.Adapters.Output.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CadastroPessoas.Tests.Infrastructure.Repositories
{
    public class PessoaRepositoryAdapterTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PessoaRepositorySqlAdapter _repository;
        private readonly Mock<ILogger<PessoaRepositorySqlAdapter>> _loggerMock;

        public PessoaRepositoryAdapterTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<PessoaRepositorySqlAdapter>>();
            _repository = new PessoaRepositorySqlAdapter(_context, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task Test_AdicionarPessoaFisicaRepository_Ok()
        {
            // Arrange
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica(0, "Gustavo M. Santana", "496.336.978-83", "F", endereco);

            // Act
            var resultado = await _repository.AddPessoaFisicaAsync(pessoa);

            // Assert
            Assert.NotEqual(0, resultado.Id);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
            Assert.Equal("496.336.978-83", resultado.CPF);
            Assert.Equal("F", resultado.Tipo);

            var pessoaNoDb = await _context.PessoasFisicas.FindAsync(resultado.Id);
            Assert.NotNull(pessoaNoDb);
            Assert.Equal("Gustavo M. Santana", pessoaNoDb.Nome);
        }

        [Fact]
        public async Task Test_RetornaListaDeTodasAsPessoasFisicas_Ok()
        {
            // Arrange
            await Test_AdicionarPessoaFisica_Ok();

            // Act
            var resultado = await _repository.ListPessoaFisicaAsync();

            // Assert
            Assert.Equal(3, resultado.Count());
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDoID_Ok()
        {
            // Arrange
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica(0, "Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.GetPessoaFisicaByIdAsync(pessoa.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDeIdInexistente_RetornaNulo()
        {
            // Act
            var resultado = await _repository.GetPessoaFisicaByIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDoCpf_Ok()
        {
            // Arrange
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica(0, "Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.GetPessoaFisicaByCpfAsync("496.336.978-83");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
            Assert.Equal("496.336.978-83", resultado.CPF);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDeCpfInexistente_RetornaNulo()
        {
            // Act
            var resultado = await _repository.GetPessoaFisicaByCpfAsync("111.222.333-44");

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaFisica_Ok()
        {
            // Arrange
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica(0, "Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.ExistsPessoaFisicaByCpfAsync("496.336.978-83");

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaFisicaComCPFInexistente_RetornaFalso()
        {
            // Act
            var resultado = await _repository.ExistsPessoaFisicaByCpfAsync("111.222.333-44");

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task Test_AtualizarPessoaFisica_Ok()
        {
            // Arrange
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica(0, "Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var novoEndereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoaAtualizada = new PessoaFisica(pessoa.Id, "Gustavo Moreira Gonçalves da Silva", "496.336.978-83", "F", novoEndereco);

            // Act
            await _repository.UpdatePessoaFisicaAsync(pessoaAtualizada);

            // Assert
            var pessoaNoDb = await _context.PessoasFisicas.FindAsync(pessoa.Id);
            Assert.NotNull(pessoaNoDb);
            Assert.Equal("Gustavo Moreira Gonçalves da Silva", pessoaNoDb.Nome);
        }

        [Fact]
        public async Task Test_ExcluirPessoaFisica_Ok()
        {
            // Arrange
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica(0, "Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var id = pessoa.Id;

            // Act
            var resultado = await _repository.DeletePessoaFisicaAsync(id);

            // Assert
            Assert.True(resultado);
            var pessoaExcluida = await _context.PessoasFisicas.FindAsync(id);
            Assert.Null(pessoaExcluida);
        }

        [Fact]
        public async Task Test_ObterTodasPessoasFisicas_Ok()
        {
            // Arrange
            await Test_AdicionarPessoaFisica_Ok();

            // Act
            var resultado = await _repository.GetAllPessoasFisicasAsync();

            // Assert
            Assert.Equal(3, resultado.Count());
        }

        private async Task Test_AdicionarPessoaFisica_Ok()
        {
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");

            var pessoas = new List<PessoaFisica>
            {
                new PessoaFisica(0, "Gustavo M. Santana", "111.222.333-44", "F", endereco),
                new PessoaFisica(0, "Laiane M. Santana", "222.333.444-55", "F", endereco),
                new PessoaFisica(0, "Pedro M. Santana", "333.444.555-66", "F", endereco)
            };

            await _context.PessoasFisicas.AddRangeAsync(pessoas);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task Test_AdicionarPessoaJuridicaRepository_Ok()
        {
            // Arrange
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica(0, "BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);

            // Act
            var resultado = await _repository.AddPessoaJuridicaAsync(pessoa);

            // Assert
            Assert.NotEqual(0, resultado.Id);
            Assert.Equal("BANCO PAN S/A.", resultado.RazaoSocial);
            Assert.Equal("Banco Pan", resultado.NomeFantasia);
            Assert.Equal("59.285.411/0001-13", resultado.CNPJ);

            var pessoaNoDb = await _context.PessoasJuridicas.FindAsync(resultado.Id);
            Assert.NotNull(pessoaNoDb);
            Assert.Equal("BANCO PAN S/A.", pessoaNoDb.RazaoSocial);
        }

        [Fact]
        public async Task Test_RetornaListaDeTodasAsPessoasJuridicas_Ok()
        {
            // Arrange
            await Test_AdicionarPessoasJuridicasTeste();

            // Act
            var resultado = await _repository.ListPessoaJuridicaAsync();

            // Assert
            Assert.Equal(3, resultado.Count());
        }

        [Fact]
        public async Task Test_ObterTodasPessoasJuridicas_Ok()
        {
            // Arrange
            await Test_AdicionarPessoasJuridicasTeste();

            // Act
            var resultado = await _repository.GetAllPessoasJuridicasAsync();

            // Assert
            Assert.Equal(3, resultado.Count());
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDoID_Ok()
        {
            // Arrange
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica(0, "BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.GetPessoaJuridicaByIdAsync(pessoa.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal("BANCO PAN S/A.", resultado.RazaoSocial);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDeIdInexistente_RetornaNulo()
        {
            // Act
            var resultado = await _repository.GetPessoaJuridicaByIdAsync(999);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDoCnpj_Ok()
        {
            // Arrange
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica(0, "BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.GetPessoaJuridicaByCnpjAsync("59.285.411/0001-13");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("BANCO PAN S/A.", resultado.RazaoSocial);
            Assert.Equal("59.285.411/0001-13", resultado.CNPJ);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDeCnpjInexistente_RetornaNulo()
        {
            // Act
            var resultado = await _repository.GetPessoaJuridicaByCnpjAsync("11.111.111/1111-11");

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaJuridica_Ok()
        {
            // Arrange
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica(0, "BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.ExistsPessoaJuridicaByCnpjAsync("59.285.411/0001-13");

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaJuridicaComCNPJInexistente_RetornaFalso()
        {
            // Act
            var resultado = await _repository.ExistsPessoaJuridicaByCnpjAsync("11.111.111/1111-11");

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task Test_AtualizarPessoaJuridica_Ok()
        {
            // Arrange
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica(0, "BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var novoEndereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoaAtualizada = new PessoaJuridica(pessoa.Id, "Banco Panamericano", "Panamericano", "59.285.411/0001-13", "J", novoEndereco);

            // Act
            await _repository.UpdatePessoaJuridicaAsync(pessoaAtualizada);

            // Assert
            var pessoaNoDb = await _context.PessoasJuridicas.FindAsync(pessoa.Id);
            Assert.NotNull(pessoaNoDb);
            Assert.Equal("Banco Panamericano", pessoaNoDb.RazaoSocial);
            Assert.Equal("Panamericano", pessoaNoDb.NomeFantasia);
        }

        [Fact]
        public async Task Test_ExcluirPessoaJuridica_Ok()
        {
            // Arrange
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica(0, "BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var id = pessoa.Id;

            // Act
            var resultado = await _repository.DeletePessoaJuridicaAsync(id);

            // Assert
            Assert.True(resultado);
            var pessoaExcluida = await _context.PessoasJuridicas.FindAsync(id);
            Assert.Null(pessoaExcluida);
        }

        private async Task Test_AdicionarPessoasJuridicasTeste()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");

            var pessoas = new List<PessoaJuridica>
            {
                new PessoaJuridica(0, "BANCO PAN S/A", "Banco Pan", "59.285.411/0001-13", "J", endereco),
                new PessoaJuridica(0, "BTG Pactual S/A", "BTG PACTUAL", "30.306.294/0001-45", "J", endereco),
                new PessoaJuridica(0, "MOBIAUTO LTDA.", "MobiAuto", "32.158.029/0001-92", "J", endereco)
            };

            await _context.PessoasJuridicas.AddRangeAsync(pessoas);
            await _context.SaveChangesAsync();
        }
    }
}