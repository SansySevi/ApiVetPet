using ApiVetPet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NugetVetPet.Models;

namespace ApiVetPet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : Controller
    {
        private RepositoryUsuarios repo;

        public ApplicationController(RepositoryUsuarios repo)
        {
            this.repo = repo;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Servicio>>> GetServicios()
        {

            List<Servicio> servicios = await this.repo.GetServicios();
            return servicios;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<FAQ>>> GetFAQs()
        {

            List<FAQ> fAQs = await this.repo.GetFAQs();
            return fAQs;
        }
    }
}
