using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface IGetPessoaFisicaUseCase
    {
        Task<PessoaFisicaDto> GetByIdAsync(int id);
        Task<PessoaFisicaDto> GetByCpfAsync(string cpf);
        Task<IEnumerable<PessoaFisicaDto>> GetAllAsync();
    }
}