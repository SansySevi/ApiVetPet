using ApiVetPet.Helpers;
using ApiVetPet.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NugetVetPet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiVetPet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryUsuarios repo;
        private HelperOAuthToken helper;

        public AuthController(RepositoryUsuarios repo, HelperOAuthToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Usuario usuario = await this.repo.ExisteUsuario(model.UserName, model.Password);

            if(usuario == null)
            {
                return Unauthorized();
            } else
            {
                SigningCredentials credentials =
                    new SigningCredentials(this.helper.GetKeyToken(),
                        SecurityAlgorithms.HmacSha256);

                string jsonUsuario = JsonConvert.SerializeObject(usuario);
                Claim[] claims = new Claim[]
                {
                    new Claim("USERDATA", jsonUsuario)
                };

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: claims,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );

                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

        }


        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Register(Usuario user)
        {
            await this.repo.RegisterAsync(user);
            return Ok();
        }
    }
}
