using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Ports.Output.Services;
using System;
using System.Threading.Tasks;

namespace CadastroPessoas.Adapters.Output.ViaCep.Services
{
    /// <summary>
    /// Adaptador que implementa IViaCepService usando IEnderecoPorCepProvider
    /// Essa classe serve como ponte entre a nova arquitetura hexagonal e o código legado
    /// </summary>
    public class ViaCepServiceAdapter : IViaCepService
    {
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;

        public ViaCepServiceAdapter(IEnderecoPorCepProvider enderecoPorCepProvider)
        {
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
        }

        public async Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep)
        {
            return await _enderecoPorCepProvider.ConsultarEnderecoPorCepAsync(cep);
        }
    }
}