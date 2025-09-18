using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Infrastructure.SQL.Data;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Infrastructure.SQL.Repositories
{
    /// <summary>
    /// Implementação do repositório de pessoas utilizando SQL Server com Entity Framework Core.
    /// Gerencia operações de persistência para entidades PessoaFisica e PessoaJuridica.
    /// </summary>
    public class PessoaRepositorySQL : IPessoaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaRepositorySQL"/>.
        /// </summary>
        /// <param name="context">O contexto do Entity Framework Core para acesso ao banco de dados.</param>
        /// <exception cref="ArgumentNullException">Lançada quando o contexto fornecido é nulo.</exception>
        public PessoaRepositorySQL(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region PessoaFisica

        /// <summary>
        /// Adiciona uma nova pessoa física ao banco de dados.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaFisica a ser adicionada.</param>
        /// <returns>A entidade PessoaFisica adicionada, com seu Id gerado pelo banco de dados.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no processo de inserção no banco de dados.</exception>
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

        /// <summary>
        /// Lista todas as pessoas físicas cadastradas no banco de dados.
        /// </summary>
        /// <returns>Uma coleção contendo todas as pessoas físicas cadastradas.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
        /// <remarks>
        /// Utiliza AsNoTracking() para melhorar a performance em operações somente leitura.
        /// </remarks>
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

        /// <summary>
        /// Busca uma pessoa física pelo seu Id.
        /// </summary>
        /// <param name="id">O Id da pessoa física a ser buscada.</param>
        /// <returns>A entidade PessoaFisica correspondente ao Id informado, ou null caso não seja encontrada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
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

        /// <summary>
        /// Busca uma pessoa física pelo seu CPF.
        /// </summary>
        /// <param name="cpf">O CPF da pessoa física a ser buscada (formato: 000.000.000-00).</param>
        /// <returns>A entidade PessoaFisica correspondente ao CPF informado, ou null caso não seja encontrada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
        /// <remarks>
        /// Utiliza AsNoTracking() para melhorar a performance em operações somente leitura.
        /// </remarks>
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

        /// <summary>
        /// Verifica se existe uma pessoa física com o CPF informado.
        /// </summary>
        /// <param name="cpf">O CPF a ser verificado (formato: 000.000.000-00).</param>
        /// <returns>True se existir uma pessoa física com o CPF informado, caso contrário False.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
        /// <remarks>
        /// Utiliza AsNoTracking() para melhorar a performance em operações somente leitura.
        /// </remarks>
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

        /// <summary>
        /// Atualiza os dados de uma pessoa física existente.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaFisica com os dados atualizados.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar o registro no banco de dados.</exception>
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

        /// <summary>
        /// Remove uma pessoa física do banco de dados.
        /// </summary>
        /// <param name="id">O Id da pessoa física a ser removida.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao remover o registro do banco de dados.</exception>
        /// <remarks>
        /// Se a pessoa física não for encontrada, nenhuma ação será tomada.
        /// </remarks>
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

        #endregion

        #region PessoaJuridica

        /// <summary>
        /// Adiciona uma nova pessoa jurídica ao banco de dados.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaJuridica a ser adicionada.</param>
        /// <returns>A entidade PessoaJuridica adicionada, com seu Id gerado pelo banco de dados.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro no processo de inserção no banco de dados.</exception>
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
                throw new Exception("Erro ao inserir Pessoa Jurídica no banco de dados: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas no banco de dados.
        /// </summary>
        /// <returns>Uma coleção contendo todas as pessoas jurídicas cadastradas.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
        /// <remarks>
        /// Utiliza AsNoTracking() para melhorar a performance em operações somente leitura.
        /// </remarks>
        public async Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync()
        {
            try
            {
                return await _context.PessoasJuridicas.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao listar Pessoas Jurídicas: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu Id.
        /// </summary>
        /// <param name="id">O Id da pessoa jurídica a ser buscada.</param>
        /// <returns>A entidade PessoaJuridica correspondente ao Id informado, ou null caso não seja encontrada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
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

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu CNPJ.
        /// </summary>
        /// <param name="cnpj">O CNPJ da pessoa jurídica a ser buscada (formato: 00.000.000/0000-00).</param>
        /// <returns>A entidade PessoaJuridica correspondente ao CNPJ informado, ou null caso não seja encontrada.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
        /// <remarks>
        /// Utiliza AsNoTracking() para melhorar a performance em operações somente leitura.
        /// </remarks>
        public async Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj)
        {
            try
            {
                return await _context.PessoasJuridicas.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CNPJ == cnpj);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar Pessoa Jurídica por CNPJ: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Verifica se existe uma pessoa jurídica com o CNPJ informado.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser verificado (formato: 00.000.000/0000-00).</param>
        /// <returns>True se existir uma pessoa jurídica com o CNPJ informado, caso contrário False.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao consultar o banco de dados.</exception>
        /// <remarks>
        /// Utiliza AsNoTracking() para melhorar a performance em operações somente leitura.
        /// </remarks>
        public async Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj)
        {
            try
            {
                return await _context.PessoasJuridicas.AsNoTracking()
                    .AnyAsync(p => p.CNPJ == cnpj);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao verificar existência de CNPJ: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica existente.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaJuridica com os dados atualizados.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao atualizar o registro no banco de dados.</exception>
        public async Task UpdatePessoaJuridicaAsync(PessoaJuridica pessoa)
        {
            try
            {
                _context.PessoasJuridicas.Update(pessoa);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar Pessoa Jurídica: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Remove uma pessoa jurídica do banco de dados.
        /// </summary>
        /// <param name="id">O Id da pessoa jurídica a ser removida.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao remover o registro do banco de dados.</exception>
        /// <remarks>
        /// Se a pessoa jurídica não for encontrada, nenhuma ação será tomada.
        /// </remarks>
        public async Task DeletePessoaJuridicaAsync(int id)
        {
            try
            {
                var p = await _context.PessoasJuridicas.FindAsync(id);
                if (p != null)
                {
                    _context.PessoasJuridicas.Remove(p);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar Pessoa Jurídica: " + ex.Message, ex);
            }
        }

        #endregion
    }
}