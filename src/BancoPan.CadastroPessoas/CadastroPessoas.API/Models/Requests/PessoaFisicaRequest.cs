using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.API.Models.Requests
{
    /// <summary>
    /// Modelo de requisição para criação ou atualização de pessoa física.
    /// </summary>
    /// <remarks>
    /// Este modelo é utilizado nas operações de POST e PUT do controlador de pessoas físicas.
    /// Todos os campos marcados como obrigatórios devem ser preenchidos corretamente.
    /// </remarks>
    public class PessoaFisicaRequest
    {
        /// <summary>
        /// Nome completo da pessoa física.
        /// </summary>
        /// <example>João da Silva</example>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CPF da pessoa física, apenas números.
        /// </summary>
        /// <example>12345678901</example>
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [MaxLength(11, ErrorMessage = "O CPF deve ter 11 caracteres")] 
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// CEP do endereço, com ou sem hífen.
        /// </summary>
        /// <example>01001000</example>
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [MaxLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres")] 
        public string CEP { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço.
        /// </summary>
        /// <example>123</example>
        [MaxLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
        public string? Numero { get; set; } = "";

        /// <summary>
        /// Informações complementares do endereço.
        /// </summary>
        /// <example>Apto 45</example>
        [MaxLength(200, ErrorMessage = "O complemento deve ter no máximo 200 caracteres")]
        public string? Complemento { get; set; } = "";
    }
}