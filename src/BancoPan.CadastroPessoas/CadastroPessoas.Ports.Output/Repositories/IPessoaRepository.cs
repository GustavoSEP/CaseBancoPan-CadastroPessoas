using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Ports.Output.Repositories
{
    public interface IPessoaRepository
    {
        Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa);
        Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id);
        Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf);
        Task<IEnumerable<PessoaFisica>> GetAllPessoasFisicasAsync();
        Task<PessoaFisica> UpdatePessoaFisicaAsync(PessoaFisica pessoa);
        Task<bool> DeletePessoaFisicaAsync(int id);
        Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf);
        Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync();

        Task<PessoaJuridica> AddPessoaJuridicaAsync(PessoaJuridica pessoa);
        Task<PessoaJuridica?> GetPessoaJuridicaByIdAsync(int id);
        Task<PessoaJuridica?> GetPessoaJuridicaByCnpjAsync(string cnpj);
        Task<IEnumerable<PessoaJuridica>> GetAllPessoasJuridicasAsync();
        Task<PessoaJuridica> UpdatePessoaJuridicaAsync(PessoaJuridica pessoa);
        Task<bool> DeletePessoaJuridicaAsync(int id);
        Task<bool> ExistsPessoaJuridicaByCnpjAsync(string cnpj);
        Task<IEnumerable<PessoaJuridica>> ListPessoaJuridicaAsync();
    }
}