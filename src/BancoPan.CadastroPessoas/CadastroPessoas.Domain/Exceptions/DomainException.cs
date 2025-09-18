using System;

namespace CadastroPessoas.Domain.Exceptions
{
    /// <summary>
    /// Exceção base para todas as exceções relacionadas a regras de domínio da aplicação.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Esta classe abstrai o conceito de erro de domínio, permitindo distinguir entre erros
    /// de regras de negócio (domínio) e erros técnicos ou de infraestrutura. Seguindo os
    /// princípios do Domain-Driven Design (DDD), as exceções de domínio representam violações
    /// das invariantes e regras do modelo de negócio.
    /// </para>
    /// <para>
    /// Todas as exceções específicas de domínio devem herdar desta classe base para facilitar
    /// o tratamento uniforme de erros relacionados às regras de negócio.
    /// </para>
    /// <para>
    /// Na arquitetura hexagonal, estas exceções podem ser lançadas na camada de domínio e
    /// tratadas apropriadamente nos adaptadores de entrada para retornar respostas adequadas
    /// ao cliente.
    /// </para>
    /// </remarks>
    /// <example>
    /// Exemplo de criação de uma exceção especializada:
    /// <code>
    /// public class CpfInvalidoException : DomainException
    /// {
    ///     public CpfInvalidoException(string cpf) 
    ///         : base($"O CPF '{cpf}' é inválido.")
    ///     {
    ///         Cpf = cpf;
    ///     }
    ///     
    ///     public string Cpf { get; }
    /// }
    /// </code>
    /// </example>
    public class DomainException : Exception
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem de erro específica.
        /// </summary>
        /// <param name="message">Mensagem que descreve o erro de domínio.</param>
        public DomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DomainException"/> com uma mensagem de erro específica
        /// e uma exceção interna que é a causa do erro atual.
        /// </summary>
        /// <param name="message">Mensagem que descreve o erro de domínio.</param>
        /// <param name="innerException">Exceção que é a causa da exceção atual.</param>
        /// <remarks>
        /// Este construtor é útil quando um erro de domínio ocorre como resultado de outra exceção.
        /// Por exemplo, quando uma exceção de infraestrutura precisa ser traduzida para um contexto de domínio.
        /// </remarks>
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}