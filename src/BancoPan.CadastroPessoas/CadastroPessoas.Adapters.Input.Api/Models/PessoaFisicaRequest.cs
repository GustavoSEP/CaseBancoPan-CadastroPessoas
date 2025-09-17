using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Adapters.Input.Api.Models
{
    public class PessoaFisicaRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "O CPF deve ter no máximo 14 caracteres")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres")]
        public string CEP { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "O número deve ter no máximo 10 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; } = string.Empty;
    }
}