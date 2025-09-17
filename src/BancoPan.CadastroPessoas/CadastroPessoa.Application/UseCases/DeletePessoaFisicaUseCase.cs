using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases
{
    public class DeletePessoaFisicaUseCase : IDeletePessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<DeletePessoaFisicaUseCase> _logger;

        public DeletePessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<DeletePessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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