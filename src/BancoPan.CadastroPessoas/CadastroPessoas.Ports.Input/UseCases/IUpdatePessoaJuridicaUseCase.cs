using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface IUpdatePessoaJuridicaUseCase
    {
        Task<PessoaJuridicaDto> ExecuteAsync(int id, UpdatePessoaJuridicaCommand command);
    }
}