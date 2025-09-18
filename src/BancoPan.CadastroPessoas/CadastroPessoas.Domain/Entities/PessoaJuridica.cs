using System;

namespace CadastroPessoas.Domain.Entities
{
    /// <summary>
    /// Representa uma pessoa jurídica (empresa) no domínio da aplicação.
    /// </summary>
    /// <remarks>
    /// Esta entidade armazena informações de pessoas jurídicas, incluindo seus dados cadastrais
    /// e endereço completo. Implementa o padrão de entidade de domínio com identificador único
    /// e propriedades imutáveis para garantir a integridade dos dados.
    /// 
    /// O tipo da pessoa é sempre "J" para pessoa jurídica, conforme padrão do sistema.
    /// </remarks>
    public class PessoaJuridica
    {
        /// <summary>
        /// Obtém o identificador único da pessoa jurídica.
        /// </summary>
        /// <remarks>
        /// Este ID é gerado pelo banco de dados quando o registro é persistido.
        /// Para novas instâncias que ainda não foram persistidas, o valor padrão é 0.
        /// </remarks>
        public int Id { get; private set; }

        /// <summary>
        /// Obtém a razão social da empresa.
        /// </summary>
        /// <example>Empresa XYZ Comércio e Serviços Ltda.</example>
        /// <remarks>
        /// A razão social é o nome oficial da empresa, conforme registrado nos órgãos competentes.
        /// </remarks>
        public string RazaoSocial { get; private set; }

        /// <summary>
        /// Obtém o nome fantasia da empresa.
        /// </summary>
        /// <example>XYZ Tecnologia</example>
        /// <remarks>
        /// O nome fantasia é o nome comercial da empresa, podendo ser diferente da razão social.
        /// Este campo é opcional e pode ser uma string vazia.
        /// </remarks>
        public string NomeFantasia { get; private set; }

        /// <summary>
        /// Obtém o CNPJ (Cadastro Nacional da Pessoa Jurídica) formatado.
        /// </summary>
        /// <example>12.345.678/0001-90</example>
        /// <remarks>
        /// O CNPJ é armazenado no formato XX.XXX.XXX/XXXX-XX e é utilizado como identificador
        /// natural da pessoa jurídica, devendo ser único no sistema.
        /// </remarks>
        public string CNPJ { get; private set; }

        /// <summary>
        /// Obtém o tipo da pessoa, que para pessoa jurídica é sempre "J".
        /// </summary>
        /// <example>J</example>
        /// <remarks>
        /// Este campo é utilizado para diferenciar pessoas jurídicas de físicas
        /// em consultas que envolvem ambos os tipos.
        /// </remarks>
        public string Tipo { get; private set; }

        /// <summary>
        /// Obtém o endereço completo da pessoa jurídica.
        /// </summary>
        /// <remarks>
        /// Contém todos os dados de endereçamento, incluindo CEP, logradouro, número,
        /// complemento, bairro, cidade e estado.
        /// </remarks>
        public Endereco Endereco { get; private set; }

        /// <summary>
        /// Obtém a data e hora de cadastro da pessoa jurídica no sistema.
        /// </summary>
        /// <example>2025-09-18T14:30:00</example>
        /// <remarks>
        /// Este valor é definido automaticamente no momento da criação da instância
        /// e não pode ser alterado posteriormente.
        /// </remarks>
        public DateTime DataCadastro { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaJuridica"/> com todos os dados necessários.
        /// </summary>
        /// <param name="id">Identificador único (0 para novas empresas).</param>
        /// <param name="razaoSocial">Razão social da empresa.</param>
        /// <param name="nomeFantasia">Nome fantasia da empresa (opcional).</param>
        /// <param name="cnpj">CNPJ formatado da empresa.</param>
        /// <param name="tipo">Tipo da pessoa (deve ser "J" para pessoa jurídica).</param>
        /// <param name="endereco">Endereço completo da empresa.</param>
        /// <exception cref="ArgumentNullException">
        /// Lançada quando razaoSocial, cnpj, tipo ou endereco são nulos.
        /// </exception>
        /// <remarks>
        /// Este construtor cria uma nova instância com os dados fornecidos e define
        /// automaticamente a data de cadastro como a data e hora atual.
        /// Note que o nome fantasia pode ser nulo ou vazio, diferente da razão social que é obrigatória.
        /// </remarks>
        public PessoaJuridica(int id, string razaoSocial, string nomeFantasia, string cnpj, string tipo, Endereco endereco)
        {
            Id = id;
            RazaoSocial = razaoSocial ?? throw new ArgumentNullException(nameof(razaoSocial));
            NomeFantasia = nomeFantasia;
            CNPJ = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
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
        protected PessoaJuridica() { }
    }
}