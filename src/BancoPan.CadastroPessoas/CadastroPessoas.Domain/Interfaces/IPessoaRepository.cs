using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Interfaces
{
    public interface IPessoaRepository
    {

        // Pessoa Fisica
        Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa);
        Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync();
        Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id);
        Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf);
        Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf);
        Task UpdatePessoaFisicaAsync(PessoaFisica pessoa);
        Task DeletePessoaFisicaAsync(int id);

        // Pessoa Juridica
        Task<PessoaJuridica> AddPessoaJuridicaAsync(PessoaJuridica pessoa);
        Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync();
        Task<PessoaJuridica?> GetPessoaJuridicaByIdAsync(int id);
        Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj);
        Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj);
        Task UpdatePessoaJuridicaAsync(PessoaJuridica pessoa);
        Task DeletePessoaJuridicaAsync(int id);
    }
}
