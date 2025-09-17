using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de gerenciamento de pessoas jurídicas.
    /// Fornece operações CRUD para entidades do tipo PessoaJuridica.
    /// </summary>
    public interface IPessoaJuridicaService
    {
        /// <summary>
        /// Cria uma nova pessoa jurídica no sistema.
        /// </summary>
        /// <param name="razaoSocial">Razão social da empresa.</param>
        /// <param name="nomeFantasia">Nome fantasia da empresa.</param>
        /// <param name="cnpj">CNPJ da empresa (com ou sem formatação).</param>
        /// <param name="cep">CEP do endereço da empresa.</param>
        /// <param name="numero">Número do endereço da empresa.</param>
        /// <param name="complemento">Complemento do endereço da empresa (opcional).</param>
        /// <returns>A entidade PessoaJuridica criada com seus dados completos.</returns>
        /// <exception cref="ArgumentException">Lançada quando algum dos parâmetros é inválido.</exception>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a criação.</exception>
        Task<PessoaJuridica> CreateAsync(string razaoSocial, string nomeFantasia, string cnpj, string cep, string numero, string complemento);
        
        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas no sistema.
        /// </summary>
        /// <returns>Uma coleção contendo todas as pessoas jurídicas cadastradas.</returns>
        Task<IEnumerable<PessoaJuridica>> ListAsync();
        
        /// <summary>
        /// Busca uma pessoa jurídica pelo seu CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser buscada (com ou sem formatação).</param>
        /// <returns>A entidade PessoaJuridica correspondente ao CNPJ informado, ou null caso não seja encontrada.</returns>
        /// <exception cref="ArgumentException">Lançada quando o CNPJ fornecido é inválido.</exception>
        Task<PessoaJuridica?> GetByCnpjAsync(string cnpj);

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica existente.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser atualizada (com ou sem formatação).</param>
        /// <param name="razaoSocial">Nova razão social da empresa (opcional).</param>
        /// <param name="nomeFantasia">Novo nome fantasia da empresa (opcional).</param>
        /// <param name="cnpjRaw">Novo CNPJ da empresa (não é possivel altera-lo) (opcional).</param>
        /// <param name="cep">Novo CEP do endereço da empresa (opcional).</param>
        /// <param name="numero">Novo número do endereço da empresa (opcional).</param>
        /// <param name="complemento">Novo complemento do endereço da empresa (opcional).</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="ArgumentException">Lançada quando algum dos parâmetros é inválido.</exception>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a atualização.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa jurídica não é encontrada.</exception>
        Task UpdateByCnpjAsync(string cnpj, string? razaoSocial, string? nomeFantasia, string? cnpjRaw, string? cep, string? numero, string? complemento);
        
        /// <summary>
        /// Remove uma pessoa jurídica do sistema.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser removida (com ou sem formatação).</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="ArgumentException">Lançada quando o CNPJ fornecido é inválido.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa jurídica não é encontrada.</exception>
        Task DeleteByCnpjAsync(string cnpj);
    }
}