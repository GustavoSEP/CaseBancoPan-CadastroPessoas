using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    /// <summary>
    /// Interface que define o caso de uso para consulta de pessoas jurídicas no sistema.
    /// </summary>
    /// <remarks>
    /// Este caso de uso segue os princípios da arquitetura hexagonal, atuando como uma porta de entrada (input port)
    /// que permite à camada de aplicação interagir com o domínio para consultar pessoas jurídicas existentes.
    /// 
    /// Oferece diferentes métodos de consulta:
    /// - Consulta por ID para recuperar uma pessoa jurídica específica
    /// - Consulta por CNPJ para encontrar uma pessoa jurídica pelo seu documento
    /// - Consulta geral para listar todas as pessoas jurídicas cadastradas
    /// </remarks>
    public interface IGetPessoaJuridicaUseCase
    {
        /// <summary>
        /// Consulta uma pessoa jurídica pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único da pessoa jurídica a ser consultada.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um <see cref="PessoaJuridicaDto"/> com os dados da pessoa jurídica encontrada.
        /// Retorna <c>null</c> se nenhuma pessoa jurídica for encontrada com o ID especificado.
        /// </returns>
        Task<PessoaJuridicaDto> GetByIdAsync(int id);

        /// <summary>
        /// Consulta uma pessoa jurídica pelo seu CNPJ (Cadastro Nacional da Pessoa Jurídica).
        /// </summary>
        /// <param name="cnpj">O CNPJ da pessoa jurídica a ser consultada.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um <see cref="PessoaJuridicaDto"/> com os dados da pessoa jurídica encontrada.
        /// Retorna <c>null</c> se nenhuma pessoa jurídica for encontrada com o CNPJ especificado.
        /// </returns>
        /// <remarks>
        /// O CNPJ pode ser informado com ou sem formatação (pontos, barra e traço).
        /// </remarks>
        Task<PessoaJuridicaDto> GetByCnpjAsync(string cnpj);

        /// <summary>
        /// Recupera todas as pessoas jurídicas cadastradas no sistema.
        /// </summary>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo uma coleção de <see cref="PessoaJuridicaDto"/> com os dados de todas as pessoas jurídicas cadastradas.
        /// Retorna uma coleção vazia se não houver pessoas jurídicas cadastradas.
        /// </returns>
        Task<IEnumerable<PessoaJuridicaDto>> GetAllAsync();
    }
}