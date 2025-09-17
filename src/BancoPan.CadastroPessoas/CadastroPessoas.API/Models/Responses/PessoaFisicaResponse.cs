namespace CadastroPessoas.API.Models.Responses
{
    /// <summary>
    /// Modelo de resposta para pessoa física.
    /// </summary>
    /// <remarks>
    /// Este modelo é retornado pelos endpoints da API de pessoas físicas nas operações
    /// de criação, consulta e listagem. Contém todos os dados cadastrais da pessoa física.
    /// </remarks>
    public class PessoaFisicaResponse
    {
        /// <summary>
        /// Identificador único da pessoa física no sistema.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo da pessoa física.
        /// </summary>
        /// <example>João da Silva</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Número do CPF da pessoa física, no formato com pontuação.
        /// </summary>
        /// <example>123.456.789-01</example>
        public string Documento { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de pessoa no sistema, para pessoa física será sempre "F".
        /// </summary>
        /// <example>F</example>
        public string TipoPessoa { get; set; } = string.Empty;

        /// <summary>
        /// Dados completos do endereço da pessoa física.
        /// </summary>
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
    }
}