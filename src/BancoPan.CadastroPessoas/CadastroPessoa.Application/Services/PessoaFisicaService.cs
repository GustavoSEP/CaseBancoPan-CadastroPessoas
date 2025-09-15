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
        private readonly IPessoaFisicaService _pessoaRepository;
        private readonly IViaCepService _viaCepService;
        
        public PessoaFisicaService(IPessoaFisicaService pessoaRepository, IViaCepService viaCepService)
        {
            _pessoaRepository = pessoaRepository;
            _viaCepService = viaCepService;
        }

        public async Task<PessoaFisica> CreateAsync(string nome, string cpf, string cep, string numero, string complemento)
        {
            try
            {
                Endereco enderecoViaCep = await _viaCepService.ConsultarEnderecoPorCepAsync(cep)
                        ?? throw new Exception($"Erro ao consultar o CEP: {ex.Message}");

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
    }
}
