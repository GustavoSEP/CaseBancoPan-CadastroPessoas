using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CadastroPessoas.API.Models.Requests;
using CadastroPessoas.API.Models.Responses;
using CadastroPessoas.Application.Interfaces;
using System;

namespace CadastroPessoas.API.Controllers
{
    /// <summary>
    /// API para gerenciamento de pessoas físicas no sistema de cadastro
    /// Fornece endpoints para criar, listar, consultar, atualizar e excluir pessoas físicas.
    /// </summary>
    [ApiController]
    [Route("api/v1/pessoas/fisicas")]
    public class PessoaFisicaController : ControllerBase
    {
        private readonly IPessoaFisicaService _service;
        private readonly ILogger<PessoaFisicaController> _logger;

        /// <summary>
        /// Inicializa uma nova instância do controlador de pessoas físicas.
        /// </summary>
        /// <param name="service">Service de pessoa fisica, que realiza as ações programadas para pessoa fisica.</param>
        /// <param name="logger">Logger para registro de atividades e erros. Utilizado para observabilidade do código.</param>
        public PessoaFisicaController(IPessoaFisicaService service, ILogger<PessoaFisicaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova pessoa fisica.
        /// </summary>
        /// <param name="request">Dados da pessoa física a ser criada.</param>
        /// <returns>
        /// 201 (Created) Retorna os dados da pessoa fisica criada com as informações de endereço.
        /// 400 (Bad Request) caso os dados de entrada sejam inválidos.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// POST /api/v1/pessoas/fisicas
        /// {
        ///     "nome": "João Silva",
        ///     "cpf": "12345678901",
        ///     "cep": "01001000",
        ///     "numero": "123",
        ///     "complemento": "Apto 45"
        /// }
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PessoaFisicaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var pessoa = await _service.CreateAsync(request.Nome, request.CPF, request.CEP, request.Numero, request.Complemento);
                var response = new PessoaFisicaResponse
                {
                    Id = pessoa.Id,
                    Nome = pessoa.Nome,
                    Documento = pessoa.CPF,
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
                return CreatedAtAction(nameof(GetByCpf), new { cpf = pessoa.CPF }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar pessoa física. Nome: {Nome}, CPF: {Cpf}", request?.Nome, request?.CPF);
                return StatusCode(500, new { error = $"Erro ao criar pessoa física para '{request?.Nome}': {ex.Message}" });
            }
        }

        /// <summary>
        /// Lista todas as pessoas físicas com suporte a paginação.
        /// </summary>
        /// <param name="page">Número da página, iniciando em 1. Valor padrão: 1.</param>
        /// <param name="pageSize">Quantidade de itens por página. Valor padrão: 20.</param>
        /// <returns>
        /// 200 (OK) com a lista de pessoas físicas.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// GET /api/v1/pessoas/fisicas?page=1&amp;pageSize=10
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var lista = await _service.ListAsync();
                var paged = lista.Skip((page - 1) * pageSize).Take(pageSize);
                var result = paged.Select(p => new PessoaFisicaResponse
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Documento = p.CPF,
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
                _logger.LogError(ex, "Erro inesperado ao listar pessoas físicas");
                return StatusCode(500, new { error = $"Erro ao listar pessoas físicas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém os dados de uma pessoa física pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser consultada.</param>
        /// <returns>
        /// 200 (OK) com os dados da pessoa física.
        /// 404 (Not Found) se a pessoa física não for encontrada.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// GET /api/v1/pessoas/fisicas/12345678901
        /// </remarks>
        [HttpGet("{cpf}")]
        public async Task<IActionResult> GetByCpf([FromRoute] string cpf)
        {
            try
            {
                var p = await _service.GetByCpfAsync(cpf);
                if (p == null) return NotFound();
                var response = new PessoaFisicaResponse
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Documento = p.CPF,
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
                _logger.LogError(ex, "Erro inesperado ao obter pessoa física por CPF: {Cpf}", cpf);
                return StatusCode(500, new { error = $"Erro ao obter as informações do CPF: {cpf}. Erro: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa física pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser atualizada.</param>
        /// <param name="request">Novos dados da pessoa física.</param>
        /// <returns>
        /// 204 (No Content) se a atualização for bem-sucedida.
        /// 400 (Bad Request) caso os dados de entrada sejam inválidos.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// PUT /api/v1/pessoas/fisicas/12345678901
        /// {
        ///     "nome": "João Silva Atualizado",
        ///     "cpf": "12345678901",
        ///     "cep": "01001000",
        ///     "numero": "456",
        ///     "complemento": "Sala 789"
        /// }
        /// </remarks>
        [HttpPut("{cpf}")]
        public async Task<IActionResult> UpdateByCpf([FromRoute] string cpf, [FromBody] PessoaFisicaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _service.UpdateByCpfAsync(cpf, request.Nome, request.CPF, request.CEP, request.Numero, request.Complemento);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar pessoa física. CPF alvo: {Cpf}, Novo CPF: {NewCpf}, Nome: {Nome}", cpf, request?.CPF, request?.Nome);
                return StatusCode(500, new { error = $"Erro ao atualizar pessoa com CPF: {cpf}. Erro: {ex.Message}" });
            }
        }
        /// <summary>
        /// Exclui uma pessoa física pelo CPF.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física a ser excluída.</param>
        /// <returns>
        /// 204 (No Content) se a exclusão for bem-sucedida.
        /// 500 (Internal Server Error) em caso de erro no processamento.
        /// </returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// DELETE /api/v1/pessoas/fisicas/12345678901
        /// </remarks>
        [HttpDelete("{cpf}")]
        public async Task<IActionResult> DeleteByCpf([FromRoute] string cpf)
        {
            try
            {
                await _service.DeleteByCpfAsync(cpf);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar pessoa física por CPF: {Cpf}", cpf);
                return StatusCode(500, new { error = $"Erro ao deletar pessoa com CPF: {cpf}. Erro: {ex.Message}" });
            }
        }
    }
}