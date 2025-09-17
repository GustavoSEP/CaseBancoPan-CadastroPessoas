namespace CadastroPessoas.Ports.Input.Commands
{
    public class UpdatePessoaFisicaCommand
    {
        public string Nome { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
    }
}