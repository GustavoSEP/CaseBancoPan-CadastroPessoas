using System.Threading.Tasks;
using CadastroPessoas.Ports.Input.Commands;
using CadastroPessoas.Ports.Input.Dtos;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface ICreatePessoaFisicaUseCase
    {
        Task<PessoaFisicaDto> ExecuteAsync(CreatePessoaFisicaCommand command);
    }
}