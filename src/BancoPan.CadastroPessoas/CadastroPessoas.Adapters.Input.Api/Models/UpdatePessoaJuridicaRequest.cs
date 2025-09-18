using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    /// <summary>
    /// Modelo de requisição para atualização de pessoa jurídica
    /// </summary>
    /// <remarks>
    /// Este modelo permite a atualização parcial dos dados de uma pessoa jurídica.
    /// Apenas os campos preenchidos serão atualizados, permitindo modificar somente 
    /// as informações necessárias. O CNPJ não pode ser alterado e, portanto, não está 
    /// presente neste modelo.
    /// </remarks>
    public class UpdatePessoaJuridicaRequest
    {
        /// <summary>
        /// Nova razão social da empresa (opcional)
        /// </summary>
        /// <example>Empresa XYZ Comércio e Serviços Ltda.</example>
        /// <remarks>Se não informado, a razão social atual será mantida</remarks>
        [StringLength(200, ErrorMessage = "A razão social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Novo nome fantasia da empresa (opcional)
        /// </summary>
        /// <example>XYZ Solutions</example>
        /// <remarks>Se não informado, o nome fantasia atual será mantido</remarks>
        [StringLength(200, ErrorMessage = "O nome fantasia deve ter no máximo 200 caracteres")]
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// Novo CEP para atualização do endereço (opcional)
        /// </summary>
        /// <example>04711-130</example>
        /// <remarks>
        /// Se informado, o endereço completo será atualizado com base neste CEP.
        /// Pode ser informado com ou sem hífen (ex: 04711-130 ou 04711130).
        /// Se não informado, o CEP e endereço atuais serão mantidos.
        /// </remarks>
        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres")]
        public string CEP { get; set; } = string.Empty;

        /// <summary>
        /// Novo número do endereço (opcional)
        /// </summary>
        /// <example>1500</example>
        /// <remarks>Se não informado, o número atual será mantido</remarks>
        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Novo complemento do endereço (opcional)
        /// </summary>
        /// <example>Torre B, 15º andar</example>
        /// <remarks>Se não informado, o complemento atual será mantido</remarks>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; } = string.Empty;
    }
}