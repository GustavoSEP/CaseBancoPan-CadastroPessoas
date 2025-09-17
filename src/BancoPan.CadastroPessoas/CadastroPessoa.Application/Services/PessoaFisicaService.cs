using CadastroPessoas.Application.Helpers;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Ports.Output.Repositories;
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

            // Corrigido construtor PessoaFisica
            var pessoa = new PessoaFisica(0, nome, cpfFormatted, "F", endereco);

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

            // Substituindo o método NormalizeDigits
            var digits = cpf.Replace(".", "").Replace("-", "").Trim();
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

            // Substituindo o método NormalizeDigits
            var digits = cpf.Replace(".", "").Replace("-", "").Trim();
            if (digits.Length != 11) throw new ValidationException("CPF inválido.");

            var formatted = DocumentoHelper.FormatCpf(digits);

            PessoaFisica? pessoaExistente = await _pessoaRepository.GetPessoaFisicaByCpfAsync(formatted);

            if (pessoaExistente == null)
                throw new Exception("Pessoa física não encontrada.");

            if (!string.IsNullOrWhiteSpace(cpfRaw))
            {
                if (!DocumentoHelper.IsValidCpf(cpfRaw))
                    throw new ValidationException("CPF inválido.");

                var cpfFormatted = DocumentoHelper.FormatCpf(cpfRaw);
                if (!string.Equals(cpfFormatted, pessoaExistente.CPF, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Não é permitido alterar o CPF (documento).");
            }

            // Criar o novo endereço
            Endereco novoEndereco;
            if (!string.IsNullOrWhiteSpace(cep))
            {
                Endereco? enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep)
                                                ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");

                novoEndereco = new Endereco(
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    numero ?? pessoaExistente.Endereco.Numero,
                    complemento ?? pessoaExistente.Endereco.Complemento
                );
            }
            else
            {
                // Manter o endereço atual, atualizando apenas número e complemento se fornecidos
                novoEndereco = new Endereco(
                    pessoaExistente.Endereco.Cep,
                    pessoaExistente.Endereco.Logradouro,
                    pessoaExistente.Endereco.Bairro,
                    pessoaExistente.Endereco.Cidade,
                    pessoaExistente.Endereco.Estado,
                    !string.IsNullOrWhiteSpace(numero) ? numero : pessoaExistente.Endereco.Numero,
                    !string.IsNullOrWhiteSpace(complemento) ? complemento : pessoaExistente.Endereco.Complemento
                );
            }

            // Criar uma nova PessoaFisica com os dados atualizados
            var pessoaAtualizada = new PessoaFisica(
                pessoaExistente.Id,
                !string.IsNullOrWhiteSpace(nome) ? nome : pessoaExistente.Nome,
                pessoaExistente.CPF,
                pessoaExistente.Tipo,
                novoEndereco
            );

            try
            {
                await _pessoaRepository.UpdatePessoaFisicaAsync(pessoaAtualizada);
                _logger.LogInformation("Pessoa Física atualizada. Id: {Id}", pessoaAtualizada.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Física. Id: {Id}", pessoaAtualizada.Id);
                throw;
            }
        }

        public async Task DeleteByCpfAsync(string cpf)
        {
            _logger.LogInformation("Excluindo Pessoa Física por CPF: {Cpf}", cpf);

            if (string.IsNullOrWhiteSpace(cpf))
                throw new ValidationException("CPF é obrigatório para exclusão.");

            // Substituindo o método NormalizeDigits
            var digits = cpf.Replace(".", "").Replace("-", "").Trim();
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