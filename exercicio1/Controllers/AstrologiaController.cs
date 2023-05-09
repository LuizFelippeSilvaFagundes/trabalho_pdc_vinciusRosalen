using exercicio1.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace AstrologiaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AstrologiaController : ControllerBase
    {
        private readonly List<Signo> signos;
        private readonly List<Cliente> clientes;

        public AstrologiaController()
        {
            // Inicialize a lista de signos
            signos = new List<Signo>
            {
                new Signo { Nome = "Áries", DataInicio = new DateTime(DateTime.Now.Year, 3, 21), DataFim = new DateTime(DateTime.Now.Year, 4, 19) },
                new Signo { Nome = "Touro", DataInicio = new DateTime(DateTime.Now.Year, 4, 20), DataFim = new DateTime(DateTime.Now.Year, 5, 20) },
                new Signo { Nome = "Gêmeos", DataInicio = new DateTime(DateTime.Now.Year, 5, 21), DataFim = new DateTime(DateTime.Now.Year, 6, 20) },
                // Adicione os demais signos
                // ...
            };

            // Inicialize a lista de clientes
            clientes = new List<Cliente>();
        }

        [HttpGet("signos")]
        public ActionResult<IEnumerable<string>> GetSignos()
        {
            // Retorna apenas os nomes dos signos
            return signos.Select(s => s.Nome).ToList();
        }

        [HttpGet("previsao/{signo}")]
        public ActionResult<string> GetPrevisao(string signo)
        {
            // Encontre o signo correspondente ao parâmetro informado
            var signoEncontrado = signos.FirstOrDefault(s => string.Equals(s.Nome, signo, StringComparison.OrdinalIgnoreCase));

            if (signoEncontrado != null)
            {
                // Verifique o plano do cliente
                var cliente = ObterClienteLogado(); // Implemente o método para obter o cliente logado
                if (cliente != null)
                {
                    if (cliente.Plano == "basico")
                    {
                        return $"Previsão básica para o signo {signoEncontrado.Nome}";
                    }
                    else if (cliente.Plano == "avancado")
                    {
                        return $"Previsão avançada para o signo {signoEncontrado.Nome} com o número do bicho do dia: {ObterNumeroBicho()}";
                    }
                }

                // Se o plano do cliente não for reconhecido ou não estiver logado, retorne uma previsão básica padrão
                return $"Previsão básica para o signo {signoEncontrado.Nome}";
            }

            return NotFound(); // Retorna 404 se o signo não for encontrado
        }

        [HttpPost("cliente")]
        public ActionResult<string> CriarCliente([FromBody] Cliente cliente)
        {
            // Verifique se o cliente já existe pelo nickname
            if (clientes.Any(c => c.Nickname == cliente.Nickname))
            {
                return BadRequest("Já existe um cliente com esse nickname.");
            }

            // Adicione o cliente à lista de clientes
            clientes.Add(cliente);

            return Ok("Cliente cadastrado com sucesso.");
        }

        [HttpGet("cliente/{nickname}")]
        public ActionResult<Cliente> ObterCliente(string nickname)
        {
            // Encontre o cliente correspondente ao nickname informado
            var clienteEncontrado = clientes.FirstOrDefault(c => string.Equals(c.Nickname, nickname, StringComparison.OrdinalIgnoreCase));

            if (clienteEncontrado != null)
            {
                return Ok(clienteEncontrado);
            }

            return NotFound(); // Retorna 404 se o cliente não for encontrado
        }


        [HttpPut("cliente/{nickname}/plano")]
        public ActionResult<string> AtualizarPlano(string nickname, [FromBody] string novoPlano)
        {
            // Encontre o cliente correspondente ao nickname informado
            var clienteEncontrado = clientes.FirstOrDefault(c => string.Equals(c.Nickname, nickname, StringComparison.OrdinalIgnoreCase));

            if (clienteEncontrado != null)
            {
                // Atualize o plano do cliente
                clienteEncontrado.Plano = novoPlano;
                return Ok("Plano atualizado com sucesso.");
            }

            return NotFound(); // Retorna 404 se o cliente não for encontrado
        }

        private string ObterNumeroBicho()
        {

            // Obtém o dia do ano atual
            int diaDoAno = DateTime.Now.DayOfYear;

            // Realiza algum cálculo ou lógica para determinar o número do bicho com base no dia do ano
            int numeroBicho = (diaDoAno % 10) + 1;

            return numeroBicho.ToString();

        }

        private Cliente? ObterClienteLogado()
        {
                // Obtém o token de autenticação do cabeçalho da requisição
                string token = Request.Headers["Authorization"];

                if (!string.IsNullOrEmpty(token))
                {
                    // validação e decodificação do token JWT para obter as informações do cliente
                    var handler = new JwtSecurityTokenHandler();

                    try
                    {
                        var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave-secreta")),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        }, out var validatedToken);

                        // Verifica se o token é válido e se contém as informações necessárias
                        if (validatedToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Claims.Any())
                        {
                            // Recupera as informações do cliente do token
                            var nickname = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value;

                            if (!string.IsNullOrEmpty(nickname))
                            {
                                // Encontra o cliente correspondente ao nickname
                                var clienteEncontrado = clientes.FirstOrDefault(c => string.Equals(c.Nickname, nickname, StringComparison.OrdinalIgnoreCase));

                                if (clienteEncontrado != null)
                                {
                                    return clienteEncontrado;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Tratar erros na validação do token, caso necessário

                    }
                }
                return null; // Nenhum cliente logado ou token inválido
            }
        }
    }

