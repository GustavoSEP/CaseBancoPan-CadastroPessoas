using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Ports.Output.Services
{
    public interface IEnderecoPorCepProvider
    {
        Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep);
    }
}