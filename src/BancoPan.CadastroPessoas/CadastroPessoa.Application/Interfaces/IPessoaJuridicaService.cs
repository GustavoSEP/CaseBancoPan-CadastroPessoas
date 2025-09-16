using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Application.Interfaces
{
    public interface IPessoaJuridicaService
    {
        Task<PessoaJuridica> CreateAsync(string razaoSocial, string nomeFantasia, string cnpj, string? representanteLegal, string cep, string numero, string complemento);
        Task<IEnumerable<PessoaJuridica>> ListAsync();
        Task<PessoaJuridica?> GetByCnpjAsync(string cnpj);
        Task UpdateByCnpjAsync(string cnpj, string? razaoSocial, string? nomeFantasia, string? cnpjRaw, string? representanteLegal, string? cep, string? numero, string? complemento);
        Task DeleteByCnpjAsync(string cnpj);
    }
}
