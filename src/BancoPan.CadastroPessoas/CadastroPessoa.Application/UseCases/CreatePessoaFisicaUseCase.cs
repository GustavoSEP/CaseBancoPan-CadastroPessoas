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
    public class CreatePessoaFisicaUseCase : ICreatePessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<CreatePessoaFisicaUseCase> _logger;

        public CreatePessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<CreatePessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PessoaFisicaDto> ExecuteAsync(CreatePessoaFisicaCommand command)
        {
            _logger.LogInformation("Criando Pessoa Física. Nome: {Nome}, CPF: {Cpf}, CEP: {Cep}",
                                  command.Nome, command.CPF, command.CEP);

            if (!DocumentoHelper.IsValidCpf(command.CPF))
                throw new ValidationException("CPF inválido.");

            var cpfFormatted = DocumentoHelper.FormatCpf(command.CPF);

            if (await _pessoaRepository.ExistsPessoaFisicaByCpfAsync(cpfFormatted))
                throw new ValidationException("Pessoa física com esse CPF já existe.");

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

            var pessoa = new PessoaFisica(0, command.Nome, cpfFormatted, "F", endereco);

            try
            {
                var created = await _pessoaRepository.AddPessoaFisicaAsync(pessoa);
                _logger.LogInformation("Pessoa Física criada com Id {Id} e CPF {Cpf}", created.Id, created.CPF);

                return PessoaFisicaMapper.ToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir Pessoa Física no repositório. CPF: {Cpf}", cpfFormatted);
                throw;
            }
        }
    }
}