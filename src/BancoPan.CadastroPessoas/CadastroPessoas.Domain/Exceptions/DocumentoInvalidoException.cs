using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Exceptions
{
    public class DocumentoInvalidoException : DomainException
    {
        public DocumentoInvalidoException(string message) : base(message)
        {
        }
    }
}

