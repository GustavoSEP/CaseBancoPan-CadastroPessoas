using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Infrasctructure.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Infrasctructure.SQL.Repositories
{
    public class PessoaRepositorySQL : IPessoaRepository
    {
        private readonly AppDbContext _context; // Utilizado para se conectar ao SQL (instalei o SQL Express e gerei um banco de dados local.
                                                // ConnectionString vai ficar no appssetings.json)

        public PessoaRepositorySQL(AppDbContext context)
        {
            _context = context ?? throw new AbandonedMutexException(nameof(context));
        }

        public async Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa)
        {
            try
            {
                var entry = await _context.PessoasFisicas.AddAsync(pessoa);
                await _context.SaveChangesAsync();
                return entry.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar Pessoa Física: {ex.Message}");
            }
        }

        public async Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync()
        {
            try
            {
                return await _context.PessoasFisicas.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar Pessoas Fisicas: {ex.Message} - {ex}");
            }
        }

        public async Task<PessoaFisica> GetPessoaFisicaByIdAsync(int id)
        {
            try
            {
                return await _context.PessoasFisicas.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar Pessoa Física por ID. Erro: {ex.Message} - {ex} ");
            }
        }

        public async Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf)
        {
            try
            {
                return await _context.PessoasFisicas.AsNoTracking()
                                .FirstOrDefaultAsync(pessoa => pessoa.CPF == cpf);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar Pessoa Fisica por Documento. Erro:  {ex.Message} - {ex} ");
            }
        }

        public async Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf)
        {
            try
            {
                return await _context.PessoasFisicas.AsNoTracking()
                                .AnyAsync(pessoa => pessoa.CPF == cpf);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência de Pessoa Física pelo CPF: {cpf}. Erro: {ex.Message} - {ex} ");
            }
        }
        public async Task UpdatePessoaFisicaAsync(PessoaFisica pessoa)
        {
            try
            {
                _context.PessoasFisicas.Update(pessoa);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception($"Erro ao atualizar Pessoa Física com ID: {pessoa.Id}");
            }
        }

        public async Task DeletePessoaFisicaAsync(int id)
        {
            try
            {
                var pessoa = await _context.PessoasFisicas.FindAsync(id);
                if (pessoa != null)
                {
                    _context.PessoasFisicas.Remove(pessoa);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Pessoa Física com ID: {id} não encontrada para exclusão.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar Pessoa Física com ID: {id}. Erro: {ex.Message} - {ex} ");
            }
        }
    }
}
