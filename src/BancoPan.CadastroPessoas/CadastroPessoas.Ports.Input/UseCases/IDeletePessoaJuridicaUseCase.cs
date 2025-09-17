using System.Threading.Tasks;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface IDeletePessoaJuridicaUseCase
    {
        Task<bool> ExecuteAsync(int id);
    }
}