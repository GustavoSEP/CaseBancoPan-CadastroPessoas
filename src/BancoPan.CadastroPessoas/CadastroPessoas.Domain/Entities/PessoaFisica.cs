using System;

namespace CadastroPessoas.Domain.Entities
{
    /// <summary>
    /// Representa uma pessoa física no domínio da aplicação.
    /// </summary>
    /// <remarks>
    /// Esta entidade armazena informações de pessoas físicas, incluindo seus dados pessoais
    /// e endereço completo. Implementa o padrão de entidade de domínio com identificador único
    /// e propriedades imutáveis para garantir a integridade dos dados.
    /// 
    /// O tipo da pessoa é sempre "F" para pessoa física, conforme padrão do sistema.
    /// </remarks>
    public class PessoaFisica
    {
        /// <summary>
        /// Obtém o identificador único da pessoa física.
        /// </summary>
        /// <remarks>
        /// Este ID é gerado pelo banco de dados quando o registro é persistido.
        /// Para novas instâncias que ainda não foram persistidas, o valor padrão é 0.
        /// </remarks>
        public int Id { get; private set; }

        /// <summary>
        /// Obtém o nome completo da pessoa física.
        /// </summary>
        /// <example>João da Silva Santos</example>
        public string Nome { get; private set; }

        /// <summary>
        /// Obtém o CPF (Cadastro de Pessoa Física) formatado.
        /// </summary>
        /// <example>123.456.789-10</example>
        /// <remarks>
        /// O CPF é armazenado no formato XXX.XXX.XXX-XX e é utilizado como identificador
        /// natural da pessoa física, devendo ser único no sistema.
        /// </remarks>
        public string CPF { get; private set; }

        /// <summary>
        /// Obtém o tipo da pessoa, que para pessoa física é sempre "F".
        /// </summary>
        /// <example>F</example>
        /// <remarks>
        /// Este campo é utilizado para diferenciar pessoas físicas de jurídicas
        /// em consultas que envolvem ambos os tipos.
        /// </remarks>
        public string Tipo { get; private set; }

        /// <summary>
        /// Obtém o endereço completo da pessoa física.
        /// </summary>
        /// <remarks>
        /// Contém todos os dados de endereçamento, incluindo CEP, logradouro, número,
        /// complemento, bairro, cidade e estado.
        /// </remarks>
        public Endereco Endereco { get; private set; }

        /// <summary>
        /// Obtém a data e hora de cadastro da pessoa física no sistema.
        /// </summary>
        /// <example>2025-09-18T10:30:00</example>
        /// <remarks>
        /// Este valor é definido automaticamente no momento da criação da instância
        /// e não pode ser alterado posteriormente.
        /// </remarks>
        public DateTime DataCadastro { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaFisica"/> com todos os dados necessários.
        /// </summary>
        /// <param name="id">Identificador único (0 para novas pessoas).</param>
        /// <param name="nome">Nome completo da pessoa física.</param>
        /// <param name="cpf">CPF formatado da pessoa física.</param>
        /// <param name="tipo">Tipo da pessoa (deve ser "F" para pessoa física).</param>
        /// <param name="endereco">Endereço completo da pessoa física.</param>
        /// <exception cref="ArgumentNullException">
        /// Lançada quando nome, cpf, tipo ou endereco são nulos.
        /// </exception>
        /// <remarks>
        /// Este construtor cria uma nova instância com os dados fornecidos e define
        /// automaticamente a data de cadastro como a data e hora atual.
        /// </remarks>
        public PessoaFisica(int id, string nome, string cpf, string tipo, Endereco endereco)
        {
            Id = id;
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            CPF = cpf ?? throw new ArgumentNullException(nameof(cpf));
            Tipo = tipo ?? throw new ArgumentNullException(nameof(tipo));
            Endereco = endereco ?? throw new ArgumentNullException(nameof(endereco));
            DataCadastro = DateTime.Now;
        }

        /// <summary>
        /// Construtor protegido sem parâmetros para uso do Entity Framework Core.
        /// </summary>
        /// <remarks>
        /// Este construtor é necessário para que o Entity Framework Core possa criar
        /// instâncias da entidade durante a leitura do banco de dados. Não deve ser
        /// utilizado diretamente no código da aplicação.
        /// </remarks>
        protected PessoaFisica() { }
    }
}