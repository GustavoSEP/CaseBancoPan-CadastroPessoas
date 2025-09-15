using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Interfaces
{
    public interface IPessoaFisicaService
    {

        Task<PessoaFisica> AddPessoaFisicaAsync(PessoaFisica pessoa);
        Task<IEnumerable<PessoaFisica>> ListPessoaFisicaAsync();
    }
}
