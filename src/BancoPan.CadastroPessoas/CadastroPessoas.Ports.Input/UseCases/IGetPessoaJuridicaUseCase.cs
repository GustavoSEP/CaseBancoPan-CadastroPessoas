using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface IGetPessoaJuridicaUseCase
    {
        Task<PessoaJuridicaDto> GetByIdAsync(int id);
        Task<PessoaJuridicaDto> GetByCnpjAsync(string cnpj);
        Task<IEnumerable<PessoaJuridicaDto>> GetAllAsync();
    }
}