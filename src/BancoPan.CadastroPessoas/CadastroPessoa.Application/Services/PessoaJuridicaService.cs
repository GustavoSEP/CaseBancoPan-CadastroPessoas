using Microsoft.Extensions.Logging;
using CadastroPessoas.Application.Helpers;
using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Output.Repositories;

namespace CadastroPessoas.Application.Services
{
    /// <summary>
    /// Serviço para gerenciamento de pessoas jurídicas, implementando a interface <see cref="IPessoaJuridicaService"/>.
    /// Provê funcionalidades para criar, listar, buscar, atualizar e excluir pessoas jurídicas.
    /// </summary>
    public class PessoaJuridicaService : IPessoaJuridicaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IViaCepService _viaCepService;
        private readonly ILogger<PessoaJuridicaService> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaJuridicaService"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas jurídicas.</param>
        /// <param name="viaCepService">Serviço para consulta de endereços pelo CEP.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public PessoaJuridicaService(IPessoaRepository pessoaRepository, IViaCepService viaCepService, ILogger<PessoaJuridicaService> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Cria uma nova pessoa jurídica com os dados fornecidos.
        /// </summary>
        /// <param name="razaoSocial">Razão social da empresa.</param>
        /// <param name="nomeFantasia">Nome fantasia da empresa.</param>
        /// <param name="cnpjRaw">CNPJ não formatado.</param>
        /// <param name="cep">CEP para consulta e criação do endereço.</param>
        /// <param name="numero">Número do endereço.</param>
        /// <param name="complemento">Complemento do endereço.</param>
        /// <returns>A pessoa jurídica criada, incluindo seu ID gerado.</returns>
        /// <exception cref="ValidationException">Lançada quando o CNPJ é inválido ou o CEP não retorna um endereço válido.</exception>
        /// <exception cref="Exception">Lançada quando já existe uma pessoa jurídica com o CNPJ informado ou ocorre um erro no repositório.</exception>
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

            // Corrigido construtor PessoaJuridica
            var pessoa = new PessoaJuridica(0, razaoSocial, nomeFantasia, cnpjFormatted, "J", endereco);

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

        /// <summary>
        /// Lista todas as pessoas jurídicas cadastradas.
        /// </summary>
        /// <returns>Coleção de pessoas jurídicas.</returns>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao acessar o repositório.</exception>
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

        /// <summary>
        /// Busca uma pessoa jurídica pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica, pode ser formatado ou não.</param>
        /// <returns>A pessoa jurídica encontrada ou null se não existir.</returns>
        /// <exception cref="ValidationException">Lançada quando o CNPJ é inválido ou vazio.</exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro ao acessar o repositório.</exception>
        public async Task<PessoaJuridica?> GetByCnpjAsync(string cnpj)
        {
            _logger.LogInformation("Buscando Pessoa Jurídica por CNPJ: {Cnpj}", cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ValidationException("CNPJ é obrigatório.");

            // Substituindo o método NormalizeDigits
            var digits = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
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

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica identificada pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser atualizada.</param>
        /// <param name="razaoSocial">Nova razão social (opcional).</param>
        /// <param name="nomeFantasia">Novo nome fantasia (opcional).</param>
        /// <param name="cnpjRaw">CNPJ para validação (deve ser o mesmo da pessoa jurídica).</param>
        /// <param name="cep">Novo CEP para atualização de endereço (opcional).</param>
        /// <param name="numero">Novo número de endereço (opcional).</param>
        /// <param name="complemento">Novo complemento de endereço (opcional).</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="ValidationException">Lançada quando o CNPJ é inválido, vazio ou o CEP não retorna um endereço válido.</exception>
        /// <exception cref="Exception">Lançada quando a pessoa jurídica não é encontrada, quando tenta-se alterar o CNPJ, ou quando ocorre um erro no repositório.</exception>
        public async Task UpdateByCnpjAsync(string cnpj, string? razaoSocial, string? nomeFantasia, string? cnpjRaw, string? cep, string? numero, string? complemento)
        {
            _logger.LogInformation("Atualizando Pessoa Jurídica. CNPJ alvo: {Cnpj}", cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ValidationException("CNPJ é obrigatório para atualização.");

            // Substituindo o método NormalizeDigits
            var digits = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
            if (digits.Length != 14) throw new ValidationException("CNPJ inválido.");

            var formatted = DocumentoHelper.FormatCnpj(digits);

            PessoaJuridica? pessoaExistente = await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(formatted);

            if (pessoaExistente == null)
                throw new Exception("Pessoa jurídica não encontrada.");

            if (!string.IsNullOrWhiteSpace(cnpjRaw))
            {
                if (!DocumentoHelper.IsValidCnpj(cnpjRaw))
                    throw new ValidationException("CNPJ inválido.");
                var cnpjFormatted = DocumentoHelper.FormatCnpj(cnpjRaw);
                if (!string.Equals(cnpjFormatted, pessoaExistente.CNPJ, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Não é permitido alterar o CNPJ (documento).");
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

            // Criar uma nova PessoaJuridica com os dados atualizados
            var pessoaAtualizada = new PessoaJuridica(
                pessoaExistente.Id,
                !string.IsNullOrWhiteSpace(razaoSocial) ? razaoSocial : pessoaExistente.RazaoSocial,
                !string.IsNullOrWhiteSpace(nomeFantasia) ? nomeFantasia : pessoaExistente.NomeFantasia,
                pessoaExistente.CNPJ,
                pessoaExistente.Tipo, // Corrigido de TipoPessoa para Tipo
                novoEndereco
            );

            try
            {
                await _pessoaRepository.UpdatePessoaJuridicaAsync(pessoaAtualizada);
                _logger.LogInformation("Pessoa Jurídica atualizada. Id: {Id}", pessoaAtualizada.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Jurídica. Id: {Id}", pessoaAtualizada.Id);
                throw;
            }
        }

        /// <summary>
        /// Exclui uma pessoa jurídica pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser excluída.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        /// <exception cref="ValidationException">Lançada quando o CNPJ é inválido ou vazio.</exception>
        /// <exception cref="Exception">Lançada quando a pessoa jurídica não é encontrada ou ocorre um erro no repositório.</exception>
        public async Task DeleteByCnpjAsync(string cnpj)
        {
            _logger.LogInformation("Excluindo Pessoa Jurídica por CNPJ: {Cnpj}", cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ValidationException("CNPJ é obrigatório para exclusão.");

            // Substituindo o método NormalizeDigits
            var digits = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
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