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
    /// <summary>
    /// Implementação do repositório de pessoas utilizando SQL Server e Entity Framework Core.
    /// </summary>
    /// <remarks>
    /// Este adaptador implementa a porta de saída <see cref="IPessoaRepository"/> seguindo
    /// os princípios da Arquitetura Hexagonal, isolando o acesso ao banco de dados do
    /// núcleo da aplicação. Utiliza o Entity Framework Core para persistência.
    /// </remarks>
    public class PessoaRepositorySqlAdapter : IPessoaRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PessoaRepositorySqlAdapter> _logger;

        /// <summary>
        /// Inicializa uma nova instância do adaptador de repositório SQL.
        /// </summary>
        /// <param name="context">Contexto do Entity Framework para acesso ao banco de dados.</param>
        /// <param name="logger">Serviço de log para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public PessoaRepositorySqlAdapter(AppDbContext context, ILogger<PessoaRepositorySqlAdapter> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Adiciona uma nova pessoa física no banco de dados.
        /// </summary>
        /// <param name="pessoa">Entidade de pessoa física a ser persistida.</param>
        /// <returns>A entidade persistida com seu ID gerado.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Busca uma pessoa física pelo seu ID.
        /// </summary>
        /// <param name="id">ID da pessoa física a ser buscada.</param>
        /// <returns>A entidade encontrada ou null se não existir.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Busca uma pessoa física pelo seu CPF.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser buscada.</param>
        /// <returns>A entidade encontrada ou null se não existir.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Recupera todas as pessoas físicas cadastradas.
        /// </summary>
        /// <returns>Coleção com todas as pessoas físicas, incluindo seus endereços.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Atualiza os dados de uma pessoa física existente.
        /// </summary>
        /// <param name="pessoa">Entidade com os dados atualizados.</param>
        /// <returns>A entidade atualizada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
        public async Task<PessoaFisica> UpdatePessoaFisicaAsync(PessoaFisica pessoa)
        {
            try
            {
                // Buscar a entidade atual para remover do contexto
                var existingEntity = await _context.PessoasFisicas
                    .Include(p => p.Endereco)
                    .FirstOrDefaultAsync(p => p.Id == pessoa.Id);

                if (existingEntity == null)
                {
                    throw new Exception($"Pessoa Física com ID {pessoa.Id} não encontrada para atualização.");
                }

                // Remover a entidade existente do contexto
                _context.PessoasFisicas.Remove(existingEntity);
                
                // Adicionar a nova entidade (com as alterações)
                _context.PessoasFisicas.Add(pessoa);
                
                // Salvar as alterações
                await _context.SaveChangesAsync();
                
                return pessoa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Física no banco de dados. ID: {Id}", pessoa.Id);
                throw new Exception("Erro ao atualizar Pessoa Física: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove uma pessoa física do banco de dados.
        /// </summary>
        /// <param name="id">ID da pessoa física a ser removida.</param>
        /// <returns>True se a remoção foi bem-sucedida, False se a pessoa não foi encontrada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Verifica se existe uma pessoa física com o CPF informado.
        /// </summary>
        /// <param name="cpf">CPF a ser verificado.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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
        
        /// <summary>
        /// Adiciona uma nova pessoa jurídica no banco de dados.
        /// </summary>
        /// <param name="pessoa">Entidade de pessoa jurídica a ser persistida.</param>
        /// <returns>A entidade persistida com seu ID gerado.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu ID.
        /// </summary>
        /// <param name="id">ID da pessoa jurídica a ser buscada.</param>
        /// <returns>A entidade encontrada ou null se não existir.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser buscada.</param>
        /// <returns>A entidade encontrada ou null se não existir.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Recupera todas as pessoas jurídicas cadastradas.
        /// </summary>
        /// <returns>Coleção com todas as pessoas jurídicas, incluindo seus endereços.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica existente.
        /// </summary>
        /// <param name="pessoa">Entidade com os dados atualizados.</param>
        /// <returns>A entidade atualizada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
        public async Task<PessoaJuridica> UpdatePessoaJuridicaAsync(PessoaJuridica pessoa)
        {
            try
            {
                // Buscar a entidade atual para remover do contexto
                var existingEntity = await _context.PessoasJuridicas
                    .Include(p => p.Endereco)
                    .FirstOrDefaultAsync(p => p.Id == pessoa.Id);

                if (existingEntity == null)
                {
                    throw new Exception($"Pessoa Jurídica com ID {pessoa.Id} não encontrada para atualização.");
                }

                // Remover a entidade existente do contexto
                _context.PessoasJuridicas.Remove(existingEntity);
                
                // Adicionar a nova entidade (com as alterações)
                _context.PessoasJuridicas.Add(pessoa);
                
                // Salvar as alterações
                await _context.SaveChangesAsync();
                
                return pessoa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Jurídica no banco de dados. ID: {Id}", pessoa.Id);
                throw new Exception("Erro ao atualizar Pessoa Jurídica: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove uma pessoa jurídica do banco de dados.
        /// </summary>
        /// <param name="id">ID da pessoa jurídica a ser removida.</param>
        /// <returns>True se a remoção foi bem-sucedida, False se a pessoa não foi encontrada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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

        /// <summary>
        /// Verifica se existe uma pessoa jurídica com o CNPJ informado.
        /// </summary>
        /// <param name="cnpj">CNPJ a ser verificado.</param>
        /// <returns>True se existir, False caso contrário.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
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
        
        /// <summary>
        /// Lista todas as pessoas físicas cadastradas.
        /// </summary>
        /// <returns>Coleção com todas as pessoas físicas, incluindo seus endereços.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
        /// <remarks>
        /// Este método é similar ao GetAllPessoasFisicasAsync, mas mantido para compatibilidade com a interface.
        /// </remarks>
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
        
        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas.
        /// </summary>
        /// <returns>Coleção com todas as pessoas jurídicas, incluindo seus endereços.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no acesso ao banco de dados.</exception>
        /// <remarks>
        /// Este método é similar ao GetAllPessoasJuridicasAsync, mas mantido para compatibilidade com a interface.
        /// </remarks>
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