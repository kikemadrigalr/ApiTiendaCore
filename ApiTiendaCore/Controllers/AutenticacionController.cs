using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//referencias para el uso de autenticacion
using ApiTiendaCore.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

//controler para manejar la autenticacion del usuario con el modelo Usuario
namespace ApiTiendaCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretKey;

        //constructor del controler para obtener la secret key desde el archivo appsettings
        public AutenticacionController(IConfiguration configuration)
        {
            secretKey = configuration.GetSection("Settings").GetSection("SecretKey").ToString();
        }

        //metodo para autenticar al usuario
        [HttpPost]
        [Route("Validar")]
        public ActionResult Validar([FromBody] Usuario request)
        {
            //obtener los datos del usuario desde la base de datos
            //
            //

            if (request.Email == "correo@email.com" && request.Password == "123456")
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.Email));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado});
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }
    }
}
