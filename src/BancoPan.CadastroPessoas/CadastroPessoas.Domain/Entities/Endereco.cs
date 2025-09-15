using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Entities
{
    public class Endereco
    {
        public string Cep { get; set; } = "";
        public string Logradouro { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Numero { get; set; } = "";
        public string Complemento { get; set; } = "";

        public Endereco() { }

        public Endereco(string cep, string logradouro, string bairro, string cidade, string estado)
            : this(cep, logradouro, bairro, cidade, estado, string.Empty, string.Empty)
        { }

        public Endereco(string cep, string logradouro, string bairro, string cidade, string estado, string numero, string complemento)
        {
            Cep = cep ?? string.Empty;
            Logradouro = logradouro ?? string.Empty;
            Bairro = bairro ?? string.Empty;
            Cidade = cidade ?? string.Empty;
            Estado = estado ?? string.Empty;
            Numero = numero ?? string.Empty;
            Complemento = complemento ?? string.Empty;
        }

        public void AtualizarNumeroComplemento(string numero, string complemento)
        {
            if (!string.IsNullOrWhiteSpace(numero))
                Numero = numero;
            if (!string.IsNullOrWhiteSpace(complemento))
                Complemento = complemento;
        }
    }
}
