using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases
{
    /// <summary>
    /// Use case responsável pela exclusão de pessoas jurídicas do sistema.
    /// Implementa a interface <see cref="IDeletePessoaJuridicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case verifica a existência da pessoa jurídica pelo ID fornecido
    /// antes de proceder com a exclusão, garantindo que apenas registros existentes
    /// sejam processados e fornecendo feedback apropriado.
    /// </remarks>
    public class DeletePessoaJuridicaUseCase : IDeletePessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<DeletePessoaJuridicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DeletePessoaJuridicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas jurídicas.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public DeletePessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<DeletePessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o caso de uso para excluir uma pessoa jurídica pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da pessoa jurídica a ser excluída.</param>
        /// <returns>
        /// Um valor booleano indicando o resultado da operação:
        /// - <c>true</c> se a pessoa jurídica foi excluída com sucesso;
        /// - <c>false</c> se a operação de exclusão falhou.
        /// </returns>
        /// <exception cref="ValidationException">Lançada quando a pessoa jurídica com o ID especificado não é encontrada.</exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro durante a exclusão no repositório.</exception>
        public async Task<bool> ExecuteAsync(int id)
        {
            _logger.LogInformation("Excluindo Pessoa Jurídica. ID: {Id}", id);

            var pessoa = await _pessoaRepository.GetPessoaJuridicaByIdAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Jurídica não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Jurídica com ID {id} não encontrada.");
            }

            try
            {
                var result = await _pessoaRepository.DeletePessoaJuridicaAsync(id);

                if (result)
                {
                    _logger.LogInformation("Pessoa Jurídica excluída com Id {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Falha ao excluir Pessoa Jurídica com Id {Id}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir Pessoa Jurídica no repositório. ID: {Id}", id);
                throw;
            }
        }
    }
}