using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface ICreatePessoaJuridicaUseCase
    {
        Task<PessoaJuridicaDto> ExecuteAsync(CreatePessoaJuridicaCommand command);
    }
}