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
    /// <summary>
    /// Use case responsável pela atualização de pessoas físicas no sistema.
    /// Implementa a interface <see cref="IUpdatePessoaFisicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case permite a atualização parcial de dados de uma pessoa física,
    /// mantendo imutáveis os campos que não devem ser alterados (como CPF e tipo).
    /// Caso um CEP seja fornecido, o endereço é reconsultado; caso contrário,
    /// apenas os campos específicos do endereço são atualizados.
    /// </remarks>
    public class UpdatePessoaFisicaUseCase : IUpdatePessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<UpdatePessoaFisicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="UpdatePessoaFisicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas físicas.</param>
        /// <param name="enderecoPorCepProvider">Provedor de consulta de endereço por CEP.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public UpdatePessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<UpdatePessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o caso de uso para atualizar uma pessoa física existente.
        /// </summary>
        /// <param name="id">O ID da pessoa física a ser atualizada.</param>
        /// <param name="command">Comando contendo os dados a serem atualizados.</param>
        /// <returns>DTO com os dados atualizados da pessoa física.</returns>
        /// <exception cref="ValidationException">
        /// Lançada quando:
        /// - A pessoa física com o ID especificado não é encontrada
        /// - O CEP fornecido não retorna um endereço válido
        /// </exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro durante a atualização no repositório.</exception>
        /// <remarks>
        /// Os campos não incluídos no comando de atualização ou com valor nulo mantêm seus valores originais.
        /// O CPF e o tipo da pessoa não são alterados, mesmo se fornecidos no comando.
        /// </remarks>
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