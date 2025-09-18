using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases
{
    /// <summary>
    /// Use case responsável pela exclusão de pessoas físicas do sistema.
    /// Implementa a interface <see cref="IDeletePessoaFisicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case verifica a existência da pessoa física pelo ID fornecido
    /// antes de proceder com a exclusão, garantindo que apenas registros existentes
    /// sejam processados e fornecendo feedback apropriado.
    /// </remarks>
    public class DeletePessoaFisicaUseCase : IDeletePessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<DeletePessoaFisicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DeletePessoaFisicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas físicas.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public DeletePessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<DeletePessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o caso de uso para excluir uma pessoa física pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da pessoa física a ser excluída.</param>
        /// <returns>
        /// Um valor booleano indicando o resultado da operação:
        /// - <c>true</c> se a pessoa física foi excluída com sucesso;
        /// - <c>false</c> se a operação de exclusão falhou.
        /// </returns>
        /// <exception cref="ValidationException">Lançada quando a pessoa física com o ID especificado não é encontrada.</exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro durante a exclusão no repositório.</exception>
        public async Task<bool> ExecuteAsync(int id)
        {
            _logger.LogInformation("Excluindo Pessoa Física. ID: {Id}", id);

            var pessoa = await _pessoaRepository.GetPessoaFisicaByIdAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Física não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Física com ID {id} não encontrada.");
            }

            try
            {
                var result = await _pessoaRepository.DeletePessoaFisicaAsync(id);

                if (result)
                {
                    _logger.LogInformation("Pessoa Física excluída com Id {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Falha ao excluir Pessoa Física com Id {Id}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Pessoa Física no repositório. ID: {Id}", id);
                throw;
            }
        }
    }
}