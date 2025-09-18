using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Interfaces;
using CadastroPessoas.Ports.Output.Services;
using System;
using System.Threading.Tasks;

namespace CadastroPessoas.Adapters.Output.ViaCep.Services
{
    /// <summary>
    /// Adaptador que implementa <see cref="IViaCepService"/> usando <see cref="IEnderecoPorCepProvider"/>.
    /// </summary>
    /// <remarks>
    /// Esta classe serve como uma ponte entre a interface de domínio legada (<see cref="IViaCepService"/>)
    /// e a nova interface de porta de saída da arquitetura hexagonal (<see cref="IEnderecoPorCepProvider"/>).
    /// 
    /// O padrão Adapter é utilizado para permitir a migração gradual para a arquitetura hexagonal,
    /// mantendo a compatibilidade com o código existente que depende da interface <see cref="IViaCepService"/>,
    /// enquanto internamente utiliza a nova implementação baseada em portas e adaptadores.
    /// </remarks>
    public class ViaCepServiceAdapter : IViaCepService
    {
        private readonly IEnderecoPorCepProvider _enderecoPorCepProvider;

        /// <summary>
        /// Inicializa uma nova instância do adaptador <see cref="ViaCepServiceAdapter"/>.
        /// </summary>
        /// <param name="enderecoPorCepProvider">Provedor de endereços por CEP da nova arquitetura.</param>
        /// <exception cref="ArgumentNullException">Lançada quando o parâmetro <paramref name="enderecoPorCepProvider"/> é nulo.</exception>
        public ViaCepServiceAdapter(IEnderecoPorCepProvider enderecoPorCepProvider)
        {
            _enderecoPorCepProvider = enderecoPorCepProvider ?? throw new ArgumentNullException(nameof(enderecoPorCepProvider));
        }

        /// <summary>
        /// Consulta um endereço pelo CEP informado.
        /// </summary>
        /// <param name="cep">CEP a ser consultado (formato com ou sem hífen).</param>
        /// <returns>
        /// Objeto <see cref="Endereco"/> contendo os dados do endereço, ou null se o CEP não for encontrado.
        /// </returns>
        /// <remarks>
        /// Este método simplesmente delega a chamada para o provedor de endereços subjacente,
        /// realizando a adaptação entre as interfaces sem modificar os dados.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Lançada quando o CEP é nulo, vazio ou inválido (propagada do provedor).
        /// </exception>
        /// <exception cref="Exception">
        /// Lançada quando ocorre um erro na consulta do CEP (propagada do provedor).
        /// </exception>
        public async Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep)
        {
            return await _enderecoPorCepProvider.ConsultarEnderecoPorCepAsync(cep);
        }
    }
}