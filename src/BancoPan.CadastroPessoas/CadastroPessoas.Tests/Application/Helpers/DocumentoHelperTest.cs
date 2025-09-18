using CadastroPessoas.Application.Helpers;
using System;
using Xunit;

namespace CadastroPessoas.Tests.Helpers
{
    public class DocumentoHelperTests
    {
        [Fact]
        public void NormalizeDigits_DeveRemoverCaracteresNaoNumericos()
        {

            string entrada = "496.336.978-83";

            string resultado = DocumentoHelper.NormalizeDigits(entrada);

            Assert.Equal("49633697883", resultado);
        }

        [Fact]
        public void NormalizeDigits_ComEntradaVazia_DeveRetornarStringVazia()
        {
            string resultado = DocumentoHelper.NormalizeDigits(null);

            Assert.Equal("", resultado);
        }

        [Fact]
        public void IsValidCpf_ComCpfValido_DeveRetornarTrue()
        {
            string cpfValido = "529.982.247-25";

            bool resultado = DocumentoHelper.IsValidCpf(cpfValido);

            Assert.True(resultado);
        }

        [Fact]
        public void IsValidCpf_ComCpfInvalido_DeveRetornarFalse()
        {
            string cpfInvalido = "111.111.111-11";

            bool resultado = DocumentoHelper.IsValidCpf(cpfInvalido);

            Assert.False(resultado);
        }

        [Fact]
        public void FormatCpf_ComCpfValido_DeveFormatarCorretamente()
        {
            string cpfSemFormato = "49633697883";

            string resultado = DocumentoHelper.FormatCpf(cpfSemFormato);

            Assert.Equal("496.336.978-83", resultado);
        }

        [Fact]
        public void FormatCpf_ComCpfInvalido_DeveLancarExcecao()
        {
            string cpfInvalido = "1234";

            Assert.Throws<ArgumentException>(() => DocumentoHelper.FormatCpf(cpfInvalido));
        }

        [Fact]
        public void IsValidCnpj_ComCnpjValido_DeveRetornarTrue()
        {
            string cnpjValido = "30.306.294/0001-45";

            bool resultado = DocumentoHelper.IsValidCnpj(cnpjValido);

            Assert.True(resultado);
        }

        [Fact]
        public void IsValidCnpj_ComCnpjInvalido_DeveRetornarFalse()
        {
            string cnpjInvalido = "11.111.111/1111-11";

            bool resultado = DocumentoHelper.IsValidCnpj(cnpjInvalido);

            Assert.False(resultado);
        }

        [Fact]
        public void FormatCnpj_ComCnpjValido_DeveFormatarCorretamente()
        {
            string cnpjSemFormato = "30306294000145";

            string resultado = DocumentoHelper.FormatCnpj(cnpjSemFormato);

            Assert.Equal("30.306.294/0001-45", resultado);
        }

        [Fact]
        public void FormatCnpj_ComCnpjInvalido_DeveLancarExcecao()
        {
            string cnpjInvalido = "1234";

            Assert.Throws<ArgumentException>(() => DocumentoHelper.FormatCnpj(cnpjInvalido));
        }

        [Fact]
        public void IsValidTipoPessoa_ComTipoFisica_DeveRetornarTrue()
        {
            string tipo = "F";

            bool resultado = DocumentoHelper.IsValidTipoPessoa(tipo);

            Assert.True(resultado);
        }

        [Fact]
        public void IsValidTipoPessoa_ComTipoJuridica_DeveRetornarTrue()
        {
            string tipo = "J";

            bool resultado = DocumentoHelper.IsValidTipoPessoa(tipo);

            Assert.True(resultado);
        }

        [Fact]
        public void IsValidTipoPessoa_ComTipoInvalido_DeveRetornarFalse()
        {
            string tipo = null;

            bool resultado = DocumentoHelper.IsValidTipoPessoa(tipo);

            Assert.False(resultado);
        }

        [Fact]
        public void NormalizeTipoPessoa_ComTipoF_DeveRetornarF()
        {
            string tipo = "F";

            string resultado = DocumentoHelper.NormalizeTipoPessoa(tipo);

            Assert.Equal("F", resultado);
        }

        [Fact]
        public void NormalizeTipoPessoa_ComTipoFisica_DeveRetornarF()
        {
            string tipo = "FISICA";

            string resultado = DocumentoHelper.NormalizeTipoPessoa(tipo);

            Assert.Equal("F", resultado);
        }

        [Fact]
        public void NormalizeTipoPessoa_ComTipoJ_DeveRetornarJ()
        {
            string tipo = "J";

            string resultado = DocumentoHelper.NormalizeTipoPessoa(tipo);

            Assert.Equal("J", resultado);
        }

        [Fact]
        public void NormalizeTipoPessoa_ComTipoJuridica_DeveRetornarJ()
        {
            string tipo = "JURIDICA";

            string resultado = DocumentoHelper.NormalizeTipoPessoa(tipo);

            Assert.Equal("J", resultado);
        }

        [Fact]
        public void NormalizeTipoPessoa_ComTipoInvalido_DeveLancarExcecao()
        {
            string tipo = "X";

            Assert.Throws<ArgumentException>(() => DocumentoHelper.NormalizeTipoPessoa(tipo));
        }
    }
}