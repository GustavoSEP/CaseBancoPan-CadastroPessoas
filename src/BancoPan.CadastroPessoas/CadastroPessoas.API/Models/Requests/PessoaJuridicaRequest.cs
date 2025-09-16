using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.API.Models.Requests
{
    public class PessoaJuridicaRequest
    {
        [Required]
        [MaxLength(200)]
        public string RazaoSocial { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? NomeFantasia { get; set; } = string.Empty;

        [Required]
        [MaxLength(14)] // Limitado a quantidade de caracteres considerando somente os numeros.
        public string CNPJ { get; set; } = string.Empty;

        [Required]
        [MaxLength(9)]
        public string CEP { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Numero { get; set; } = "";

        [MaxLength(200)]
        public string? Complemento { get; set; } = "";
    }
}
