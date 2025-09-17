using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Entities
{
    /// <summary>
    /// Representa uma pessoa jurídica (empresa) no sistema de cadastro.
    /// Contém informações empresariais e de endereço de uma organização.
    /// </summary>
    public class PessoaJuridica
    {
        /// <summary>
        /// Obtém ou define o identificador único da pessoa jurídica.
        /// </summary>
        /// <remarks>Chave primária gerada pelo banco de dados.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a razão social da empresa.
        /// </summary>
        /// <remarks>Nome oficial registrado na Receita Federal.</remarks>
        public string RazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o nome fantasia da empresa.
        /// </summary>
        /// <remarks>Nome comercial pelo qual a empresa é conhecida no mercado.</remarks>
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o CNPJ (Cadastro Nacional da Pessoa Jurídica) da empresa.
        /// </summary>
        /// <remarks>Formato padrão: 00.000.000/0000-00</remarks>
        public string CNPJ { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o tipo de pessoa.
        /// </summary>
        /// <remarks>Para pessoa jurídica, o valor padrão é "J".</remarks>
        public string TipoPessoa { get; set; } = "J";

        /// <summary>
        /// Obtém ou define o endereço completo da empresa.
        /// </summary>
        public Endereco Endereco { get; set; } = new Endereco();

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaJuridica"/> com valores padrão.
        /// </summary>
        public PessoaJuridica()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaJuridica"/> com os dados especificados.
        /// </summary>
        /// <param name="razaoSocial">A razão social da empresa.</param>
        /// <param name="nomeFantasia">O nome fantasia da empresa.</param>
        /// <param name="cnpj">O CNPJ da empresa (formato: 00.000.000/0000-00).</param>
        /// <param name="tipoPessoa">O tipo de pessoa (normalmente "J" para pessoa jurídica).</param>
        /// <param name="endereco">O endereço completo da empresa.</param>
        public PessoaJuridica(string razaoSocial, string nomeFantasia, string cnpj, string tipoPessoa, Endereco endereco)
        {
            RazaoSocial = razaoSocial;
            NomeFantasia = nomeFantasia;
            CNPJ = cnpj;
            TipoPessoa = tipoPessoa;
            Endereco = endereco;
        }

        /// <summary>
        /// Atualiza os dados básicos da pessoa jurídica.
        /// </summary>
        /// <param name="razaoSocial">A nova razão social da empresa.</param>
        /// <param name="nomeFantasia">O novo nome fantasia da empresa.</param>
        /// <param name="cnpj">O CNPJ da empresa (não deve ser alterado, apenas validado).</param>
        /// <param name="tipoPessoa">O tipo de pessoa (normalmente "J" para pessoa jurídica).</param>
        public void AtualizarDados(string razaoSocial, string nomeFantasia, string cnpj, string tipoPessoa)
        {
            RazaoSocial = razaoSocial;
            NomeFantasia = nomeFantasia;
            CNPJ = cnpj;
            TipoPessoa = tipoPessoa;
        }

        /// <summary>
        /// Atualiza o endereço completo da empresa.
        /// </summary>
        /// <param name="endereco">O novo endereço completo a ser atribuído.</param>
        public void AtualizarEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        /// <summary>
        /// Atualiza apenas o número e o complemento do endereço da empresa.
        /// </summary>
        /// <param name="numero">O novo número do endereço.</param>
        /// <param name="complemento">O novo complemento do endereço.</param>
        /// <remarks>
        /// Se o endereço não existir, um novo endereço será criado com apenas número e complemento preenchidos.
        /// Os demais campos do endereço permanecerão vazios.
        /// </remarks>
        public void AtualizarNumeroComplemento(string numero, string complemento)
        {
            if (Endereco == null) Endereco = new Endereco();
            Endereco.Numero = numero;
            Endereco.Complemento = complemento;
        }
    }
}