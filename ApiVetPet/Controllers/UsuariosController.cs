
using ApiVetPet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetVetPet.Models;
using System.Security.Claims;

namespace ApiRepasoSegundoExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private RepositoryUsuarios repo;

        public UsuariosController(RepositoryUsuarios repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateDepartamento
            (Usuario usuario)
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            await this.repo.UpdateUsuario(usuario);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> PerfilUsuario()
        {
            //DEBEMOS BUSCAR EL CLAIM DEL EMPLEADO
            Claim claim = HttpContext.User.Claims
                .SingleOrDefault(x => x.Type == "USERDATA");
            string jsonUsuario =
                claim.Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>
                (jsonUsuario);
            return usuario;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Cita>>> Citas()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Cita> citas = await this.repo.GetCitas();
            return citas;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Evento>>> Eventos()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Evento> eventos = await this.repo.GetEventos(user.IdUsuario);
            return eventos;
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> SolicitarCita(Cita cita)
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            await this.repo.CreateCita(cita, cita.IdUsuario);
            return Ok();
        }

    }
}
