using ApiVetPet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetVetPet.Models;
using System.Security.Claims;

namespace ApiVetPet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MascotasController : ControllerBase
    {
        private RepositoryUsuarios repo;

        public MascotasController(RepositoryUsuarios repo)
        {
            this.repo = repo;
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Mascota>>> Mascotas()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Mascota> mascotas = await this.repo.GetMascotas(user.IdUsuario);
            return mascotas;
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateDepartamento
            (Mascota mascota)
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            await this.repo.UpdateMascota(mascota);
            return Ok();
        }


        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Tratamiento>>> Tratamientos()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Tratamiento> tratamientos = await this.repo.GetTratamientos(user.IdUsuario);
            return tratamientos;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Vacuna>>> Vacunas()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Vacuna> vacunas = await this.repo.GetVacunas(user.IdUsuario);
            return vacunas;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Prueba>>> Pruebas()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Prueba> pruebas = await this.repo.GetPruebas(user.IdUsuario);
            return pruebas;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Procedimiento>>> Procedimientos()
        {
            string jsonUser =
                HttpContext.User.Claims.SingleOrDefault(z => z.Type == "USERDATA").Value;
            Usuario user = JsonConvert.DeserializeObject<Usuario>(jsonUser);

            List<Procedimiento> procedimientos = await this.repo.GetProcedimientos(user.IdUsuario);
            return procedimientos;
        }
    }
}
