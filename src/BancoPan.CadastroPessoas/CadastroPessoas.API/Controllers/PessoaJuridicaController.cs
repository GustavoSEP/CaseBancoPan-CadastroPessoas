using CadastroPessoas.API.Models.Requests;
using CadastroPessoas.API.Models.Responses;
using CadastroPessoas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.API.Controllers
{
    /// <summary>
    /// API para gerenciamento de pessoas jurídicas no sistema de cadastro.
    /// Fornece endpoints para criar, listar, consultar, atualizar e excluir pessoas jurídicas.
    /// </summary>
    [ApiController]
    [Route("api/v1/pessoas/juridicas")]
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly IPessoaJuridicaService _service;
        private readonly ILogger<PessoaJuridicaController> _logger;

        /// <summary>
        /// Inicializa uma nova instância do controlador de pessoas jurídicas.
        /// </summary>
        /// <param name="service">Service de pessoa Juridica que realiza as ações programadas para pessoa fisica.</param>
        /// <param name="logger">Logger para registro de atividades e erros. Utilizado para observabilidade do código.</param>
        public PessoaJuridicaController(IPessoaJuridicaService service, ILogger<PessoaJuridicaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova pessoa jurídica no sistema.
        /// </summary>
        /// <param name="request">Dados da pessoa jurídica a ser criada.</param>
        /// <returns>
        /// 201 (Created) com os dados da pessoa jurídica criada e um link para consulta.
        /// 400 (Bad Request) caso os dados de entrada sejam inválidos.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// POST /api/v1/pessoas/juridicas
        /// {
        ///     "razaoSocial": "Empresa ABC Ltda",
        ///     "nomeFantasia": "ABC Sistemas",
        ///     "cnpj": "12345678000190",
        ///     "cep": "01001000",
        ///     "numero": "123",
        ///     "complemento": "Sala 45"
        /// }
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PessoaJuridicaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var pessoa = await _service.CreateAsync(
                    request.RazaoSocial,
                    request.NomeFantasia,
                    request.CNPJ,
                    request.CEP,
                    request.Numero ?? string.Empty,
                    request.Complemento ?? string.Empty);

                var response = new PessoaJuridicaResponse
                {
                    Id = pessoa.Id,
                    RazaoSocial = pessoa.RazaoSocial,
                    NomeFantasia = pessoa.NomeFantasia,
                    Documento = pessoa.CNPJ,
                    TipoPessoa = pessoa.TipoPessoa,
                    Endereco = new EnderecoDto
                    {
                        Cep = pessoa.Endereco.Cep,
                        Logradouro = pessoa.Endereco.Logradouro,
                        Bairro = pessoa.Endereco.Bairro,
                        Cidade = pessoa.Endereco.Cidade,
                        Estado = pessoa.Endereco.Estado,
                        Numero = pessoa.Endereco.Numero,
                        Complemento = pessoa.Endereco.Complemento
                    }
                };

                return CreatedAtAction(nameof(GetByCnpj), new { cnpj = pessoa.CNPJ }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar pessoa jurídica. Razão social: {RazaoSocial}, CNPJ: {Cnpj}", request?.RazaoSocial, request?.CNPJ);
                return StatusCode(500, new { error = $"Erro ao cadastrar pessoa jurídica '{request?.RazaoSocial}': {ex.Message}" });
            }
        }

        /// <summary>
        /// Lista todas as pessoas jurídicas com suporte a paginação.
        /// </summary>
        /// <param name="page">Número da página, iniciando em 1. Valor padrão: 1.</param>
        /// <param name="pageSize">Quantidade de itens por página. Valor padrão: 20.</param>
        /// <returns>
        /// 200 (OK) com a lista de pessoas jurídicas.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// GET /api/v1/pessoas/juridicas?page=1&amp;pageSize=10
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var lista = await _service.ListAsync();
                var paged = lista.Skip((page - 1) * pageSize).Take(pageSize);
                var result = paged.Select(p => new PessoaJuridicaResponse
                {
                    Id = p.Id,
                    RazaoSocial = p.RazaoSocial,
                    NomeFantasia = p.NomeFantasia,
                    Documento = p.CNPJ,
                    TipoPessoa = p.TipoPessoa,
                    Endereco = new EnderecoDto
                    {
                        Cep = p.Endereco.Cep,
                        Logradouro = p.Endereco.Logradouro,
                        Bairro = p.Endereco.Bairro,
                        Cidade = p.Endereco.Cidade,
                        Estado = p.Endereco.Estado,
                        Numero = p.Endereco.Numero,
                        Complemento = p.Endereco.Complemento
                    }
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao listar pessoas jurídicas");
                return StatusCode(500, new { error = $"Erro ao listar pessoas jurídicas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém os dados de uma pessoa jurídica pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser consultada.</param>
        /// <returns>
        /// 200 (OK) com os dados da pessoa jurídica.
        /// 404 (Not Found) se a pessoa jurídica não for encontrada.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// GET /api/v1/pessoas/juridicas/12345678000190
        /// </remarks>
        [HttpGet("{cnpj}")]
        public async Task<IActionResult> GetByCnpj([FromRoute] string cnpj)
        {
            try
            {
                var p = await _service.GetByCnpjAsync(cnpj);
                if (p == null) return NotFound();

                var response = new PessoaJuridicaResponse
                {
                    Id = p.Id,
                    RazaoSocial = p.RazaoSocial,
                    NomeFantasia = p.NomeFantasia,
                    Documento = p.CNPJ,
                    TipoPessoa = p.TipoPessoa,
                    Endereco = new EnderecoDto
                    {
                        Cep = p.Endereco.Cep,
                        Logradouro = p.Endereco.Logradouro,
                        Bairro = p.Endereco.Bairro,
                        Cidade = p.Endereco.Cidade,
                        Estado = p.Endereco.Estado,
                        Numero = p.Endereco.Numero,
                        Complemento = p.Endereco.Complemento
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter pessoa jurídica por CNPJ: {Cnpj}", cnpj);
                return StatusCode(500, new { error = $"Erro ao obter as informações do CNPJ: {cnpj}. Erro: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa jurídica pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser atualizada.</param>
        /// <param name="request">Novos dados da pessoa jurídica.</param>
        /// <returns>
        /// 204 (No Content) se a atualização for bem-sucedida.
        /// 400 (Bad Request) caso os dados de entrada sejam inválidos.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// PUT /api/v1/pessoas/juridicas/12345678000190
        /// {
        ///     "razaoSocial": "Empresa ABC Atualizada Ltda",
        ///     "nomeFantasia": "ABC Sistemas",
        ///     "cnpj": "12345678000190",
        ///     "cep": "01001000",
        ///     "numero": "456",
        ///     "complemento": "Andar 10"
        /// }
        /// </remarks>
        [HttpPut("{cnpj}")]
        public async Task<IActionResult> UpdateByCnpj([FromRoute] string cnpj, [FromBody] PessoaJuridicaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.UpdateByCnpjAsync(
                    cnpj,
                    request.RazaoSocial,
                    request.NomeFantasia,
                    request.CNPJ,
                    request.CEP,
                    request.Numero ?? string.Empty,
                    request.Complemento ?? string.Empty);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar pessoa jurídica. CNPJ alvo: {Cnpj}", cnpj);
                return StatusCode(500, new { error = $"Erro ao atualizar a pessoa jurídica com CNPJ: {cnpj}. Erro: {ex.Message}" });
            }
        }

        /// <summary>
        /// Exclui uma pessoa jurídica pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica a ser excluída.</param>
        /// <returns>
        /// 204 (No Content) se a exclusão for bem-sucedida.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// DELETE /api/v1/pessoas/juridicas/12345678000190
        /// </remarks>
        [HttpDelete("{cnpj}")]
        public async Task<IActionResult> DeleteByCnpj([FromRoute] string cnpj)
        {
            try
            {
                await _service.DeleteByCnpjAsync(cnpj);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar pessoa jurídica por CNPJ: {Cnpj}", cnpj);
                return StatusCode(500, new { error = $"Erro ao deletar pessoa com CNPJ: {cnpj}. Erro: {ex.Message}" });
            }
        }
    }
}