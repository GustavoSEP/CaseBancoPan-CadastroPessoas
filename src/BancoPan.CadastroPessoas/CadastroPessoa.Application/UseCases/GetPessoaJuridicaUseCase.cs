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
    public class GetPessoaJuridicaUseCase : IGetPessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ILogger<GetPessoaJuridicaUseCase> _logger;

        public GetPessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            ILogger<GetPessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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

        public async Task<IEnumerable<PessoaJuridicaDto>> GetAllAsync()
        {
            _logger.LogInformation("Buscando todas as Pessoas Jurídicas");

            var pessoas = await _pessoaRepository.GetAllPessoasJuridicasAsync();

            return PessoaJuridicaMapper.ToDtoList(pessoas);
        }
    }
}