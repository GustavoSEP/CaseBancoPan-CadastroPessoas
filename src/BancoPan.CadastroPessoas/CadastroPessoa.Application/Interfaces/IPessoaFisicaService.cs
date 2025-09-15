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
        Task<PessoaFisica> CreateAsync(string nome, string cpf, string cep, string numero, string complemento);
        Task<IEnumerable<PessoaFisica>> ListAsync();
        Task<PessoaFisica> GetPessoaByCpf(string cpf);
        Task UpdatePessoaByCpfAsync(string cpf, string? nome, string? cpfNovo, string? cep,
                                                           string? numero, string? complemento);
        Task DeletePessoaByCpfAsync(string cpf);
    }
}
