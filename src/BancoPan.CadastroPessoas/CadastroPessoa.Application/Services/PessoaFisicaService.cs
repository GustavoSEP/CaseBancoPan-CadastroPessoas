using CadastroPessoas.Application.Helpers;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Application.Services
{
    public class PessoaFisicaService : IPessoaFisicaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IViaCepService _viaCepService;
        private readonly ILogger<PessoaFisicaService> _logger;

        public PessoaFisicaService(IPessoaRepository pessoaRepository, IViaCepService viaCepService, ILogger<PessoaFisicaService> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PessoaFisica> CreateAsync(string nome, string cpfRaw, string cep, string numero, string complemento)
        {
            _logger.LogInformation("Criando Pessoa Física. Nome: {Nome}, CPF(raw): {CpfRaw}, CEP: {Cep}", nome, cpfRaw, cep);

            if (!DocumentoHelper.IsValidCpf(cpfRaw))
                throw new ValidationException("CPF inválido.");

            var cpfFormatted = DocumentoHelper.FormatCpf(cpfRaw);

            if (await _pessoaRepository.ExistsPessoaFisicaByCpfAsync(cpfFormatted))
                throw new Exception("Pessoa física com esse CPF já existe.");

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

            var pessoa = new PessoaFisica(nome, cpfFormatted, "F", endereco);

            try
            {
                var created = await _pessoaRepository.AddPessoaFisicaAsync(pessoa);
                _logger.LogInformation("Pessoa Física criada com Id {Id} e CPF {Cpf}", created.Id, created.CPF);
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir Pessoa Física no repositório. CPF: {Cpf}", cpfFormatted);
                throw;
            }
        }

        public Task<IEnumerable<PessoaFisica>> ListAsync()
        {
            _logger.LogInformation("Listando Pessoas Físicas.");
            try
            {
                return _pessoaRepository.ListPessoaFisicaAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar Pessoas Físicas");
                throw;
            }
        }

        public async Task<PessoaFisica?> GetByCpfAsync(string cpf)
        {
            _logger.LogInformation("Buscando Pessoa Física por CPF: {Cpf}", cpf);

            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF é obrigatório.");

            var digits = DocumentoHelper.NormalizeDigits(cpf);
            if (digits.Length != 11)
                throw new ValidationException("CPF inválido.");

            var formatted = DocumentoHelper.FormatCpf(digits);

            try
            {
                return await _pessoaRepository.GetPessoaFisicaByCpfAsync(formatted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Pessoa Física por CPF: {Cpf}", formatted);
                throw;
            }
        }

        public async Task UpdateByCpfAsync(string cpf, string? nome, string? cpfRaw, string? cep, string? numero, string? complemento)
        {
            _logger.LogInformation("Atualizando Pessoa Física. CPF alvo: {Cpf}", cpf);

            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF é obrigatório para atualização.");

            var digits = DocumentoHelper.NormalizeDigits(cpf);
            if (digits.Length != 11) throw new ValidationException("CPF inválido.");

            var formatted = DocumentoHelper.FormatCpf(digits);

            PessoaFisica? pessoa = await _pessoaRepository.GetPessoaFisicaByCpfAsync(formatted);

            if (pessoa == null)
                throw new Exception("Pessoa física não encontrada.");

            if (!string.IsNullOrWhiteSpace(cpfRaw))
            {
                if (!DocumentoHelper.IsValidCpf(cpfRaw))
                    throw new ValidationException("CPF inválido.");

                var cpfFormatted = DocumentoHelper.FormatCpf(cpfRaw);
                if (!string.Equals(cpfFormatted, pessoa.CPF, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Não é permitido alterar o CPF (documento).");
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

            var novoNome = !string.IsNullOrWhiteSpace(nome) ? nome : pessoa.Nome;
            pessoa.AtualizarDados(novoNome, pessoa.CPF, pessoa.TipoPessoa);

            try
            {
                await _pessoaRepository.UpdatePessoaFisicaAsync(pessoa);
                _logger.LogInformation("Pessoa Física atualizada. Id: {Id}", pessoa.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Física. Id: {Id}", pessoa.Id);
                throw;
            }
        }

        public async Task DeleteByCpfAsync(string cpf)
        {
            _logger.LogInformation("Excluindo Pessoa Física por CPF: {Cpf}", cpf);

            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF é obrigatório para exclusão.");

            var digits = DocumentoHelper.NormalizeDigits(cpf);
            if (digits.Length != 11) throw new ValidationException("CPF inválido.");

            var formatted = DocumentoHelper.FormatCpf(digits);

            PessoaFisica? pessoa = await _pessoaRepository.GetPessoaFisicaByCpfAsync(formatted);

            if (pessoa == null)
                throw new Exception("Pessoa física não encontrada.");

            try
            {
                await _pessoaRepository.DeletePessoaFisicaAsync(pessoa.Id);
                _logger.LogInformation("Pessoa Física excluída. Id: {Id}", pessoa.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar Pessoa Física. Id: {Id}", pessoa.Id);
                throw;
            }
        }
    }
}