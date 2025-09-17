using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases
{
    public class DeletePessoaJuridicaUseCase : IDeletePessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<DeletePessoaJuridicaUseCase> _logger;

        public DeletePessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<DeletePessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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