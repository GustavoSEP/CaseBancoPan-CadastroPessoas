using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Ports.Output.Repositories
{
    /// <summary>
    /// Interface que define as operações de persistência para entidades de pessoa física e jurídica.
    /// </summary>
    /// <remarks>
    /// Esta interface segue os princípios da arquitetura hexagonal, atuando como uma porta de saída (output port)
    /// que permite ao domínio interagir com a infraestrutura de persistência sem depender de sua implementação.
    /// 
    /// Fornece métodos para operações CRUD (Create, Read, Update, Delete) tanto para pessoas físicas quanto
    /// para pessoas jurídicas, além de métodos de consulta especializados.
    /// </remarks>
    public interface IPessoaRepository
    {
        /// <summary>
        /// Adiciona uma nova pessoa física ao repositório.
        /// </summary>
        /// <param name="pessoa">A entidade de pessoa física a ser adicionada.</param>
        /// <returns>A entidade de pessoa física persistida, incluindo seu identificador atribuído.</returns>
        Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa);

        /// <summary>
        /// Busca uma pessoa física pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único da pessoa física.</param>
        /// <returns>A entidade de pessoa física encontrada, ou null se não existir.</returns>
        Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id);

        /// <summary>
        /// Busca uma pessoa física pelo seu CPF (Cadastro de Pessoa Física).
        /// </summary>
        /// <param name="cpf">O CPF da pessoa física a ser buscada.</param>
        /// <returns>A entidade de pessoa física encontrada, ou null se não existir.</returns>
        Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf);

        /// <summary>
        /// Recupera todas as pessoas físicas cadastradas no repositório.
        /// </summary>
        /// <returns>Uma coleção de entidades de pessoa física.</returns>
        Task<IEnumerable<PessoaFisica>> GetAllPessoasFisicasAsync();

        /// <summary>
        /// Atualiza os dados de uma pessoa física existente no repositório.
        /// </summary>
        /// <param name="pessoa">A entidade de pessoa física com os dados atualizados.</param>
        /// <returns>A entidade de pessoa física atualizada.</returns>
        /// <remarks>
        /// A entidade deve conter um Id válido de uma pessoa física existente no repositório.
        /// </remarks>
        Task<PessoaFisica> UpdatePessoaFisicaAsync(PessoaFisica pessoa);

        /// <summary>
        /// Remove uma pessoa física do repositório.
        /// </summary>
        /// <param name="id">O identificador único da pessoa física a ser removida.</param>
        /// <returns>
        /// <c>true</c> se a pessoa física foi removida com sucesso; caso contrário, <c>false</c>.
        /// Um valor <c>false</c> geralmente indica que a pessoa física não foi encontrada.
        /// </returns>
        Task<bool> DeletePessoaFisicaAsync(int id);

        /// <summary>
        /// Verifica se existe uma pessoa física com o CPF especificado.
        /// </summary>
        /// <param name="cpf">O CPF a ser verificado.</param>
        /// <returns><c>true</c> se existir uma pessoa física com o CPF especificado; caso contrário, <c>false</c>.</returns>
        Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf);

        /// <summary>
        /// Lista todas as pessoas físicas cadastradas no repositório.
        /// </summary>
        /// <returns>Uma coleção de entidades de pessoa física.</returns>
        Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync();

        /// <summary>
        /// Adiciona uma nova pessoa jurídica ao repositório.
        /// </summary>
        /// <param name="pessoa">A entidade de pessoa jurídica a ser adicionada.</param>
        /// <returns>A entidade de pessoa jurídica persistida, incluindo seu identificador atribuído.</returns>
        Task<PessoaJuridica> AddPessoaJuridicaAsync(PessoaJuridica pessoa);

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único da pessoa jurídica.</param>
        /// <returns>A entidade de pessoa jurídica encontrada, ou null se não existir.</returns>
        Task<PessoaJuridica?> GetPessoaJuridicaByIdAsync(int id);

        /// <summary>
        /// Busca uma pessoa jurídica pelo seu CNPJ (Cadastro Nacional da Pessoa Jurídica).
        /// </summary>
        /// <param name="cnpj">O CNPJ da pessoa jurídica a ser buscada.</param>
        /// <returns>A entidade de pessoa jurídica encontrada, ou null se não existir.</returns>
        Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj);

        /// <summary>
        /// Recupera todas as pessoas jurídicas cadastradas no repositório.
        /// </summary>
        /// <returns>Uma coleção de entidades de pessoa jurídica.</returns>
        Task<IEnumerable<PessoaJuridica>> GetAllPessoasJuridicasAsync();

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica existente no repositório.
        /// </summary>
        /// <param name="pessoa">A entidade de pessoa jurídica com os dados atualizados.</param>
        /// <returns>A entidade de pessoa jurídica atualizada.</returns>
        /// <remarks>
        /// A entidade deve conter um Id válido de uma pessoa jurídica existente no repositório.
        /// </remarks>
        Task<PessoaJuridica> UpdatePessoaJuridicaAsync(PessoaJuridica pessoa);

        /// <summary>
        /// Remove uma pessoa jurídica do repositório.
        /// </summary>
        /// <param name="id">O identificador único da pessoa jurídica a ser removida.</param>
        /// <returns>
        /// <c>true</c> se a pessoa jurídica foi removida com sucesso; caso contrário, <c>false</c>.
        /// Um valor <c>false</c> geralmente indica que a pessoa jurídica não foi encontrada.
        /// </returns>
        Task<bool> DeletePessoaJuridicaAsync(int id);

        /// <summary>
        /// Verifica se existe uma pessoa jurídica com o CNPJ especificado.
        /// </summary>
        /// <param name="cnpj">O CNPJ a ser verificado.</param>
        /// <returns><c>true</c> se existir uma pessoa jurídica com o CNPJ especificado; caso contrário, <c>false</c>.</returns>
        Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj);

        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas no repositório.
        /// </summary>
        /// <returns>Uma coleção de entidades de pessoa jurídica.</returns>
        Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync();
    }
}