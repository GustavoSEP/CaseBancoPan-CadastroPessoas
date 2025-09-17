using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Interfaces
{
    /// <summary>
    /// Interface para o repositório de pessoas físicas e jurídicas.
    /// Define operações de persistência para entidades do tipo PessoaFisica e PessoaJuridica.
    /// </summary>
    public interface IPessoaRepository
    {
        // PessoaFisica
        /// <summary>
        /// Adiciona uma nova pessoa física ao repositório.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaFisica a ser adicionada.</param>
        /// <returns>A entidade PessoaFisica adicionada, com seu Id gerado.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a persistência.</exception>
        Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa);

        /// <summary>
        /// Lista todas as pessoas físicas cadastradas no repositório.
        /// </summary>
        /// <returns>Uma coleção contendo todas as pessoas físicas cadastradas.</returns>
        Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync();

        /// <summary>
        /// Busca uma pessoa física pelo seu Id.
        /// </summary>
        /// <param name="id">O Id da pessoa física a ser buscada.</param>
        /// <returns>A entidade PessoaFisica correspondente ao Id informado, ou null caso não seja encontrada.</returns>
        Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id);

        /// <summary>
        /// Busca uma pessoa física pelo seu CPF.
        /// </summary>
        /// <param name="cpf">O CPF da pessoa física a ser buscada (formato: 000.000.000-00).</param>
        /// <returns>A entidade PessoaFisica correspondente ao CPF informado, ou null caso não seja encontrada.</returns>
        Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf);

        /// <summary>
        /// Verifica se existe uma pessoa física com o CPF informado.
        /// </summary>
        /// <param name="cpf">O CPF a ser verificado (formato: 000.000.000-00).</param>
        /// <returns>True se existir uma pessoa física com o CPF informado, caso contrário False.</returns>
        Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf);

        /// <summary>
        /// Atualiza os dados de uma pessoa física existente.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaFisica com os dados atualizados.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a atualização.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa física não é encontrada.</exception>
        Task UpdatePessoaFisicaAsync(PessoaFisica pessoa);

        /// <summary>
        /// Remove uma pessoa física do repositório.
        /// </summary>
        /// <param name="id">O Id da pessoa física a ser removida.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a remoção.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa física não é encontrada.</exception>
        Task DeletePessoaFisicaAsync(int id);

        // PessoaJuridica
        /// <summary>
        /// Adiciona uma nova pessoa jurídica ao repositório.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaJuridica a ser adicionada.</param>
        /// <returns>A entidade PessoaJuridica adicionada, com seu Id gerado.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a persistência.</exception>
        Task<PessoaJuridica> AddPessoaJuridicaAsync(PessoaJuridica pessoa);

        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas no repositório.
        /// </summary>
        /// <returns>Uma coleção contendo todas as pessoas jurídicas cadastradas.</returns>
        Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync();

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu Id.
        /// </summary>
        /// <param name="id">O Id da pessoa jurídica a ser buscada.</param>
        /// <returns>A entidade PessoaJuridica correspondente ao Id informado, ou null caso não seja encontrada.</returns>
        Task<PessoaJuridica?> GetPessoaJuridicaByIdAsync(int id);

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu CNPJ.
        /// </summary>
        /// <param name="cnpj">O CNPJ da pessoa jurídica a ser buscada (formato: 00.000.000/0000-00).</param>
        /// <returns>A entidade PessoaJuridica correspondente ao CNPJ informado, ou null caso não seja encontrada.</returns>
        Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj);

        /// <summary>
        /// Verifica se existe uma pessoa jurídica com o CNPJ informado.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser verificado (formato: 00.000.000/0000-00).</param>
        /// <returns>True se existir uma pessoa jurídica com o CNPJ informado, caso contrário False.</returns>
        Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj);

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica existente.
        /// </summary>
        /// <param name="pessoa">A entidade PessoaJuridica com os dados atualizados.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a atualização.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa jurídica não é encontrada.</exception>
        Task UpdatePessoaJuridicaAsync(PessoaJuridica pessoa);

        /// <summary>
        /// Remove uma pessoa jurídica do repositório.
        /// </summary>
        /// <param name="id">O Id da pessoa jurídica a ser removida.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a remoção.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa jurídica não é encontrada.</exception>
        Task DeletePessoaJuridicaAsync(int id);
    }
}