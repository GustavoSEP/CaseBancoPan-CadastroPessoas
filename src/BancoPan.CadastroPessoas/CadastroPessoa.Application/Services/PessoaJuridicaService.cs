using Microsoft.Extensions.Logging;
using CadastroPessoas.Application.Helpers;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Application.Services
{
    public class PessoaJuridicaService : IPessoaJuridicaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IViaCepService _viaCepService;
        private readonly ILogger<PessoaJuridicaService> _logger;

        public PessoaJuridicaService(IPessoaRepository pessoaRepository, IViaCepService viaCepService, ILogger<PessoaJuridicaService> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PessoaJuridica> CreateAsync(string razaoSocial, string nomeFantasia, string cnpjRaw, string cep, string numero, string complemento)
        {
            _logger.LogInformation("Criando Pessoa Jurídica. RazaoSocial: {RazaoSocial}, CNPJ(raw): {CnpjRaw}, CEP: {Cep}", razaoSocial, cnpjRaw, cep);

            if (!DocumentoHelper.IsValidCnpj(cnpjRaw))
                throw new ValidationException("CNPJ inválido.");

            var cnpjFormatted = DocumentoHelper.FormatCnpj(cnpjRaw);

            if (await _pessoaRepository.ExistsPessoaJuridicaByCnpjAsync(cnpjFormatted))
                throw new Exception("Pessoa jurídica com esse CNPJ já existe.");

            Endereco enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep)
                                        ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");

            var endereco = new Endereco(
                enderecoViaCep.Cep,
                enderecoViaCep.Logradouro,
                enderecoViaCep.Bairro,
                enderecoViaCep.Cidade,
                enderecoViaCep.Estado,
                numero ?? string.Empty,
                complemento ?? string.Empty
            );

            var pessoa = new PessoaJuridica(razaoSocial, nomeFantasia, cnpjFormatted, "J", endereco);

            try
            {
                var created = await _pessoaRepository.AddPessoaJuridicaAsync(pessoa);
                _logger.LogInformation("Pessoa Jurídica criada com Id {Id} e CNPJ {Cnpj}", created.Id, created.CNPJ);
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir Pessoa Jurídica no repositório. CNPJ: {Cnpj}", cnpjFormatted);
                throw;
            }
        }

        public Task<IEnumerable<PessoaJuridica>> ListAsync()
        {
            _logger.LogInformation("Listando Pessoas Jurídicas.");
            try
            {
                return _pessoaRepository.ListPessoaJuridicaAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar Pessoas Jurídicas");
                throw;
            }
        }

        public async Task<PessoaJuridica?> GetByCnpjAsync(string cnpj)
        {
            _logger.LogInformation("Buscando Pessoa Jurídica por CNPJ: {Cnpj}", cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ValidationException("CNPJ é obrigatório.");

            var digits = DocumentoHelper.NormalizeDigits(cnpj);
            if (digits.Length != 14)
                throw new ValidationException("CNPJ inválido.");

            var formatted = DocumentoHelper.FormatCnpj(digits);

            try
            {
                return await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(formatted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pessoa Jurídica por CNPJ: {Cnpj}", formatted);
                throw;
            }
        }

        public async Task UpdateByCnpjAsync(string cnpj, string? razaoSocial, string? nomeFantasia, string? cnpjRaw, string? cep, string? numero, string? complemento)
        {
            _logger.LogInformation("Atualizando Pessoa Jurídica. CNPJ alvo: {Cnpj}", cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ValidationException("CNPJ é obrigatório para atualização.");

            var digits = DocumentoHelper.NormalizeDigits(cnpj);
            if (digits.Length != 14) throw new ValidationException("CNPJ inválido.");

            var formatted = DocumentoHelper.FormatCnpj(digits);

            PessoaJuridica? pessoa = await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(formatted);

            if (pessoa == null)
                throw new Exception("Pessoa jurídica não encontrada.");

            if (!string.IsNullOrWhiteSpace(cnpjRaw))
            {
                if (!DocumentoHelper.IsValidCnpj(cnpjRaw))
                    throw new ValidationException("CNPJ inválido.");
                var cnpjFormatted = DocumentoHelper.FormatCnpj(cnpjRaw);
                if (!string.Equals(cnpjFormatted, pessoa.CNPJ, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Não é permitido alterar o CNPJ (documento).");
            }

            if (!string.IsNullOrWhiteSpace(cep))
            {
                Endereco? enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep)
                                                ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");

                var novoEndereco = new Endereco(
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    numero ?? pessoa.Endereco.Numero,
                    complemento ?? pessoa.Endereco.Complemento
                );

                pessoa.AtualizarEndereco(novoEndereco);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(numero) || !string.IsNullOrWhiteSpace(complemento))
                {
                    pessoa.AtualizarNumeroComplemento(numero ?? string.Empty, complemento ?? string.Empty);
                }
            }

            var novaRazao = !string.IsNullOrWhiteSpace(razaoSocial) ? razaoSocial : pessoa.RazaoSocial;
            var novoNomeFantasia = !string.IsNullOrWhiteSpace(nomeFantasia) ? nomeFantasia : pessoa.NomeFantasia;

            pessoa.AtualizarDados(novaRazao, novoNomeFantasia, pessoa.CNPJ, pessoa.TipoPessoa);

            try
            {
                await _pessoaRepository.UpdatePessoaJuridicaAsync(pessoa);
                _logger.LogInformation("Pessoa Jurídica atualizada. Id: {Id}", pessoa.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Jurídica. Id: {Id}", pessoa.Id);
                throw;
            }
        }

        public async Task DeleteByCnpjAsync(string cnpj)
        {
            _logger.LogInformation("Excluindo Pessoa Jurídica por CNPJ: {Cnpj}", cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ValidationException("CNPJ é obrigatório para exclusão.");

            var digits = DocumentoHelper.NormalizeDigits(cnpj);
            if (digits.Length != 14) throw new ValidationException("CNPJ inválido.");

            var formatted = DocumentoHelper.FormatCnpj(digits);

            PessoaJuridica? pessoa = await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(formatted);

            if (pessoa == null)
                throw new Exception("Pessoa jurídica não encontrada.");

            try
            {
                await _pessoaRepository.DeletePessoaJuridicaAsync(pessoa.Id);
                _logger.LogInformation("Pessoa Jurídica excluída. Id: {Id}", pessoa.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar Pessoa Jurídica. Id: {Id}", pessoa.Id);
                throw;
            }
        }
    }
}