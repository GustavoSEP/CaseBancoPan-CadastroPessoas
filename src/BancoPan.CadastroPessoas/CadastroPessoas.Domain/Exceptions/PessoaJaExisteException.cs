using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Exceptions
{
    /// <summary>
    /// Exceção de domínio lançada quando se tenta cadastrar uma pessoa com um documento (CPF ou CNPJ) que já existe no sistema.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Esta exceção é especializada para tratar violações de unicidade de documentos de identificação.
    /// Representa uma regra de negócio que estabelece que um CPF ou CNPJ só pode estar associado a uma 
    /// única pessoa no sistema.
    /// </para>
    /// <para>
    /// Além da mensagem de erro, esta exceção armazena o documento específico que causou o conflito,
    /// permitindo que as camadas superiores acessem esta informação para tratamento adequado do erro.
    /// </para>
    /// <para>
    /// Por herdar de <see cref="DomainException"/>, esta exceção é considerada parte da camada de 
    /// domínio e representa uma violação de regra de negócio.
    /// </para>
    /// </remarks>
    /// <example>
    /// Exemplo de uso:
    /// <code>
    /// if (await _repository.ExistsPessoaFisicaByCpfAsync(cpf))
    /// {
    ///     throw new PessoaJaExisteException(cpf);
    /// }
    /// </code>
    /// </example>
    public class PessoaJaExisteException : DomainException
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaJaExisteException"/> com o documento que causou o conflito.
        /// </summary>
        /// <param name="documento">O documento (CPF ou CNPJ) que já está cadastrado no sistema.</param>
        /// <remarks>
        /// O construtor gera automaticamente uma mensagem de erro que inclui o documento,
        /// e armazena o documento para que possa ser acessado posteriormente.
        /// </remarks>
        public PessoaJaExisteException(string documento)
            : base($"Pessoa com documento '{documento}' já existe.")
        {
            Documento = documento;
        }

        /// <summary>
        /// Obtém o documento (CPF ou CNPJ) que já está cadastrado no sistema.
        /// </summary>
        /// <remarks>
        /// Esta propriedade pode ser útil para camadas superiores que precisam acessar o documento
        /// específico que causou o conflito, para exibição em mensagens de erro ou logs.
        /// </remarks>
        /// <example>
        /// <code>
        /// catch (PessoaJaExisteException ex)
        /// {
        ///     _logger.LogWarning("Tentativa de cadastro duplicado. Documento: {Documento}", ex.Documento);
        ///     return Conflict($"Já existe um cadastro com o documento {ex.Documento}");
        /// }
        /// </code>
        /// </example>
        public string Documento { get; }
    }
}