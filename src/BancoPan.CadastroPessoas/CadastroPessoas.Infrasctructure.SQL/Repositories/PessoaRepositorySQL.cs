using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Infrastructure.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Infrastructure.SQL.Repositories
{
    public class PessoaRepositorySQL : IPessoaRepository
    {
        private readonly AppDbContext _context;

        public PessoaRepositorySQL(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
                throw new Exception("Erro ao inserir Pessoa Física no banco de dados: " + ex.Message, ex);
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
                throw new Exception("Erro ao listar Pessoas Físicas: " + ex.Message, ex);
            }
        }

        public async Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id)
        {
            try
            {
                return await _context.PessoasFisicas.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar Pessoa Física por Id: " + ex.Message, ex);
            }
        }

        public async Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf)
        {
            try
            {
                return await _context.PessoasFisicas.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CPF == cpf);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar Pessoa Física por CPF: " + ex.Message, ex);
            }
        }

        public async Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf)
        {
            try
            {
                return await _context.PessoasFisicas.AsNoTracking()
                    .AnyAsync(p => p.CPF == cpf);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao verificar existência de CPF: " + ex.Message, ex);
            }
        }

        public async Task UpdatePessoaFisicaAsync(PessoaFisica pessoa)
        {
            try
            {
                _context.PessoasFisicas.Update(pessoa);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar Pessoa Física: " + ex.Message, ex);
            }
        }

        public async Task DeletePessoaFisicaAsync(int id)
        {
            try
            {
                var p = await _context.PessoasFisicas.FindAsync(id);
                if (p != null)
                {
                    _context.PessoasFisicas.Remove(p);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar Pessoa Física: " + ex.Message, ex);
            }
        }
        
        // PessoaJuridica

        public async Task<PessoaJuridica> AddPessoaJuridicaAsync(PessoaJuridica pessoa)
        {
            try
            {
                var entry = await _context.PessoasJuridicas.AddAsync(pessoa);
                await _context.SaveChangesAsync();
                return entry.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inserir Pessoa Jurídica no banco de dados: {ex.Message} ");
            }
        }
        public async Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync()
        {
            try
            {
                return await _context.PessoasJuridicas.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar Pessoas Jurídicas: {ex.Message}");
            }
        }
        public async Task<PessoaJuridica?> GetPessoaJuridicaByIdAsync(int id)
        {
            try
            {
                return await _context.PessoasJuridicas.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar Pessoa Jurídica por Id: " + ex.Message, ex);
            }
        }
        public async Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj)
        {
            try
            {
                return await _context.PessoasJuridicas.AsNoTracking().FirstOrDefaultAsync(p => p.CNPJ == cnpj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar Pessoa Jurídica por CNPJ: {ex.Message}");
            }
        }
        public async Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj)
        {
            try
            {
                return await _context.PessoasJuridicas.AsNoTracking().AnyAsync(p => p.CNPJ == cnpj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência de CNPJ: {ex.Message}");
            }
        }
        public async Task DeletePessoaJuridicaAsync(int id)
        {
            try
            {
                var pessoa = await _context.PessoasJuridicas.FindAsync(id);
                if (pessoa != null)
                {
                    _context.PessoasJuridicas.Remove(pessoa);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar Pessoa Jurídica: {ex.Message}");
            }
        }
    }
}
