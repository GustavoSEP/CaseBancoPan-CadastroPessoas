using CadastroPessoas.API.Models.Requests;
using CadastroPessoas.API.Models.Responses;
using CadastroPessoas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/pessoas/juridicas")]
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly IPessoaJuridicaService _service;
        private readonly ILogger<PessoaJuridicaController> _logger;

        public PessoaJuridicaController(IPessoaJuridicaService service, ILogger<PessoaJuridicaController> logger)
        {
            _service = service;
            _logger = logger;
        }

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
