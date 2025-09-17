using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Application.Helpers;
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
    public class CreatePessoaJuridicaUseCase : ICreatePessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<CreatePessoaJuridicaUseCase> _logger;

        public CreatePessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<CreatePessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PessoaJuridicaDto> ExecuteAsync(CreatePessoaJuridicaCommand command)
        {
            _logger.LogInformation("Criando Pessoa Jurídica. Razão Social: {RazaoSocial}, CNPJ: {Cnpj}, CEP: {Cep}",
                                  command.RazaoSocial, command.CNPJ, command.CEP);

            if (!DocumentoHelper.IsValidCnpj(command.CNPJ))
                throw new ValidationException("CNPJ inválido.");

            var cnpjFormatted = DocumentoHelper.FormatCnpj(command.CNPJ);

            if (await _pessoaRepository.ExistsPessoaJuridicaByCnpjAsync(cnpjFormatted))
                throw new ValidationException("Pessoa jurídica com esse CNPJ já existe.");

            Endereco? enderecoViaCep = await _enderecoPorCepProvider.ConsultarEnderecoPorCepAsync(command.CEP)
                                      ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");

            var endereco = new Endereco(
                enderecoViaCep.Cep,
                enderecoViaCep.Logradouro,
                enderecoViaCep.Bairro,
                enderecoViaCep.Cidade,
                enderecoViaCep.Estado,
                command.Numero ?? string.Empty,
                command.Complemento ?? string.Empty
            );

            var pessoa = new PessoaJuridica(
                0,
                command.RazaoSocial,
                command.NomeFantasia,
                cnpjFormatted,
                "J",
                endereco
            );

            try
            {
                var created = await _pessoaRepository.AddPessoaJuridicaAsync(pessoa);
                _logger.LogInformation("Pessoa Jurídica criada com Id {Id} e CNPJ {Cnpj}", created.Id, created.CNPJ);

                return PessoaJuridicaMapper.ToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir Pessoa Jurídica no repositório. CNPJ: {Cnpj}", cnpjFormatted);
                throw;
            }
        }
    }
}