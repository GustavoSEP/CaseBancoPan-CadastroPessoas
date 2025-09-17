using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de gerenciamento de pessoas físicas.
    /// Fornece operações CRUD para entidades do tipo PessoaFisica.
    /// </summary>
    public interface IPessoaFisicaService
    {
        /// <summary>
        /// Cria uma nova pessoa física no sistema.
        /// </summary>
        /// <param name="nome">Nome completo da pessoa física.</param>
        /// <param name="cpf">CPF da pessoa física (com ou sem formatação).</param>
        /// <param name="cep">CEP do endereço da pessoa física.</param>
        /// <param name="numero">Número do endereço da pessoa física.</param>
        /// <param name="complemento">Complemento do endereço da pessoa física (opcional).</param>
        /// <returns>A entidade PessoaFisica criada com seus dados completos.</returns>
        /// <exception cref="ArgumentException">Lançada quando algum dos parâmetros é inválido.</exception>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a criação.</exception>
        Task<PessoaFisica> CreateAsync(string nome, string cpf, string cep, string numero, string complemento);
        
        /// <summary>
        /// Lista todas as pessoas físicas cadastradas no sistema.
        /// </summary>
        /// <returns>Uma coleção contendo todas as pessoas físicas cadastradas.</returns>
        Task<IEnumerable<PessoaFisica>> ListAsync();
        
        /// <summary>
        /// Busca uma pessoa física pelo seu CPF.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser buscada (com ou sem formatação).</param>
        /// <returns>A entidade PessoaFisica correspondente ao CPF informado, ou null caso não seja encontrada.</returns>
        /// <exception cref="ArgumentException">Lançada quando o CPF fornecido é inválido.</exception>
        Task<PessoaFisica?> GetByCpfAsync(string cpf);

        /// <summary>
        /// Atualiza os dados de uma pessoa física existente.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser atualizada (com ou sem formatação).</param>
        /// <param name="nome">Novo nome da pessoa física (opcional).</param>
        /// <param name="cpfRaw">CPF da pessoa física a ser atualizada (não é possivel altera-lo) (opcional).</param>
        /// <param name="cep">Novo CEP do endereço da pessoa física (opcional).</param>
        /// <param name="numero">Novo número do endereço da pessoa física (opcional).</param>
        /// <param name="complemento">Novo complemento do endereço da pessoa física (opcional).</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="ArgumentException">Lançada quando algum dos parâmetros é inválido.</exception>
        /// <exception cref="InvalidOperationException">Lançada quando ocorre um erro durante a atualização.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa física não é encontrada.</exception>
        Task UpdateByCpfAsync(string cpf, string? nome, string? cpfRaw, string? cep, string? numero, string? complemento);
        
        /// <summary>
        /// Remove uma pessoa física do sistema.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser removida (com ou sem formatação).</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="ArgumentException">Lançada quando o CPF fornecido é inválido.</exception>
        /// <exception cref="KeyNotFoundException">Lançada quando a pessoa física não é encontrada.</exception>
        Task DeleteByCpfAsync(string cpf);
    }
}