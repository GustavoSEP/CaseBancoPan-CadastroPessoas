using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    /// <summary>
    /// Modelo de requisição para cadastro e atualização de pessoa física
    /// </summary>
    /// <remarks>
    /// Este modelo contém os dados necessários para criar ou atualizar uma pessoa física no sistema.
    /// O endereço será complementado automaticamente a partir do CEP informado.
    /// </remarks>
    public class PessoaFisicaRequest
    {
        /// <summary>
        /// Nome completo da pessoa física
        /// </summary>
        /// <example>João da Silva</example>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CPF da pessoa física (com ou sem formatação)
        /// </summary>
        /// <example>123.456.789-10</example>
        /// <remarks>Pode ser informado com ou sem pontuação (ex: 123.456.789-10 ou 12345678910)</remarks>
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres")]
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// CEP para consulta do endereço
        /// </summary>
        /// <example>01310-100</example>
        /// <remarks>Pode ser informado com ou sem hífen (ex: 01310-100 ou 01310100)</remarks>
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres")]
        public string CEP { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço
        /// </summary>
        /// <example>1000</example>
        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Complemento do endereço (opcional)
        /// </summary>
        /// <example>Apto 123</example>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; } = string.Empty;
    }
}