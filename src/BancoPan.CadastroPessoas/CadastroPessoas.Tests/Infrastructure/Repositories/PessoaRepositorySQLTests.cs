using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Infrastructure.SQL.Data;
using CadastroPessoas.Infrastructure.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Tests.Infrastructure.Repositories
{
    public class PessoaRepositorySQLTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly PessoaRepositorySQL _repository;

        public PessoaRepositorySQLTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new PessoaRepositorySQL(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeletedAsync();
            _context.Dispose();
        }

        [Fact]
        public async Task Test_AdicionarPessoaFisicaRepository_Ok()
        {
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica("Gustavo M. Santana", "496.336.978-83", "F", endereco);

            var resultado = await _repository.AddPessoaFisicaAsync(pessoa);

            Assert.NotEqual(0, resultado.Id);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
            Assert.Equal("496.336.978-83", resultado.CPF);
            Assert.Equal("F", resultado.TipoPessoa);

            var pessoaNoDb = await _context.PessoasFisicas.FindAsync(resultado.Id);
            Assert.NotNull(pessoaNoDb);
            Assert.Equal("Gustavo M. Santana", pessoaNoDb.Nome);
        }

        [Fact]
        public async Task Test_RetornaListaDeTodasAsPessoasFisicas_Ok()
        {
            await Test_AdicionarPessoaFisica_Ok();

            var resultado = await _repository.ListPessoaFisicaAsync();

            Assert.Equal(3, resultado.Count());
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDoID_Ok()
        {
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica("Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var resultado = await _repository.GetPessoaFisicaByIdAsync(pessoa.Id);

            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDeIdInexistente_ExceptionError()
        {

            var resultado = await _repository.GetPessoaFisicaByIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDoCpf_Ok()
        {

            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica("Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var resultado = await _repository.GetPessoaFisicaByCpfAsync("496.336.978-83");

            Assert.NotNull(resultado);
            Assert.Equal("Gustavo M. Santana", resultado.Nome);
            Assert.Equal("496.336.978-83", resultado.CPF);
        }

        [Fact]
        public async Task Test_ObterPessoaFisicaAtravesDeCpfInexistente_ExceptionError()
        {
            var resultado = await _repository.GetPessoaFisicaByCpfAsync("111.222.333-44");

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaFisica_Ok()
        {
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica("Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var resultado = await _repository.ExistsPessoaFisicaByCpfAsync("496.336.978-83");

            Assert.True(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaFisicaComCPFInexistente_ExceptionError()
        {

            var resultado = await _repository.ExistsPessoaFisicaByCpfAsync("111.222.333-44");

            Assert.False(resultado);
        }

        [Fact]
        public async Task Test_AtualizarPessoaFisica_Ok()
        {
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica("Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            pessoa.AtualizarDados("Gustavo Moreira Gonçalves da Silva", pessoa.CPF, pessoa.TipoPessoa);

            await _repository.UpdatePessoaFisicaAsync(pessoa);

            var pessoaAtualizada = await _context.PessoasFisicas.FindAsync(pessoa.Id);
            Assert.NotNull(pessoaAtualizada);
            Assert.Equal("Gustavo Moreira Gonçalves da Silva", pessoaAtualizada.Nome);
        }

        [Fact]
        public async Task Test_ExcluirPessoaFisica_Ok()
        {

            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");
            var pessoa = new PessoaFisica("Gustavo M. Santana", "496.336.978-83", "F", endereco);
            await _context.PessoasFisicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var id = pessoa.Id;

            await _repository.DeletePessoaFisicaAsync(id);

            var pessoaExcluida = await _context.PessoasFisicas.FindAsync(id);
            Assert.Null(pessoaExcluida);
        }

        private async Task Test_AdicionarPessoaFisica_Ok()
        {
            var endereco = new Endereco("05723330", "Rua Afonso Vidal", "Vila Andrade", "São Paulo", "SP", "390", "AP 96B");

            var pessoas = new List<PessoaFisica>
            {
                new PessoaFisica("Gustavo M. Santana", "111.222.333-44", "F", endereco),
                new PessoaFisica("Laiane M. Santana", "222.333.444-55", "F", endereco),
                new PessoaFisica("Pedro M. Santana", "333.444.555-66", "F", endereco)
            };

            await _context.PessoasFisicas.AddRangeAsync(pessoas);
            await _context.SaveChangesAsync();
        }


    }
}
