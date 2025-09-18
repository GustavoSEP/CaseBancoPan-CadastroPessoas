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
    /// Use case responsável pela atualização de pessoas jurídicas no sistema.
    /// Implementa a interface <see cref="IUpdatePessoaJuridicaUseCase"/> seguindo
    /// os princípios da Arquitetura Hexagonal.
    /// </summary>
    /// <remarks>
    /// Este use case permite a atualização parcial de dados de uma pessoa jurídica,
    /// mantendo imutáveis os campos que não devem ser alterados (como CNPJ e tipo).
    /// A atualização é realizada de forma incremental, tratando cada campo de forma 
    /// independente (razão social, nome fantasia, endereço) para evitar sobrescrita
    /// de dados não fornecidos no comando.
    /// </remarks>
    public class UpdatePessoaJuridicaUseCase : IUpdatePessoaJuridicaUseCase
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;
        private readonly ILogger<UpdatePessoaJuridicaUseCase> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="UpdatePessoaJuridicaUseCase"/>.
        /// </summary>
        /// <param name="pessoaRepository">Repositório para acesso aos dados de pessoas jurídicas.</param>
        /// <param name="enderecoPorCepProvider">Provedor de consulta de endereço por CEP.</param>
        /// <param name="logger">Logger para registro de operações.</param>
        /// <exception cref="ArgumentNullException">Lançada quando algum parâmetro é nulo.</exception>
        public UpdatePessoaJuridicaUseCase(
            IPessoaRepository pessoaRepository,
            IEnderecoPorCepProvider enderecoPorCepProvider,
            ILogger<UpdatePessoaJuridicaUseCase> logger)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executa o caso de uso para atualizar uma pessoa jurídica existente.
        /// </summary>
        /// <param name="id">O ID da pessoa jurídica a ser atualizada.</param>
        /// <param name="command">Comando contendo os dados a serem atualizados.</param>
        /// <returns>DTO com os dados atualizados da pessoa jurídica.</returns>
        /// <exception cref="ValidationException">
        /// Lançada quando:
        /// - A pessoa jurídica com o ID especificado não é encontrada
        /// - O CEP fornecido não retorna um endereço válido
        /// </exception>
        /// <exception cref="Exception">Lançada quando ocorre um erro durante a atualização no repositório.</exception>
        /// <remarks>
        /// O método implementa uma abordagem incremental para atualização, onde:
        /// 1. Verifica e atualiza os dados de identificação (razão social, nome fantasia)
        /// 2. Verifica e atualiza o endereço via CEP, se fornecido
        /// 3. Atualiza apenas número e complemento, se apenas estes foram fornecidos
        /// 
        /// O CNPJ e o tipo da pessoa não são alterados em nenhuma circunstância.
        /// </remarks>
        public async Task<PessoaJuridicaDto> ExecuteAsync(int id, UpdatePessoaJuridicaCommand command)
        {
            _logger.LogInformation("Atualizando Pessoa Jurídica. ID: {Id}, Razão Social: {RazaoSocial}, CEP: {Cep}",
                                 id, command.RazaoSocial, command.CEP);

            var pessoa = await _pessoaRepository.GetPessoaJuridicaByIdAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning("Pessoa Jurídica não encontrada com ID: {Id}", id);
                throw new ValidationException($"Pessoa Jurídica com ID {id} não encontrada.");
            }

            // Consulta o endereço pelo CEP, se fornecido
            Endereco? enderecoViaCep = null;
            if (!string.IsNullOrWhiteSpace(command.CEP))
            {
                enderecoViaCep = await _enderecoPorCepProvider.ConsultarEnderecoPorCepAsync(command.CEP)
                    ?? throw new ValidationException("Endereço não encontrado para o CEP informado.");
            }

            // Atualiza razão social e/ou nome fantasia se fornecidos
            if (!string.IsNullOrWhiteSpace(command.RazaoSocial))
            {
                pessoa = new PessoaJuridica(
                    pessoa.Id,
                    command.RazaoSocial,
                    !string.IsNullOrWhiteSpace(command.NomeFantasia) ? command.NomeFantasia : pessoa.NomeFantasia,
                    pessoa.CNPJ,
                    pessoa.Tipo,
                    pessoa.Endereco
                );
            }
            else if (!string.IsNullOrWhiteSpace(command.NomeFantasia))
            {
                pessoa = new PessoaJuridica(
                    pessoa.Id,
                    pessoa.RazaoSocial,
                    command.NomeFantasia,
                    pessoa.CNPJ,
                    pessoa.Tipo,
                    pessoa.Endereco
                );
            }

            // Atualiza endereço completo se CEP foi consultado
            if (enderecoViaCep != null)
            {
                var endereco = new Endereco(
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    command.Numero ?? pessoa.Endereco.Numero,
                    command.Complemento ?? pessoa.Endereco.Complemento
                );

                pessoa = new PessoaJuridica(
                    pessoa.Id,
                    pessoa.RazaoSocial,
                    pessoa.NomeFantasia,
                    pessoa.CNPJ,
                    pessoa.Tipo,
                    endereco
                );
            }
            else
            {
                // Atualiza apenas número e complemento se fornecidos
                if (!string.IsNullOrWhiteSpace(command.Numero) || !string.IsNullOrWhiteSpace(command.Complemento))
                {
                    var endereco = new Endereco(
                        pessoa.Endereco.Cep,
                        pessoa.Endereco.Logradouro,
                        pessoa.Endereco.Bairro,
                        pessoa.Endereco.Cidade,
                        pessoa.Endereco.Estado,
                        !string.IsNullOrWhiteSpace(command.Numero) ? command.Numero : pessoa.Endereco.Numero,
                        !string.IsNullOrWhiteSpace(command.Complemento) ? command.Complemento : pessoa.Endereco.Complemento
                    );

                    pessoa = new PessoaJuridica(
                        pessoa.Id,
                        pessoa.RazaoSocial,
                        pessoa.NomeFantasia,
                        pessoa.CNPJ,
                        pessoa.Tipo,
                        endereco
                    );
                }
            }

            try
            {
                var updated = await _pessoaRepository.UpdatePessoaJuridicaAsync(pessoa);
                _logger.LogInformation("Pessoa Jurídica atualizada com Id {Id}", updated.Id);

                return PessoaJuridicaMapper.ToDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar Pessoa Jurídica no repositório. ID: {Id}", id);
                throw;
            }
        }
    }
}