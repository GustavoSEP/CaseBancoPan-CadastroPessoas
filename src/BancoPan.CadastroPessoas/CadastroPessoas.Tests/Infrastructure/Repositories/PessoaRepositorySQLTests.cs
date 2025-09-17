using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Adapters.Output.SQL.Data;
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

        [Fact]
        public async Task Test_AdicionarPessoaJuridicaRepository_Ok()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);

            var resultado = await _repository.AddPessoaJuridicaAsync(pessoa);

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
            await Test_AdicionarPessoasJuridicasTeste();

            var resultado = await _repository.ListPessoaJuridicaAsync();

            Assert.Equal(3, resultado.Count());
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDoID_Ok()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var resultado = await _repository.GetPessoaJuridicaByIdAsync(pessoa.Id);

            Assert.NotNull(resultado);
            Assert.Equal(pessoa.Id, resultado.Id);
            Assert.Equal("BANCO PAN S/A.", resultado.RazaoSocial);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDeIdInexistente_ExceptionError()
        {
            var resultado = await _repository.GetPessoaJuridicaByIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDCnpj_Ok()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var resultado = await _repository.GetPessoaJuridicaByCnpjAsync("59.285.411/0001-13");

            Assert.NotNull(resultado);
            Assert.Equal("BANCO PAN S/A.", resultado.RazaoSocial);
            Assert.Equal("59.285.411/0001-13", resultado.CNPJ);
        }

        [Fact]
        public async Task Test_ObterPessoaJuridicaAtravesDeCnpjInexistente_ExceptionError()
        {
            var resultado = await _repository.GetPessoaJuridicaByCnpjAsync("11.111.111/1111-11");

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoJuridica_Ok()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var resultado = await _repository.ExistsPessoaJuridicaByCnpjAsync("59.285.411/0001-13");

            Assert.True(resultado);
        }

        [Fact]
        public async Task Test_ValidarSeExistePessoaJuridicaComCNPJInexistente_ExceptionError()
        {
            var resultado = await _repository.ExistsPessoaJuridicaByCnpjAsync("11.111.111/1111-11");

            Assert.False(resultado);
        }

        [Fact]
        public async Task Test_AtualizarPessoJuridica_Ok()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            pessoa.AtualizarDados("Banco Panamericano", "Panamericano", pessoa.CNPJ, pessoa.TipoPessoa);

            await _repository.UpdatePessoaJuridicaAsync(pessoa);

            var pessoaAtualizada = await _context.PessoasJuridicas.FindAsync(pessoa.Id);
            Assert.NotNull(pessoaAtualizada);
            Assert.Equal("Banco Panamericano", pessoaAtualizada.RazaoSocial);
            Assert.Equal("Panamericano", pessoaAtualizada.NomeFantasia);
        }

        [Fact]
        public async Task Test_ExcluirPessoaJuridica_Ok()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");
            var pessoa = new PessoaJuridica("BANCO PAN S/A.", "Banco Pan", "59.285.411/0001-13", "J", endereco);
            await _context.PessoasJuridicas.AddAsync(pessoa);
            await _context.SaveChangesAsync();

            var id = pessoa.Id;

            await _repository.DeletePessoaJuridicaAsync(id);

            var pessoaExcluida = await _context.PessoasJuridicas.FindAsync(id);
            Assert.Null(pessoaExcluida);
        }

        private async Task Test_AdicionarPessoasJuridicasTeste()
        {
            var endereco = new Endereco("01310100", "Av. Paulista", "Bela Vista", "São Paulo", "SP", "1374", "10 Andar");

            var pessoas = new List<PessoaJuridica>
            {
                new PessoaJuridica("BANCO PAN S/A", "Banco Pan", "59.285.411/0001-13", "J", endereco),
                new PessoaJuridica("BTG Pactual S/A", "BTG PACTUAL", "30.306.294/0001-45", "J", endereco),
                new PessoaJuridica("MOBIAUTO LTDA.", "MobiAuto", "32.158.029/0001-92", "J", endereco)
            };

            await _context.PessoasJuridicas.AddRangeAsync(pessoas);
            await _context.SaveChangesAsync();
        }
    }
}
