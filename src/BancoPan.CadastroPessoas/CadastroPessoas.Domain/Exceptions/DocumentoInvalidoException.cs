using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Exceptions
{
    /// <summary>
    /// Exceção de domínio lançada quando um documento (CPF ou CNPJ) é inválido.
    /// </summary>
    /// <remarks>
    /// Esta exceção é especializada para tratamento de erros relacionados à validação 
    /// de documentos de identificação brasileiros, como CPF e CNPJ. Deve ser utilizada 
    /// quando um documento não atende aos critérios de formato ou validação de dígitos 
    /// verificadores.
    /// 
    /// Por herdar de <see cref="DomainException"/>, esta exceção é considerada parte 
    /// da camada de domínio e representa uma violação de regra de negócio, não um 
    /// erro técnico ou de infraestrutura.
    /// </remarks>
    /// <example>
    /// Exemplo de uso:
    /// <code>
    /// if (!ValidadorCpf.EhValido(cpf))
    /// {
    ///     throw new DocumentoInvalidoException($"O CPF {cpf} não é válido.");
    /// }
    /// </code>
    /// </example>
    public class DocumentoInvalidoException : DomainException
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DocumentoInvalidoException"/> com uma mensagem de erro específica.
        /// </summary>
        /// <param name="message">Mensagem que descreve o erro.</param>
        /// <remarks>
        /// A mensagem deve ser clara e específica, indicando qual documento é inválido e, 
        /// se possível, o motivo da invalidez (formato incorreto, dígitos verificadores 
        /// inválidos, etc.).
        /// </remarks>
        public DocumentoInvalidoException(string message) : base(message)
        {
        }
    }
}