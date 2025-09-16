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
    [ApiController]
    [Route("api/v1/pessoas/fisicas")]
    public class PessoasFisicasController : ControllerBase
    {
        private readonly IPessoaFisicaService _service;
        private readonly ILogger<PessoasFisicasController> _logger;

        public PessoasFisicasController(IPessoaFisicaService service, ILogger<PessoasFisicasController> logger)
        {
            _service = service;
            _logger = logger;
        }

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