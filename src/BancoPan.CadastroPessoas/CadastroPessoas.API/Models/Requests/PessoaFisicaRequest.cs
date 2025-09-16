using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.API.Models.Requests
{
    public class PessoaFisicaRequest
    {
        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(11)] // Limitei em 11 caracteres para considerar apenas números do CPF. #Todo: Adicionar validação de CPF no futuro.
        public string CPF { get; set; } = string.Empty;

        [Required]
        [MaxLength(9)] //Limitado a 9 caracteres, considerando os 8 do CEP + o hífen. #Todo: Adicionar tratamento do CEP, validar se está no formato correto.
        public string CEP { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Numero { get; set; } = "";

        [MaxLength(200)]
        public string? Complemento { get; set; } = "";
    }
}
