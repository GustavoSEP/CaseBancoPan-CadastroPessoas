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
    public class UpdatePessoaJuridicaUseCase : IUpdatePessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<UpdatePessoaJuridicaUseCase> _logger;

        public UpdatePessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<UpdatePessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PessoaJuridicaDto> ExecuteAsync(int id, UpdatePessoaJuridicaCommand command)
        {
            _logger.LogInformation("Atualizando Pessoa Jurídica. ID: {Id}, Razão Social: {RazaoSocial}, CEP: {Cep}",
                                 id, command.RazaoSocial, command.CEP);

            var pessoa = await _pessoaRepository.GetPessoaJuridicaByIdAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Jurídica não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Jurídica com ID {id} não encontrada.");
            }

            Endereco? enderecoViaCep = null;
            if (!string.IsNullOrWhiteSpace(command.CEP))
            {
                enderecoViaCep = await _enderecoPorCepProvider.ConsultarEnderecoPorCepAsync(command.CEP)
                    ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");
            }

            if (!string.IsNullOrWhiteSpace(command.RazaoSocial))
            {
                pessoa = new PessoaJuridica(
                    pessoa.Id,
                    command.RazaoSocial,
                    !string.IsNullOrWhiteSpace(command.NomeFantasia) ? command.NomeFantasia : pessoa.NomeFantasia,
                    pessoa.CNPJ,
                    pessoa.Tipo,
                    pessoa.Endereco
                );
            }
            else if (!string.IsNullOrWhiteSpace(command.NomeFantasia))
            {
                pessoa = new PessoaJuridica(
                    pessoa.Id,
                    pessoa.RazaoSocial,
                    command.NomeFantasia,
                    pessoa.CNPJ,
                    pessoa.Tipo,
                    pessoa.Endereco
                );
            }

            if (enderecoViaCep != null)
            {
                var endereco = new Endereco(
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    command.Numero ?? pessoa.Endereco.Numero,
                    command.Complemento ?? pessoa.Endereco.Complemento
                );

                pessoa = new PessoaJuridica(
                    pessoa.Id,
                    pessoa.RazaoSocial,
                    pessoa.NomeFantasia,
                    pessoa.CNPJ,
                    pessoa.Tipo,
                    endereco
                );
            }
            else
            {
                // Atualiza apenas número e complemento se fornecidos
                if (!string.IsNullOrWhiteSpace(command.Numero) || !string.IsNullOrWhiteSpace(command.Complemento))
                {
                    var endereco = new Endereco(
                        pessoa.Endereco.Cep,
                        pessoa.Endereco.Logradouro,
                        pessoa.Endereco.Bairro,
                        pessoa.Endereco.Cidade,
                        pessoa.Endereco.Estado,
                        !string.IsNullOrWhiteSpace(command.Numero) ? command.Numero : pessoa.Endereco.Numero,
                        !string.IsNullOrWhiteSpace(command.Complemento) ? command.Complemento : pessoa.Endereco.Complemento
                    );

                    pessoa = new PessoaJuridica(
                        pessoa.Id,
                        pessoa.RazaoSocial,
                        pessoa.NomeFantasia,
                        pessoa.CNPJ,
                        pessoa.Tipo,
                        endereco
                    );
                }
            }

            try
            {
                var updated = await _pessoaRepository.UpdatePessoaJuridicaAsync(pessoa);
                _logger.LogInformation("Pessoa Jurídica atualizada com Id {Id}", updated.Id);

                return PessoaJuridicaMapper.ToDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Jurídica no repositório. ID: {Id}", id);
                throw;
            }
        }
    }
}