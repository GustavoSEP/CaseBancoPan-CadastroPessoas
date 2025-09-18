using CadastroPessoas.Application.Helpers;
using System;
using Xunit;

namespace CadastroPessoas.Tests.Application.Helpers
{
    public class DocumentoHelperTests
    {
        [Fact]
        public void IsValidCpf_ComCpfValido_DeveRetornarTrue()
        {
            // Arrange
            string cpfValido = "529.982.247-25";

            // Act
            bool resultado = DocumentoHelper.IsValidCpf(cpfValido);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void IsValidCpf_ComCpfInvalido_DeveRetornarFalse()
        {
            // Arrange
            string cpfInvalido = "111.111.111-11";

            // Act
            bool resultado = DocumentoHelper.IsValidCpf(cpfInvalido);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void IsValidCpf_ComCpfVazio_DeveRetornarFalse()
        {
            // Arrange
            string cpfVazio = "";

            // Act
            bool resultado = DocumentoHelper.IsValidCpf(cpfVazio);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void IsValidCpf_ComCpfNulo_DeveRetornarFalse()
        {
            // Arrange
            string cpfNulo = null;

            // Act
            bool resultado = DocumentoHelper.IsValidCpf(cpfNulo);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void FormatCpf_ComCpfValido_DeveFormatarCorretamente()
        {
            // Arrange
            string cpfSemFormato = "49633697883";

            // Act
            string resultado = DocumentoHelper.FormatCpf(cpfSemFormato);

            // Assert
            Assert.Equal("496.336.978-83", resultado);
        }

        [Fact]
        public void FormatCpf_ComCpfFormatado_DeveManterFormatacao()
        {
            // Arrange
            string cpfFormatado = "496.336.978-83";

            // Act
            string resultado = DocumentoHelper.FormatCpf(cpfFormatado);

            // Assert
            Assert.Equal("496.336.978-83", resultado);
        }

        [Fact]
        public void FormatCpf_ComCpfVazio_DeveRetornarVazio()
        {
            // Arrange
            string cpfVazio = "";

            // Act
            string resultado = DocumentoHelper.FormatCpf(cpfVazio);

            // Assert
            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void FormatCpf_ComCpfCurto_DeveRetornarMesmoValor()
        {
            // Arrange
            string cpfCurto = "1234";

            // Act
            string resultado = DocumentoHelper.FormatCpf(cpfCurto);

            // Assert
            Assert.Equal("1234", resultado);
        }

        [Fact]
        public void IsValidCnpj_ComCnpjValido_DeveRetornarTrue()
        {
            // Arrange
            string cnpjValido = "30.306.294/0001-45";

            // Act
            bool resultado = DocumentoHelper.IsValidCnpj(cnpjValido);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void IsValidCnpj_ComCnpjInvalido_DeveRetornarFalse()
        {
            // Arrange
            string cnpjInvalido = "11.111.111/1111-11";

            // Act
            bool resultado = DocumentoHelper.IsValidCnpj(cnpjInvalido);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void IsValidCnpj_ComCnpjVazio_DeveRetornarFalse()
        {
            // Arrange
            string cnpjVazio = "";

            // Act
            bool resultado = DocumentoHelper.IsValidCnpj(cnpjVazio);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void IsValidCnpj_ComCnpjNulo_DeveRetornarFalse()
        {
            // Arrange
            string cnpjNulo = null;

            // Act
            bool resultado = DocumentoHelper.IsValidCnpj(cnpjNulo);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void FormatCnpj_ComCnpjValido_DeveFormatarCorretamente()
        {
            // Arrange
            string cnpjSemFormato = "30306294000145";

            // Act
            string resultado = DocumentoHelper.FormatCnpj(cnpjSemFormato);

            // Assert
            Assert.Equal("30.306.294/0001-45", resultado);
        }

        [Fact]
        public void FormatCnpj_ComCnpjFormatado_DeveManterFormatacao()
        {
            // Arrange
            string cnpjFormatado = "30.306.294/0001-45";

            // Act
            string resultado = DocumentoHelper.FormatCnpj(cnpjFormatado);

            // Assert
            Assert.Equal("30.306.294/0001-45", resultado);
        }

        [Fact]
        public void FormatCnpj_ComCnpjVazio_DeveRetornarVazio()
        {
            // Arrange
            string cnpjVazio = "";

            // Act
            string resultado = DocumentoHelper.FormatCnpj(cnpjVazio);

            // Assert
            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public void FormatCnpj_ComCnpjCurto_DeveRetornarMesmoValor()
        {
            // Arrange
            string cnpjCurto = "1234";

            // Act
            string resultado = DocumentoHelper.FormatCnpj(cnpjCurto);

            // Assert
            Assert.Equal("1234", resultado);
        }
    }
}