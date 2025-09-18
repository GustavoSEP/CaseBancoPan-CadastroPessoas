using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    /// <summary>
    /// Interface que define o caso de uso para consulta de pessoas físicas no sistema.
    /// </summary>
    /// <remarks>
    /// Este caso de uso segue os princípios da arquitetura hexagonal, atuando como uma porta de entrada (input port)
    /// que permite à camada de aplicação interagir com o domínio para consultar pessoas físicas existentes.
    /// 
    /// Oferece diferentes métodos de consulta:
    /// - Consulta por ID para recuperar uma pessoa física específica
    /// - Consulta por CPF para encontrar uma pessoa física pelo seu documento
    /// - Consulta geral para listar todas as pessoas físicas cadastradas
    /// </remarks>
    public interface IGetPessoaFisicaUseCase
    {
        /// <summary>
        /// Consulta uma pessoa física pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único da pessoa física a ser consultada.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um <see cref="PessoaFisicaDto"/> com os dados da pessoa física encontrada.
        /// Retorna <c>null</c> se nenhuma pessoa física for encontrada com o ID especificado.
        /// </returns>
        Task<PessoaFisicaDto> GetByIdAsync(int id);

        /// <summary>
        /// Consulta uma pessoa física pelo seu CPF (Cadastro de Pessoa Física).
        /// </summary>
        /// <param name="cpf">O CPF da pessoa física a ser consultada.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um <see cref="PessoaFisicaDto"/> com os dados da pessoa física encontrada.
        /// Retorna <c>null</c> se nenhuma pessoa física for encontrada com o CPF especificado.
        /// </returns>
        /// <remarks>
        /// O CPF pode ser informado com ou sem formatação (pontos e traço).
        /// </remarks>
        Task<PessoaFisicaDto> GetByCpfAsync(string cpf);

        /// <summary>
        /// Recupera todas as pessoas físicas cadastradas no sistema.
        /// </summary>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo uma coleção de <see cref="PessoaFisicaDto"/> com os dados de todas as pessoas físicas cadastradas.
        /// Retorna uma coleção vazia se não houver pessoas físicas cadastradas.
        /// </returns>
        Task<IEnumerable<PessoaFisicaDto>> GetAllAsync();
    }
}