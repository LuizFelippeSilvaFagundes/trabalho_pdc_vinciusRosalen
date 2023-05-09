using exercicio1.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // falta Implementar a lógica para obter o número do bicho do dia
            // Pode ser uma consulta a uma fonte externa ou uma lógica interna da aplicação
            return "123"; // Exemplo de número do bicho
        }

        private Cliente? ObterClienteLogado()
        {
            // falta a lógica para obter o cliente logado
            // Pode ser por meio de autenticação e autorização na API
            // Retornar o cliente correspondente ao usuário autenticado ou null se não estiver logado
            // Você pode acessar a informação do cliente a partir de uma sessão ou token de autenticação
            return null; // Exemplo: nenhum cliente logado
        }
    }
}
