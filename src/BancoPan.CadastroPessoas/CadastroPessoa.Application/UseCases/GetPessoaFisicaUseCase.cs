using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Application.Helpers;
using CadastroPessoas.Application.Mappers;
using CadastroPessoas.Ports.Input.Dtos;
using CadastroPessoas.Ports.Input.UseCases;
using CadastroPessoas.Ports.Output.Repositories;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases
{
    /// <summary>
    /// Use case responsável pela consulta de pessoas físicas no sistema.
    /// Implementa a interface <see cref="IGetPessoaFisicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case fornece métodos para consultar pessoas físicas por diferentes critérios:
    /// - Por ID
    /// - Por CPF (com validação prévia)
    /// - Listagem de todas as pessoas físicas
    /// 
    /// Todas as consultas retornam os dados no formato DTO para garantir a separação
    /// entre a camada de domínio e a camada de apresentação.
    /// </remarks>
    public class GetPessoaFisicaUseCase : IGetPessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<GetPessoaFisicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetPessoaFisicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas físicas.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public GetPessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<GetPessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta uma pessoa física pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da pessoa física a ser consultada.</param>
        /// <returns>DTO com os dados da pessoa física encontrada.</returns>
        /// <exception cref="ValidationException">Lançada quando a pessoa física com o ID especificado não é encontrada.</exception>
        public async Task<PessoaFisicaDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando Pessoa Física por ID: {Id}", id);

            var pessoa = await _pessoaRepository.GetPessoaFisicaByIdAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Física não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Física com ID {id} não encontrada.");
            }

            return PessoaFisicaMapper.ToDto(pessoa);
        }

        /// <summary>
        /// Consulta uma pessoa física pelo seu CPF.
        /// </summary>
        /// <param name="cpf">O CPF da pessoa física a ser consultada (pode ser formatado ou não).</param>
        /// <returns>DTO com os dados da pessoa física encontrada.</returns>
        /// <exception cref="ValidationException">
        /// Lançada quando:
        /// - O CPF fornecido é inválido
        /// - A pessoa física com o CPF especificado não é encontrada
        /// </exception>
        public async Task<PessoaFisicaDto> GetByCpfAsync(string cpf)
        {
            _logger.LogInformation("Buscando Pessoa Física por CPF: {Cpf}", cpf);

            if (!DocumentoHelper.IsValidCpf(cpf))
            {
                _logger.LogWarning("Tentativa de buscar com CPF inválido: {Cpf}", cpf);
                throw new ValidationException("CPF inválido.");
            }

            var cpfFormatted = DocumentoHelper.FormatCpf(cpf);
            var pessoa = await _pessoaRepository.GetPessoaFisicaByCpfAsync(cpfFormatted);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Física não encontrada com CPF: {Cpf}", cpfFormatted);
                throw new ValidationException($"Pessoa Física com CPF {cpfFormatted} não encontrada.");
            }

            return PessoaFisicaMapper.ToDto(pessoa);
        }

        /// <summary>
        /// Obtém a lista de todas as pessoas físicas cadastradas no sistema.
        /// </summary>
        /// <returns>Coleção de DTOs com os dados de todas as pessoas físicas.</returns>
        /// <remarks>
        /// Este método retorna uma coleção vazia se não houver pessoas físicas cadastradas,
        /// em vez de lançar uma exceção.
        /// </remarks>
        public async Task<IEnumerable<PessoaFisicaDto>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todas as Pessoas Físicas");

            var pessoas = await _pessoaRepository.GetAllPessoasFisicasAsync();

            return PessoaFisicaMapper.ToDtoList(pessoas);
        }
    }
}