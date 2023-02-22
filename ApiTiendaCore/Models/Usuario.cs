using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiTiendaCore.Models
{
    public class Usuario
    {
        //clase para que el usuario pueda autenticarse en la api con las propiedad email y password
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
