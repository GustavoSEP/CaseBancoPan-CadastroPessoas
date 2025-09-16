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
        
        public PessoasFisicasController(IPessoaFisicaService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PessoaFisicaRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
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
                return StatusCode(500, new { error = $"Erro ao listar pessoas físicas: {ex.Message}" });
            }
        }
        [HttpGet("{cpf}")]
        public async Task<IActionResult> GetByCpf([FromRoute] string cpf)
        {
            try
            {
                var pessoa = await _service.GetPessoaByCpf(cpf);
                if (pessoa == null) return NotFound(new { error = "Pessoa física não encontrada." });
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Erro ao buscar pessoa física por CPF '{cpf}': {ex.Message}" });
            }
        }

        [HttpPut("{cpf}")]
        public async Task<IActionResult> UpdateByCpf([FromRoute] string cpf, [FromBody] PessoaFisicaRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _service.UpdatePessoaByCpfAsync(cpf, request.Nome, request.CPF, request.CEP, request.Numero, request.Complemento);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Erro ao atualizar pessoa física com CPF '{cpf}': {ex.Message}" });
            }
        }
        [HttpDelete("{cpf}")]
        public async Task<IActionResult> DeleteByCpf([FromRoute] string cpf)
        {
            try
            {
                await _service.DeletePessoaByCpfAsync(cpf);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Erro ao deletar pessoa física com CPF '{cpf}': {ex.Message}" });
            }
        }
    }
}