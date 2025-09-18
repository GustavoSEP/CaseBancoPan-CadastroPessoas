using System.Threading.Tasks;
using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Ports.Output.Services
{
    /// <summary>
    /// Interface que define um provedor de consulta de endereços a partir de um CEP (Código de Endereçamento Postal).
    /// </summary>
    /// <remarks>
    /// Esta interface segue os princípios da arquitetura hexagonal, atuando como uma porta de saída (output port)
    /// que permite ao domínio interagir com serviços externos de consulta de CEP sem depender de sua implementação.
    /// 
    /// Implementações desta interface podem se comunicar com diferentes serviços externos como:
    /// - ViaCEP
    /// - BrasilAPI
    /// - Correios
    /// - Entre outros serviços de consulta de CEP
    /// 
    /// O uso desta interface permite que a aplicação obtenha informações de endereço automaticamente
    /// a partir do CEP informado, melhorando a experiência do usuário e reduzindo erros de digitação.
    /// </remarks>
    public interface IEnderecoPorCepProvider
    {
        /// <summary>
        /// Consulta um endereço a partir do CEP informado.
        /// </summary>
        /// <param name="cep">O CEP (Código de Endereçamento Postal) a ser consultado.</param>
        /// <returns>
        /// Um objeto <see cref="Task{TResult}"/> que representa a operação assíncrona,
        /// contendo uma entidade <see cref="Endereco"/> com as informações do endereço correspondente ao CEP,
        /// ou <c>null</c> se o CEP não for encontrado ou for inválido.
        /// </returns>
        /// <remarks>
        /// O CEP pode ser informado com ou sem formatação (com ou sem hífen).
        /// O método deve ser capaz de tratar ambos os formatos.
        /// 
        /// Exemplo de CEP formatado: "01001-000"
        /// Exemplo de CEP não formatado: "01001000"
        /// </remarks>
        /// <exception cref="System.Net.Http.HttpRequestException">
        /// Lançada quando ocorre um erro na comunicação com o serviço externo.
        /// </exception>
        /// <exception cref="System.TimeoutException">
        /// Lançada quando a consulta ao serviço externo excede o tempo limite.
        /// </exception>
        Task<Endereco?> ConsultarEnderecoPorCepAsync(string cep);
    }
}