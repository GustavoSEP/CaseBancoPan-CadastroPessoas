using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Exceptions
{
    public class PessoaJaExisteException : DomainException
    {
        public PessoaJaExisteException(string documento)
            : base($"Pessoa com documento '{documento}' já existe.")
        {
            Documento = documento;
        }

        public string Documento { get; }
    }
}
