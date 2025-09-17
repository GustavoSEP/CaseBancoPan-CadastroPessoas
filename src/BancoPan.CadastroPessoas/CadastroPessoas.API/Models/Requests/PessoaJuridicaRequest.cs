using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.API.Models.Requests
{
    /// <summary>
    /// Modelo de requisição para criação ou atualização de pessoa jurídica.
    /// </summary>
    /// <remarks>
    /// Este modelo é utilizado nas operações de POST e PUT do controlador de pessoas jurídicas.
    /// Contém os dados necessários para o cadastro ou alteração de uma empresa no sistema.
    /// </remarks>
    public class PessoaJuridicaRequest
    {
        /// <summary>
        /// Razão social da empresa. Nome oficial registrado na Junta Comercial.
        /// </summary>
        /// <example>Empresa ABC Ltda</example>
        [Required(ErrorMessage = "A razão social é obrigatória")]
        [MaxLength(200, ErrorMessage = "A razão social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Nome fantasia da empresa. Nome comercial pelo qual a empresa é conhecida.
        /// </summary>
        /// <example>ABC Sistemas</example>
        [MaxLength(200, ErrorMessage = "O nome fantasia deve ter no máximo 200 caracteres")]
        public string? NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da empresa, apenas números.
        /// </summary>
        /// <example>12345678000190</example>
        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [MaxLength(14, ErrorMessage = "O CNPJ deve ter 14 caracteres")] 
        public string CNPJ { get; set; } = string.Empty;

        /// <summary>
        /// CEP do endereço da empresa, com ou sem hífen.
        /// </summary>
        /// <example>01001000</example>
        [Required(ErrorMessage = "O CEP é obrigatório")]
        [MaxLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres")]
        public string CEP { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço da empresa.
        /// </summary>
        /// <example>1000</example>
        [MaxLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
        public string? Numero { get; set; } = "";

        /// <summary>
        /// Informações complementares do endereço da empresa.
        /// </summary>
        /// <example>Andar 10, Sala 1015</example>
        [MaxLength(200, ErrorMessage = "O complemento deve ter no máximo 200 caracteres")]
        public string? Complemento { get; set; } = "";
    }
}