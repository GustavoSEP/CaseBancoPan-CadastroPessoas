using System.Threading.Tasks;

namespace CadastroPessoas.Ports.Input.UseCases
{
    public interface IDeletePessoaFisicaUseCase
    {
        Task<bool> ExecuteAsync(int id);
    }
}