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
    /// <summary>
    /// Use case responsável pela criação de pessoas físicas no sistema.
    /// Implementa a interface <see cref="ICreatePessoaFisicaUseCase"/> seguindo 
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case valida o CPF, verifica se já existe uma pessoa com o mesmo CPF,
    /// consulta o endereço pelo CEP informado e persiste a pessoa física no repositório.
    /// </remarks>
    public class CreatePessoaFisicaUseCase : ICreatePessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<CreatePessoaFisicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CreatePessoaFisicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas físicas.</param>
        /// <param name="enderecoPorCepProvider">Provedor de consulta de endereço por CEP.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public CreatePessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<CreatePessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o caso de uso para criar uma nova pessoa física com base nos dados fornecidos.
        /// </summary>
        /// <param name="command">Comando contendo os dados necessários para criar uma pessoa física.</param>
        /// <returns>DTO representando a pessoa física criada, incluindo seu ID gerado.</returns>
        /// <exception cref="ValidationException">
        /// Lançada quando:
        /// - O CPF é inválido
        /// - Já existe uma pessoa física com o CPF informado
        /// - O CEP não retorna um endereço válido
        /// </exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao inserir a pessoa física no repositório.</exception>
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