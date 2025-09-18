namespace CadastroPessoas.Ports.Input.Dtos
{
    /// <summary>
    /// Representa um objeto de transferência de dados (DTO) para endereços.
    /// Esta classe é utilizada para transferir informações de endereço entre as camadas da aplicação,
    /// principalmente para entrada de dados através das portas de entrada (input ports).
    /// </summary>
    /// <remarks>
    /// Este DTO é utilizado para:
    /// - Receber dados de endereço em requisições de API
    /// - Transferir informações de endereço entre a camada de aplicação e a camada de domínio
    /// - Desacoplar a representação externa do endereço da entidade de domínio Endereco
    /// </remarks>
    public class EnderecoDto
    {
        /// <summary>
        /// Obtém ou define o CEP (Código de Endereçamento Postal) do endereço.
        /// </summary>
        /// <example>01001-000</example>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o logradouro (rua, avenida, etc.) do endereço.
        /// </summary>
        /// <example>Avenida Paulista</example>
        public string Logradouro { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o bairro do endereço.
        /// </summary>
        /// <example>Bela Vista</example>
        public string Bairro { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define a cidade do endereço.
        /// </summary>
        /// <example>São Paulo</example>
        public string Cidade { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o estado (unidade federativa) do endereço.
        /// </summary>
        /// <example>SP</example>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o número do endereço.
        /// </summary>
        /// <example>1000</example>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define informações complementares do endereço, quando aplicável.
        /// </summary>
        /// <example>Apto 42, Bloco B</example>
        public string Complemento { get; set; } = string.Empty;
    }
}