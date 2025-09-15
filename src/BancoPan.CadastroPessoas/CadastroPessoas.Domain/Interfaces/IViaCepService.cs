using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Interfaces
{
    public interface IViaCepService
    {
        Task<Endereco?> ConsultaEnderecoPorCepAsync(string cep);
    }
}
