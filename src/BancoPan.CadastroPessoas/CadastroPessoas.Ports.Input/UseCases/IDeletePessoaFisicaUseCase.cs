using System.Threading.Tasks;

namespace CadastroPessoas.Ports.Input.UseCases
{
    /// <summary>
    /// Interface que define o caso de uso para exclusão de uma pessoa física do sistema.
    /// </summary>
    /// <remarks>
    /// Este caso de uso segue os princípios da arquitetura hexagonal, atuando como uma porta de entrada (input port)
    /// que permite à camada de aplicação interagir com o domínio para excluir uma pessoa física existente.
    /// 
    /// O fluxo típico de execução inclui:
    /// 1. Receber o identificador da pessoa física a ser excluída
    /// 2. Verificar se a pessoa física existe no sistema
    /// 3. Excluir a pessoa física do repositório
    /// 4. Retornar um indicador de sucesso da operação
    /// </remarks>
    public interface IDeletePessoaFisicaUseCase
    {
        /// <summary>
        /// Executa o caso de uso para excluir uma pessoa física existente.
        /// </summary>
        /// <param name="id">O identificador único da pessoa física a ser excluída.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um valor booleano que indica o resultado da operação:
        /// <c>true</c> se a pessoa física foi excluída com sucesso; caso contrário, <c>false</c>.
        /// O valor <c>false</c> geralmente indica que a pessoa física com o ID especificado não foi encontrada.
        /// </returns>
        Task<bool> ExecuteAsync(int id);
    }
}