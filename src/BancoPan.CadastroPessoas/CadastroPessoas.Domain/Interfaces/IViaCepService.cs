using CadastroPessoas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Interfaces
{
    /// <summary>
    /// Interface para o serviço de consulta de CEP através da API ViaCEP.
    /// Permite obter informações de endereço a partir de um CEP.
    /// </summary>
    /// <remarks>
    /// Esta interface abstrai a comunicação com a API pública ViaCEP (https://viacep.com.br/),
    /// que fornece dados de endereços para CEPs válidos do Brasil.
    /// </remarks>
    public interface IViaCepService
    {
        /// <summary>
        /// Consulta um endereço completo a partir de um CEP.
        /// </summary>
        /// <param name="cep">O CEP a ser consultado (formato: 00000-000 ou 00000000).</param>
        /// <returns>
        /// Um objeto <see cref="Endereco"/> contendo os dados do endereço correspondente ao CEP informado,
        /// ou null caso o CEP seja inválido ou não seja encontrado.
        /// </returns>
        /// <exception cref="ArgumentException">Lançada quando o CEP fornecido está em formato inválido.</exception>
        /// <exception cref="HttpRequestException">Lançada quando ocorre um erro na comunicação com a API ViaCEP.</exception>
        /// <exception cref="TimeoutException">Lançada quando a requisição para a API ViaCEP excede o tempo limite.</exception>
        /// <remarks>
        /// O objeto Endereco retornado não inclui os campos de número e complemento, que devem ser
        /// fornecidos adicionalmente pelo usuário.
        /// </remarks>
        Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep);
    }
}