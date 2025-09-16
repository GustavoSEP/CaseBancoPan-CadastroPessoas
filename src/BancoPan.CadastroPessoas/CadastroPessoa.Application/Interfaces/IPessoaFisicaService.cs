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
        Task<PessoaFisica?> GetByCpfAsync(string cpf);
        Task UpdateByCpfAsync(string cpf, string? nome, string? cpfRaw, string? cep, string? numero, string? complemento);
        Task DeleteByCpfAsync(string cpf);
    }
}
