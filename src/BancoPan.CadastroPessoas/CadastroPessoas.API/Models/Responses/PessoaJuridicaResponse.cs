namespace CadastroPessoas.API.Models.Responses
{
    /// <summary>
    /// Modelo de resposta para pessoa jurídica.
    /// </summary>
    /// <remarks>
    /// Este modelo é retornado pelos endpoints da API de pessoas jurídicas nas operações
    /// de criação, consulta e listagem. Contém todos os dados cadastrais da empresa.
    /// </remarks>
    public class PessoaJuridicaResponse
    {
        /// <summary>
        /// Identificador único da pessoa jurídica no sistema.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Razão social da empresa, nome oficial registrado na Junta Comercial.
        /// </summary>
        /// <example>Empresa ABC Ltda</example>
        public string? RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Nome fantasia da empresa, nome comercial pelo qual a empresa é conhecida.
        /// </summary>
        /// <example>ABC Sistemas</example>
        public string? NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// Número do CNPJ da empresa, no formato com pontuação.
        /// </summary>
        /// <example>12.345.678/0001-90</example>
        public string Documento { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de pessoa no sistema, para pessoa jurídica será sempre "J".
        /// </summary>
        /// <example>J</example>
        public string TipoPessoa { get; set; } = string.Empty;

        /// <summary>
        /// Dados completos do endereço da empresa.
        /// </summary>
        public EnderecoDto Endereco { get; set; } = new EnderecoDto();
    }
}