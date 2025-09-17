using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Application.Mappers;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using CadastroPessoas.Ports.Output.Services;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases
{
    public class UpdatePessoaFisicaUseCase : IUpdatePessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<UpdatePessoaFisicaUseCase> _logger;

        public UpdatePessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<UpdatePessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PessoaFisicaDto> ExecuteAsync(int id, UpdatePessoaFisicaCommand command)
        {
            _logger.LogInformation("Atualizando Pessoa Física. ID: {Id}, Nome: {Nome}, CEP: {Cep}",
                                 id, command.Nome, command.CEP);

            var pessoaExistente = await _pessoaRepository.GetPessoaFisicaByIdAsync(id);

            if (pessoaExistente == null)
            {
                _logger.LogWarning("Pessoa Física não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Física com ID {id} não encontrada.");
            }

            // Verificar se precisamos atualizar o endereço via CEP
            Endereco enderecoAtualizado;
            if (!string.IsNullOrWhiteSpace(command.CEP))
            {
                var enderecoViaCep = await _enderecoPorCepProvider.ConsultarEnderecoPorCepAsync(command.CEP)
                    ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");
                
                // Criar um novo objeto Endereco com os dados do CEP e manter número/complemento se não fornecidos
                enderecoAtualizado = new Endereco(
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    command.Numero ?? pessoaExistente.Endereco.Numero,
                    command.Complemento ?? pessoaExistente.Endereco.Complemento
                );
            }
            else
            {
                // Criar um novo objeto Endereco mantendo os dados atuais, atualizando apenas número e/ou complemento se fornecidos
                enderecoAtualizado = new Endereco(
                    pessoaExistente.Endereco.Cep,
                    pessoaExistente.Endereco.Logradouro,
                    pessoaExistente.Endereco.Bairro,
                    pessoaExistente.Endereco.Cidade,
                    pessoaExistente.Endereco.Estado,
                    command.Numero ?? pessoaExistente.Endereco.Numero,
                    command.Complemento ?? pessoaExistente.Endereco.Complemento
                );
            }

            // Criar uma nova PessoaFisica com os dados atualizados
            var pessoaAtualizada = new PessoaFisica(
                pessoaExistente.Id,
                command.Nome ?? pessoaExistente.Nome,
                pessoaExistente.CPF,  // CPF não deve ser alterado
                pessoaExistente.Tipo, // Tipo não deve ser alterado
                enderecoAtualizado    // Novo objeto Endereco
            );

            try
            {
                var updated = await _pessoaRepository.UpdatePessoaFisicaAsync(pessoaAtualizada);
                _logger.LogInformation("Pessoa Física atualizada com Id {Id}", updated.Id);

                return PessoaFisicaMapper.ToDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Física no repositório. ID: {Id}", id);
                throw;
            }
        }
    }
}