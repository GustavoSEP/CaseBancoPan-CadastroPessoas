using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Interfaces
{
    public interface IPessoaRepository
    {

        Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa);
        Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync();
        Task<PessoaFisica?> GetPessoaFisicaByIdAsync(int id);
        Task<PessoaFisica?> GetPessoaFisicaByCpfAsync(string cpf);
        Task<bool> ExistsPessoaFisicaByCpfAsync(string cpf);
        Task UpdatePessoaFisicaAsync(PessoaFisica pessoa);
        Task DeletePessoaFisicaAsync(int id);

    }
}
