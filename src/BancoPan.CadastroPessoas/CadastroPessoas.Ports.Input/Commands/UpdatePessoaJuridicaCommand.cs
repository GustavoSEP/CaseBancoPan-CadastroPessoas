namespace CadastroPessoas.Ports.Input.Commands
{
    public class UpdatePessoaJuridicaCommand
    {
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
    }
}