using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Services
{
    public class PessoaJuridicaService : IPessoaJuridicaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IViaCepService _viaCepService;

        public PessoaJuridicaService(IPessoaRepository pessoaRepository, IViaCepService viaCepService)
        {
            _pessoaRepository = pessoaRepository ?? throw new ArgumentNullException(nameof(pessoaRepository));
            _viaCepService = viaCepService ?? throw new ArgumentNullException(nameof(viaCepService));
        }

        public async Task<PessoaJuridica> CreateAsync(string razaoSocial, string nomeFantasia, string cnpj, string cep, string numero, string complemento)
        {
            try
            {
                Endereco enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep)
                    ?? throw new Exception("CEP não encontrado.");

                var endereco = new Endereco(
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    numero ?? string.Empty,
                    complemento ?? string.Empty);

                var pessoa = new PessoaJuridica(razaoSocial, nomeFantasia, cnpj, "J", endereco);

                var created = await _pessoaRepository.AddPessoaJuridicaAsync(pessoa);

                return created;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar Pessoa Jurídica: " + ex.Message);
            }
        }
        public async Task<IEnumerable<PessoaJuridica>> ListAsync() 
        {
            try
            {
                return await _pessoaRepository.ListPessoaJuridicaAsync(); // Criar este método
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar Pessoas Jurídicas: {ex.Message}");
            }
        }
        public async Task<PessoaJuridica?> GetByCnpjAsync(string cnpj)
        {
            try
            {
                return await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(cnpj); // Criar este método
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar Pessoa Jurídica por CNPJ: {ex.Message}");
            }
        }
        public async Task UpdatePessoaByCnpjAsync(string cnpj, string? razaoSocial, string? nomeFantasia, string? cnpjUpdate, string? cep, string? numero, string? complemento)
        {
            try
            {
                PessoaJuridica? pessoa = await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(cnpj); // Criar este metodo
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
                        numero ?? string.Empty,
                        complemento ?? string.Empty
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

                var novoRazao = !string.IsNullOrWhiteSpace(razaoSocial) ? razaoSocial : pessoa.RazaoSocial;
                var novoNomeFantasia = !string.IsNullOrWhiteSpace(nomeFantasia) ? nomeFantasia : pessoa.NomeFantasia;

                pessoa.AtualizarDados(novoRazao, novoNomeFantasia, pessoa.CNPJ, pessoa.TipoPessoa);
                await _pessoaRepository.UpdatePessoaJuridicaAsync(pessoa); // Criar este metodo 
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar Pessoa Jurídica. Id: {pessoa.Id}");
            }
        }
        public async Task DeletePessoaByCnpjAsync(string cnpj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cnpj))
                    throw new ValidationException("CNPJ é obrigatório para exclusão.");

                PessoaJuridica? pessoa = await _pessoaRepository.GetPessoaJuridicaByCnpjAsync(cnpj); // Precisa criar este metodo.

                if (pessoa == null)
                    throw new Exception("Pessoa jurídica não encontrada.");

                await _pessoaRepository.DeletePessoaJuridicaAsync(pessoa.Id); // Precisa criar este metodo
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar Pessoa Jurídica. Id: {pessoa.Id}");
            }
        }
    }
}
