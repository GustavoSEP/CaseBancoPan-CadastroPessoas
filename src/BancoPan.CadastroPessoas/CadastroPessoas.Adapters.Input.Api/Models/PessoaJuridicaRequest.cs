using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    public class PessoaJuridicaRequest
    {
        [Required(ErrorMessage = "A razão social é obrigatória")]
        [StringLength(200, ErrorMessage = "A razão social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "O nome fantasia deve ter no máximo 200 caracteres")]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres")]
        public string CNPJ { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres")]
        public string CEP { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; } = string.Empty;
    }
}