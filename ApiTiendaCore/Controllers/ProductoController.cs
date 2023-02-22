using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiTiendaCore.Models; //referencia a la carpeta de modelos
using System.Data;
using System.Data.SqlClient; //referencia para la conexion y el manejo de la BD
using Microsoft.AspNetCore.Cors;

namespace ApiTiendaCore.Controllers
{
    [EnableCors("ReglasCors")] //habilitar cors para controlador
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {

        //acceder a la cadena de conexion que se encuentra en appsettings
        private readonly string cadenaSQl;

        public ProductoController(IConfiguration config)
        {
            cadenaSQl = config.GetConnectionString("CadenaSql");
        }

        //ENDPOINT PARA LISTAR PRODUCTOS
        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Producto> lista = new List<Producto>();

            try
            {
                using (var connection = new SqlConnection(cadenaSQl))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SP_ListarProductos", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Marca"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", response = lista });
            }
            catch(Exception error) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });
            }
        }


        //ENDPOINT PARA OBTENER UN PRODUCTO SEGUN SU ID
        [HttpGet]
        [Route("obtener/{id:int}")]
        public IActionResult Obtener(int idProducto)
        {
            List<Producto> lista = new List<Producto>();
            Producto producto = new Producto();

            try
            {
                using (var connection = new SqlConnection(cadenaSQl))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SP_ListarProductos", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(rd["IdProducto"]),
                                CodigoBarra = rd["CodigoBarra"].ToString(),
                                Nombre = rd["Nombre"].ToString(),
                                Marca = rd["Marca"].ToString(),
                                Categoria = rd["Categoria"].ToString(),
                                Precio = Convert.ToDecimal(rd["Precio"])
                            });
                        }
                    }
                }
                producto = lista.Where(item => item.IdProducto == idProducto).FirstOrDefault();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", response = producto });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = producto });
            }
        }

        //ENDPOINT PARA GUARDAR UN NUEVO PRODUCTO
        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody]Producto producto)
        {

            try
            {
                using (var connection = new SqlConnection(cadenaSQl))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SP_GuardarProducto", connection);
                    cmd.Parameters.AddWithValue("codigoBarra", producto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", producto.Nombre);
                    cmd.Parameters.AddWithValue("marca", producto.Marca);
                    cmd.Parameters.AddWithValue("categoria", producto.Categoria);
                    cmd.Parameters.AddWithValue("precio", producto.Precio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery(); 
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        //ENDPOINT PARA EDITAR UN PRODUCTO EXISTENTE
        [HttpPut]
        [Route("Editar/id")]
        public IActionResult Editar([FromBody] Producto producto, int id)
        {

            try
            {
                using (var connection = new SqlConnection(cadenaSQl))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SP_EditarProducto", connection);
                    cmd.Parameters.AddWithValue("idProducto", id);
                    cmd.Parameters.AddWithValue("codigoBarra", producto.CodigoBarra is null ? DBNull.Value : producto.CodigoBarra);
                    cmd.Parameters.AddWithValue("nombre", producto.Nombre is null ? DBNull.Value : producto.Nombre);
                    cmd.Parameters.AddWithValue("marca", producto.Marca is null ? DBNull.Value : producto.Marca);
                    cmd.Parameters.AddWithValue("categoria", producto.Categoria is null ? DBNull.Value : producto.Categoria);
                    cmd.Parameters.AddWithValue("precio", producto.Precio == 0 ? DBNull.Value : producto.Precio);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "EDITADO" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        //ENDPOINT PARA Eliminar UN PRODUCTO EXISTENTE
        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public IActionResult Eliminar(int id)
        {

            try
            {
                using (var connection = new SqlConnection(cadenaSQl))
                {
                    connection.Open();
                    var cmd = new SqlCommand("SP_EliminarProducto", connection);
                    cmd.Parameters.AddWithValue("idProducto", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ELIMINADO" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
