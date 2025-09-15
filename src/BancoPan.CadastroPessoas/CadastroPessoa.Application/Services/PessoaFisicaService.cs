using CadastroPessoas.Application.Interfaces;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Services
{
    public class PessoaFisicaService : IPessoaFisicaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IViaCepService _viaCepService;

        public PessoaFisicaService(IPessoaRepository pessoaRepository, IViaCepService viaCepService)
        {
            _pessoaRepository = pessoaRepository;
            _viaCepService = viaCepService;
        }

        public async Task<PessoaFisica> CreateAsync(string nome, string cpf, string cep, string numero, string complemento)
        {
            try
            {
                Endereco enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep);

                if (enderecoViaCep == null)
                    throw new Exception("Erro ao consultar o CEP: CEP não encontrado");

                var endereco = new Endereco
                    (
                    enderecoViaCep.Cep,
                    enderecoViaCep.Logradouro,
                    enderecoViaCep.Bairro,
                    enderecoViaCep.Cidade,
                    enderecoViaCep.Estado,
                    numero ?? string.Empty,
                    complemento ?? string.Empty
                    );

                var pessoa = new PessoaFisica(nome, cpf, "F", endereco);

                var createdPessoa = await _pessoaRepository.AddPessoaFisicaAsync(pessoa);
                return createdPessoa;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar Pessoa Física: {ex.Message}");
            }
        }

        public Task<IEnumerable<PessoaFisica>> ListAsync()
        {
            try
            {
                return _pessoaRepository.ListPessoaFisicaAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar Pessoas Físicas: {ex.Message}");
            }
        }

        public async Task<PessoaFisica> GetPessoaByCpf(string cpf)
        {
            try
            {
                var pessoas = await _pessoaRepository.ListPessoaFisicaAsync();
                var pessoa = pessoas.FirstOrDefault(p => p.CPF == cpf);
                if (pessoa == null)
                {
                    throw new Exception("Pessoa Física não encontrada.");
                }
                return pessoa;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar Pessoa Física por CPF: {ex.Message}");
            }
        }

        public async Task UpdatePessoaByCpfAsync(string cpf, string? nome, string? cpfNovo, string? cep,
                                                           string? numero, string? complemento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    throw new Exception("CPF é obrigatório para atualização.");

                PessoaFisica pessoa = await GetPessoaByCpf(cpf);

                if (!string.IsNullOrWhiteSpace(cep))
                {
                    Endereco enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep);

                    if (enderecoViaCep == null)
                        throw new Exception("Erro ao consultar o CEP informado.");

                    var novoendereco = new Endereco(
                        enderecoViaCep.Cep,
                        enderecoViaCep.Logradouro,
                        enderecoViaCep.Bairro,
                        enderecoViaCep.Cidade,
                        enderecoViaCep.Estado,
                        numero ?? pessoa.Endereco.Numero,
                        complemento ?? pessoa.Endereco.Complemento 
                    );

                    pessoa.AtualizarEndereco(novoendereco);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(numero) || !string.IsNullOrWhiteSpace(complemento))
                    {
                        pessoa.AtualizarNumeroComplemento(
                            numero ?? pessoa.Endereco.Numero,
                            complemento ?? pessoa.Endereco.Complemento
                        );
                    }
                }

                var novoNome = !string.IsNullOrWhiteSpace(nome) ? nome : pessoa.Nome;
                pessoa.AtualizarDados(novoNome, cpfNovo ?? pessoa.CPF, pessoa.TipoPessoa);

                await _pessoaRepository.UpdatePessoaFisicaAsync(pessoa);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar Pessoa Física: {ex.Message}");
            }
        }

        public async Task DeletePessoaByCpfAsync(string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    throw new Exception("CPF é obrigatório para exclusão.");

                PessoaFisica pessoa = await GetPessoaByCpf(cpf);
                await _pessoaRepository.DeletePessoaFisicaAsync(pessoa.Id); 
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir Pessoa Física: {ex.Message}");
            }
        }
    }
}