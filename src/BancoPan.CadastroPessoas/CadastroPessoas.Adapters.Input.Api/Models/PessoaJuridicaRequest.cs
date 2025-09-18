using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    /// <summary>
    /// Modelo de requisição para cadastro e atualização de pessoa jurídica
    /// </summary>
    /// <remarks>
    /// Este modelo contém os dados necessários para criar ou atualizar uma pessoa jurídica no sistema.
    /// O endereço será complementado automaticamente a partir do CEP informado.
    /// </remarks>
    public class PessoaJuridicaRequest
    {
        /// <summary>
        /// Razão social da empresa
        /// </summary>
        /// <example>Empresa XYZ Ltda.</example>
        [Required(ErrorMessage = "A razão social é obrigatória")]
        [StringLength(200, ErrorMessage = "A razão social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Nome fantasia da empresa (opcional)
        /// </summary>
        /// <example>XYZ Tecnologia</example>
        [StringLength(200, ErrorMessage = "O nome fantasia deve ter no máximo 200 caracteres")]
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da empresa (com ou sem formatação)
        /// </summary>
        /// <example>12.345.678/0001-90</example>
        /// <remarks>Pode ser informado com ou sem pontuação (ex: 12.345.678/0001-90 ou 12345678000190)</remarks>
        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres")]
        public string CNPJ { get; set; } = string.Empty;

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
        /// <example>Andar 10, Sala 1001</example>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; } = string.Empty;
    }
}