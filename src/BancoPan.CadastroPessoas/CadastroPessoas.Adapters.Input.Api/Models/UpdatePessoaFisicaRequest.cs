using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    /// <summary>
    /// Modelo de requisição para atualização de pessoa física
    /// </summary>
    /// <remarks>
    /// Este modelo permite a atualização parcial dos dados de uma pessoa física.
    /// Apenas os campos preenchidos serão atualizados, permitindo modificar somente 
    /// as informações necessárias. O CPF não pode ser alterado e, portanto, não está 
    /// presente neste modelo.
    /// </remarks>
    public class UpdatePessoaFisicaRequest
    {
        /// <summary>
        /// Novo nome da pessoa física (opcional)
        /// </summary>
        /// <example>João da Silva Santos</example>
        /// <remarks>Se não informado, o nome atual será mantido</remarks>
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Novo CEP para atualização do endereço (opcional)
        /// </summary>
        /// <example>04538-132</example>
        /// <remarks>
        /// Se informado, o endereço completo será atualizado com base neste CEP.
        /// Pode ser informado com ou sem hífen (ex: 04538-132 ou 04538132).
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
        /// <example>Bloco B, Apto 45</example>
        /// <remarks>Se não informado, o complemento atual será mantido</remarks>
        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; } = string.Empty;
    }
}