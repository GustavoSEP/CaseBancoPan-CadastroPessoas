namespace CadastroPessoas.Adapters.Input.Api.Models
{
    /// <summary>
    /// Modelo de resposta para operações com pessoa física
    /// </summary>
    /// <remarks>
    /// Este modelo é retornado nas operações de criação, consulta e atualização de pessoas físicas.
    /// Contém todos os dados cadastrais da pessoa física, incluindo seu endereço completo.
    /// </remarks>
    public class PessoaFisicaResponse
    {
        /// <summary>
        /// Identificador único da pessoa física
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo da pessoa física
        /// </summary>
        /// <example>João da Silva</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CPF da pessoa física (formatado)
        /// </summary>
        /// <example>123.456.789-10</example>
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// Dados de endereço da pessoa física
        /// </summary>
        public EnderecoResponse Endereco { get; set; } = new EnderecoResponse();
    }

    /// <summary>
    /// Modelo de resposta para dados de endereço
    /// </summary>
    /// <remarks>
    /// Este modelo contém as informações completas de um endereço, 
    /// incluindo os dados obtidos automaticamente via CEP e os dados 
    /// complementares fornecidos pelo usuário (número e complemento).
    /// </remarks>
    public class EnderecoResponse
    {
        /// <summary>
        /// CEP do endereço (formato 00000-000)
        /// </summary>
        /// <example>01310-100</example>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Nome da rua, avenida, praça, etc.
        /// </summary>
        /// <example>Avenida Paulista</example>
        public string Logradouro { get; set; } = string.Empty;

        /// <summary>
        /// Bairro do endereço
        /// </summary>
        /// <example>Bela Vista</example>
        public string Bairro { get; set; } = string.Empty;

        /// <summary>
        /// Cidade do endereço
        /// </summary>
        /// <example>São Paulo</example>
        public string Cidade { get; set; } = string.Empty;

        /// <summary>
        /// Estado do endereço (UF)
        /// </summary>
        /// <example>SP</example>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço
        /// </summary>
        /// <example>1000</example>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endereço (apartamento, sala, etc.)
        /// </summary>
        /// <example>Apto 123</example>
        public string Complemento { get; set; } = string.Empty;
    }
}