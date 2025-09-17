using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Adapters.Output.SQL.Data;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Adapters.Output.SQL.Repositories
{
    public class PessoaRepositorySqlAdapter : IPessoaRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PessoaRepositorySqlAdapter> _logger;

        public PessoaRepositorySqlAdapter(AppDbContext context, ILogger<PessoaRepositorySqlAdapter> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogError(ex, "Erro ao inserir Pessoa Física no banco de dados. CPF: {Cpf}", pessoa.CPF);
                throw new Exception("Erro ao inserir Pessoa Física no banco de dados: " + ex.Message, ex);
            }
        }

        public async Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id)
        {
            try
            {
                return await _context.PessoasFisicas
                    .Include(p => p.Endereco)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pessoa Física por ID no banco de dados. ID: {Id}", id);
                throw new Exception("Erro ao buscar Pessoa Física por ID: " + ex.Message, ex);
            }
        }

        public async Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf)
        {
            try
            {
                return await _context.PessoasFisicas
                    .Include(p => p.Endereco)
                    .FirstOrDefaultAsync(p => p.CPF == cpf);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pessoa Física por CPF no banco de dados. CPF: {Cpf}", cpf);
                throw new Exception("Erro ao buscar Pessoa Física por CPF: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<PessoaFisica>> GetAllPessoasFisicasAsync()
        {
            try
            {
                return await _context.PessoasFisicas
                    .Include(p => p.Endereco)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as Pessoas Físicas no banco de dados");
                throw new Exception("Erro ao buscar todas as Pessoas Físicas: " + ex.Message, ex);
            }
        }

        public async Task<PessoaFisica> UpdatePessoaFisicaAsync(PessoaFisica pessoa)
        {
            try
            {
                _context.PessoasFisicas.Update(pessoa);
                await _context.SaveChangesAsync();
                return pessoa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Física no banco de dados. ID: {Id}", pessoa.Id);
                throw new Exception("Erro ao atualizar Pessoa Física: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeletePessoaFisicaAsync(int id)
        {
            try
            {
                var pessoa = await _context.PessoasFisicas.FindAsync(id);
                if (pessoa == null)
                    return false;

                _context.PessoasFisicas.Remove(pessoa);
                var affectedRows = await _context.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Pessoa Física no banco de dados. ID: {Id}", id);
                throw new Exception("Erro ao excluir Pessoa Física: " + ex.Message, ex);
            }
        }

        public async Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf)
        {
            try
            {
                return await _context.PessoasFisicas.AnyAsync(p => p.CPF == cpf);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de Pessoa Física por CPF. CPF: {Cpf}", cpf);
                throw new Exception("Erro ao verificar existência de Pessoa Física por CPF: " + ex.Message, ex);
            }
        }

        // Implementações para PessoaJuridica
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
                _logger.LogError(ex, "Erro ao inserir Pessoa Jurídica no banco de dados. CNPJ: {Cnpj}", pessoa.CNPJ);
                throw new Exception("Erro ao inserir Pessoa Jurídica no banco de dados: " + ex.Message, ex);
            }
        }

        public async Task<PessoaJuridica?> GetPessoaJuridicaByIdAsync(int id)
        {
            try
            {
                return await _context.PessoasJuridicas
                    .Include(p => p.Endereco)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pessoa Jurídica por ID no banco de dados. ID: {Id}", id);
                throw new Exception("Erro ao buscar Pessoa Jurídica por ID: " + ex.Message, ex);
            }
        }

        public async Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj)
        {
            try
            {
                return await _context.PessoasJuridicas
                    .Include(p => p.Endereco)
                    .FirstOrDefaultAsync(p => p.CNPJ == cnpj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pessoa Jurídica por CNPJ no banco de dados. CNPJ: {Cnpj}", cnpj);
                throw new Exception("Erro ao buscar Pessoa Jurídica por CNPJ: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<PessoaJuridica>> GetAllPessoasJuridicasAsync()
        {
            try
            {
                return await _context.PessoasJuridicas
                    .Include(p => p.Endereco)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as Pessoas Jurídicas no banco de dados");
                throw new Exception("Erro ao buscar todas as Pessoas Jurídicas: " + ex.Message, ex);
            }
        }

        public async Task<PessoaJuridica> UpdatePessoaJuridicaAsync(PessoaJuridica pessoa)
        {
            try
            {
                _context.PessoasJuridicas.Update(pessoa);
                await _context.SaveChangesAsync();
                return pessoa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Jurídica no banco de dados. ID: {Id}", pessoa.Id);
                throw new Exception("Erro ao atualizar Pessoa Jurídica: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeletePessoaJuridicaAsync(int id)
        {
            try
            {
                var pessoa = await _context.PessoasJuridicas.FindAsync(id);
                if (pessoa == null)
                    return false;

                _context.PessoasJuridicas.Remove(pessoa);
                var affectedRows = await _context.SaveChangesAsync();
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Pessoa Jurídica no banco de dados. ID: {Id}", id);
                throw new Exception("Erro ao excluir Pessoa Jurídica: " + ex.Message, ex);
            }
        }

        public async Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj)
        {
            try
            {
                return await _context.PessoasJuridicas.AnyAsync(p => p.CNPJ == cnpj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de Pessoa Jurídica por CNPJ. CNPJ: {Cnpj}", cnpj);
                throw new Exception("Erro ao verificar existência de Pessoa Jurídica por CNPJ: " + ex.Message, ex);
            }
        }
        public async Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync()
        {
            try
            {
                return await _context.PessoasFisicas
                    .Include(p => p.Endereco)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar Pessoas Físicas no banco de dados");
                throw new Exception("Erro ao listar Pessoas Físicas: " + ex.Message, ex);
            }
        }
        public async Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync()
        {
            try
            {
                return await _context.PessoasJuridicas
                    .Include(p => p.Endereco)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar Pessoas Jurídicas no banco de dados");
                throw new Exception("Erro ao listar Pessoas Jurídicas: " + ex.Message, ex);
            }
        }
    }
}