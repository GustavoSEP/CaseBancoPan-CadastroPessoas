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
    /// Use case responsável pela criação de pessoas jurídicas no sistema.
    /// Implementa a interface <see cref="ICreatePessoaJuridicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case valida o CNPJ, verifica se já existe uma pessoa jurídica com o mesmo CNPJ,
    /// consulta o endereço pelo CEP informado e persiste a pessoa jurídica no repositório.
    /// </remarks>
    public class CreatePessoaJuridicaUseCase : ICreatePessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<CreatePessoaJuridicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CreatePessoaJuridicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas jurídicas.</param>
        /// <param name="enderecoPorCepProvider">Provedor de consulta de endereço por CEP.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public CreatePessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<CreatePessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o caso de uso para criar uma nova pessoa jurídica com base nos dados fornecidos.
        /// </summary>
        /// <param name="command">Comando contendo os dados necessários para criar uma pessoa jurídica.</param>
        /// <returns>DTO representando a pessoa jurídica criada, incluindo seu ID gerado.</returns>
        /// <exception cref="ValidationException">
        /// Lançada quando:
        /// - O CNPJ é inválido
        /// - Já existe uma pessoa jurídica com o CNPJ informado
        /// - O CEP não retorna um endereço válido
        /// </exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao inserir a pessoa jurídica no repositório.</exception>
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