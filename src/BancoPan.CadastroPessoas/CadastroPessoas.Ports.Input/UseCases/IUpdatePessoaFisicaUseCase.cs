using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    /// <summary>
    /// Interface que define o caso de uso para atualização de uma pessoa física no sistema.
    /// </summary>
    /// <remarks>
    /// Este caso de uso segue os princípios da arquitetura hexagonal, atuando como uma porta de entrada (input port)
    /// que permite à camada de aplicação interagir com o domínio para atualizar uma pessoa física existente.
    /// 
    /// O fluxo típico de execução inclui:
    /// 1. Receber o identificador e um comando com os dados atualizados da pessoa física
    /// 2. Verificar se a pessoa física existe no sistema
    /// 3. Validar os dados fornecidos
    /// 4. Atualizar a entidade de domínio PessoaFisica
    /// 5. Persistir as alterações através do repositório
    /// 6. Retornar os dados atualizados da pessoa física
    /// 
    /// Este caso de uso implementa o padrão de atualização parcial, permitindo que apenas os campos
    /// que precisam ser atualizados sejam incluídos no comando.
    /// </remarks>
    public interface IUpdatePessoaFisicaUseCase
    {
        /// <summary>
        /// Executa o caso de uso para atualizar uma pessoa física existente.
        /// </summary>
        /// <param name="id">O identificador único da pessoa física a ser atualizada.</param>
        /// <param name="command">O comando contendo os dados atualizados da pessoa física.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um <see cref="PessoaFisicaDto"/> com os dados atualizados da pessoa física.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// Lançada quando a pessoa física com o ID especificado não é encontrada.
        /// </exception>
        /// <exception cref="CadastroPessoas.Domain.Exceptions.DocumentoInvalidoException">
        /// Lançada quando o CPF fornecido é inválido.
        /// </exception>
        /// <remarks>
        /// O comando de atualização pode conter apenas os campos que precisam ser alterados.
        /// Campos não incluídos no comando ou com valores nulos serão mantidos inalterados.
        /// </remarks>
        Task<PessoaFisicaDto> ExecuteAsync(int id, UpdatePessoaFisicaCommand command);
    }
}