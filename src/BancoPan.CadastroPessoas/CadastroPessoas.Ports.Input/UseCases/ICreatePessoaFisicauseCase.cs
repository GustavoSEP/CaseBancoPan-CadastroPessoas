using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    /// <summary>
    /// Interface que define o caso de uso para criação de uma pessoa física no sistema.
    /// </summary>
    /// <remarks>
    /// Este caso de uso segue os princípios da arquitetura hexagonal, atuando como uma porta de entrada (input port)
    /// que permite à camada de aplicação interagir com o domínio para criar uma nova pessoa física.
    /// 
    /// O fluxo típico de execução inclui:
    /// 1. Receber um comando com os dados da pessoa física a ser criada
    /// 2. Validar os dados fornecidos (incluindo a validação do CPF)
    /// 3. Verificar se já existe uma pessoa física com o mesmo CPF
    /// 4. Criar a entidade de domínio PessoaFisica
    /// 5. Persistir a entidade através do repositório
    /// 6. Retornar os dados da pessoa física criada
    /// </remarks>
    public interface ICreatePessoaFisicaUseCase
    {
        /// <summary>
        /// Executa o caso de uso para criar uma nova pessoa física.
        /// </summary>
        /// <param name="command">O comando contendo os dados necessários para a criação da pessoa física.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo um <see cref="PessoaFisicaDto"/> com os dados da pessoa física criada.
        /// </returns>
        /// <exception cref="CadastroPessoas.Domain.Exceptions.DocumentoInvalidoException">
        /// Lançada quando o CPF fornecido é inválido.
        /// </exception>
        /// <exception cref="CadastroPessoas.Domain.Exceptions.PessoaJaExisteException">
        /// Lançada quando já existe uma pessoa física cadastrada com o mesmo CPF.
        /// </exception>
        Task<PessoaFisicaDto> ExecuteAsync(CreatePessoaFisicaCommand command);
    }
}