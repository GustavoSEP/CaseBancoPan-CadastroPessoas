using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroPessoas.Domain.Entities
{
    /// <summary>
    /// Representa uma pessoa física no sistema de cadastro.
    /// Contém informações pessoais e de endereço de um indivíduo.
    /// </summary>
    public class PessoaFisica
    {
        /// <summary>
        /// Obtém ou define o identificador único da pessoa física.
        /// </summary>
        /// <remarks>Chave primária gerada pelo banco de dados.</remarks>
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome completo da pessoa física.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o CPF (Cadastro de Pessoa Física) da pessoa.
        /// </summary>
        /// <remarks>Formato padrão: 000.000.000-00</remarks>
        public string CPF { get; set; } = string.Empty;

        /// <summary>
        /// Obtém ou define o tipo de pessoa.
        /// </summary>
        /// <remarks>Para pessoa física, o valor padrão é "F".</remarks>
        public string TipoPessoa { get; set; } = "F";

        /// <summary>
        /// Obtém ou define o endereço completo da pessoa física.
        /// </summary>
        public Endereco Endereco { get; set; } = new Endereco();

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaFisica"/> com valores padrão.
        /// </summary>
        public PessoaFisica()
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="PessoaFisica"/> com os dados especificados.
        /// </summary>
        /// <param name="nome">O nome completo da pessoa física.</param>
        /// <param name="cpf">O CPF da pessoa física (formato: 000.000.000-00).</param>
        /// <param name="tipoPessoa">O tipo de pessoa (normalmente "F" para pessoa física).</param>
        /// <param name="endereco">O endereço completo da pessoa física.</param>
        public PessoaFisica(string nome, string cpf, string tipoPessoa, Endereco endereco)
        {
            Nome = nome;
            CPF = cpf;
            TipoPessoa = tipoPessoa;
            Endereco = endereco;
        }

        /// <summary>
        /// Atualiza os dados básicos da pessoa física.
        /// </summary>
        /// <param name="nome">O novo nome completo da pessoa física.</param>
        /// <param name="cpf">O CPF da pessoa física (não deve ser alterado, apenas validado).</param>
        /// <param name="tipoPessoa">O tipo de pessoa (normalmente "F" para pessoa física).</param>
        public void AtualizarDados(string nome, string cpf, string tipoPessoa)
        {
            Nome = nome;
            CPF = cpf;
            TipoPessoa = tipoPessoa;
        }

        /// <summary>
        /// Atualiza o endereço completo da pessoa física.
        /// </summary>
        /// <param name="endereco">O novo endereço completo a ser atribuído.</param>
        public void AtualizarEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        /// <summary>
        /// Atualiza apenas o número e o complemento do endereço da pessoa física.
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