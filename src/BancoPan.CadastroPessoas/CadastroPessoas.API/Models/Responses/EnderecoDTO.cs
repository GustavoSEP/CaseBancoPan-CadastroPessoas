namespace CadastroPessoas.API.Models.Responses
{
    /// <summary>
    /// Modelo de resposta para dados de endereço.
    /// </summary>
    /// <remarks>
    /// Este modelo é utilizado nas respostas da API para representar dados de endereço.
    /// É retornado como parte das respostas dos endpoints de pessoas físicas e jurídicas.
    /// </remarks>
    public class EnderecoDto
    {
        /// <summary>
        /// CEP (Código de Endereçamento Postal) do endereço.
        /// </summary>
        /// <example>01001-000</example>
        public string Cep { get; set; } = "";

        /// <summary>
        /// Nome da rua, avenida, praça ou outro tipo de logradouro.
        /// </summary>
        /// <example>Praça da Sé</example>
        public string Logradouro { get; set; } = "";

        /// <summary>
        /// Bairro ou região do endereço.
        /// </summary>
        /// <example>Sé</example>
        public string Bairro { get; set; } = "";

        /// <summary>
        /// Nome da cidade ou município.
        /// </summary>
        /// <example>São Paulo</example>
        public string Cidade { get; set; } = "";

        /// <summary>
        /// Sigla do estado ou unidade federativa.
        /// </summary>
        /// <example>SP</example>
        public string Estado { get; set; } = "";

        /// <summary>
        /// Número do endereço.
        /// </summary>
        /// <example>123</example>
        public string Numero { get; set; } = "";

        /// <summary>
        /// Informações adicionais do endereço, como bloco, apartamento, andar, etc.
        /// </summary>
        /// <example>Apto 45, Bloco B</example>
        public string Complemento { get; set; } = "";
    }
}