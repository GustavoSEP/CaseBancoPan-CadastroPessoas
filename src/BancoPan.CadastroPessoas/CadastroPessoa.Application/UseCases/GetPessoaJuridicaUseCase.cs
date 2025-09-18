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
    /// Use case responsável pela consulta de pessoas jurídicas no sistema.
    /// Implementa a interface <see cref="IGetPessoaJuridicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case fornece métodos para consultar pessoas jurídicas por diferentes critérios:
    /// - Por ID
    /// - Por CNPJ (com validação prévia)
    /// - Listagem de todas as pessoas jurídicas
    /// 
    /// Todas as consultas retornam os dados no formato DTO para garantir a separação
    /// entre a camada de domínio e a camada de apresentação.
    /// </remarks>
    public class GetPessoaJuridicaUseCase : IGetPessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<GetPessoaJuridicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetPessoaJuridicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas jurídicas.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public GetPessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<GetPessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Consulta uma pessoa jurídica pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da pessoa jurídica a ser consultada.</param>
        /// <returns>DTO com os dados da pessoa jurídica encontrada.</returns>
        /// <exception cref="ValidationException">Lançada quando a pessoa jurídica com o ID especificado não é encontrada.</exception>
        public async Task<PessoaJuridicaDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Buscando Pessoa Jurídica por ID: {Id}", id);

            var pessoa = await _pessoaRepository.GetPessoaJuridicaByIdAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Jurídica não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Jurídica com ID {id} não encontrada.");
            }

            return PessoaJuridicaMapper.ToDto(pessoa);
        }

        /// <summary>
        /// Consulta uma pessoa jurídica pelo seu CNPJ.
        /// </summary>
        /// <param name="cnpj">O CNPJ da pessoa jurídica a ser consultada (pode ser formatado ou não).</param>
        /// <returns>DTO com os dados da pessoa jurídica encontrada.</returns>
        /// <exception cref="ValidationException">
        /// Lançada quando:
        /// - O CNPJ fornecido é inválido
        /// - A pessoa jurídica com o CNPJ especificado não é encontrada
        /// </exception>
        public async Task<PessoaJuridicaDto> GetByCnpjAsync(string cnpj)
        {
            _logger.LogInformation("Buscando Pessoa Jurídica por CNPJ: {Cnpj}", cnpj);

            if (!DocumentoHelper.IsValidCnpj(cnpj))
            {
                _logger.LogWarning("Tentativa de buscar com CNPJ inválido: {Cnpj}", cnpj);
                throw new ValidationException("CNPJ inválido.");
            }

            var cnpjFormatted = DocumentoHelper.FormatCnpj(cnpj);
            var pessoa = await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(cnpjFormatted);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Jurídica não encontrada com CNPJ: {Cnpj}", cnpjFormatted);
                throw new ValidationException($"Pessoa Jurídica com CNPJ {cnpjFormatted} não encontrada.");
            }

            return PessoaJuridicaMapper.ToDto(pessoa);
        }

        /// <summary>
        /// Obtém a lista de todas as pessoas jurídicas cadastradas no sistema.
        /// </summary>
        /// <returns>Coleção de DTOs com os dados de todas as pessoas jurídicas.</returns>
        /// <remarks>
        /// Este método retorna uma coleção vazia se não houver pessoas jurídicas cadastradas,
        /// em vez de lançar uma exceção.
        /// </remarks>
        public async Task<IEnumerable<PessoaJuridicaDto>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todas as Pessoas Jurídicas");

            var pessoas = await _pessoaRepository.GetAllPessoasJuridicasAsync();

            return PessoaJuridicaMapper.ToDtoList(pessoas);
        }
    }
}