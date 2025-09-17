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
    public class GetPessoaFisicaUseCase : IGetPessoaFisicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<GetPessoaFisicaUseCase> _logger;

        public GetPessoaFisicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<GetPessoaFisicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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

        public async Task<IEnumerable<PessoaFisicaDto>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todas as Pessoas Físicas");

            var pessoas = await _pessoaRepository.GetAllPessoasFisicasAsync();

            return PessoaFisicaMapper.ToDtoList(pessoas);
        }
    }
}