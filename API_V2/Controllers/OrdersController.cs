using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using API_V2.Models;
using Microsoft.Identity.Client;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_V2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public OrdersController(SqlConnection connection)
        {
            _connection = connection;
        }

        // GET: api/<OrdersController>
        [HttpGet]

        public async Task<IActionResult> Get()
        {
            var orders = new List<Orders>();
            var query = "sp_obtener_orden";
            try { 
        await _connection.OpenAsync();
            using (var command = new SqlCommand(query, _connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var order = new Orders
                        {
                            OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                            ShipName = reader.GetString(reader.GetOrdinal("ShipName")),
                            ShipVia = reader.GetInt32(reader.GetOrdinal("ShipVia")),
                            Freight = reader.GetDecimal(reader.GetOrdinal("Freight"))
                        };
                        orders.Add(order);
                    }
                }
            }
                return Ok(new
                {
                    exito = true,
                    message = "lista de ordenes",
                    data = orders
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new
                {
                    exito = false,
                    message = "Error al obtener datos: " + ex.Message, 
                    data = new List<Orders>() 
                });
            }
            finally
            {

                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }


        // POST api/<OrdersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Orders orders)
        {
            var query = "sp_insertar_orden";
            
            try
            {
                await _connection.OpenAsync();

            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@ShipName", (object?)orders.ShipName ?? DBNull.Value); command.Parameters.AddWithValue("@ShipVia", orders.ShipVia);
            command.Parameters.AddWithValue("@Freight", orders.Freight);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            await command.ExecuteNonQueryAsync();
                return Ok(new
                {
                    exito = true,
                    message = "orden insertada"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, message = ex.Message });
            }
            finally
            {

                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }


        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult>Put(int id, [FromBody] Orders orders)

        {

            var query = "sp_modificar_orden";
            try
            {
                await _connection.OpenAsync();
                using var command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@OrderID", id);
                command.Parameters.AddWithValue("@ShipName", (object?)orders.ShipName ?? DBNull.Value);
                command.Parameters.AddWithValue("@ShipVia", orders.ShipVia);
                command.Parameters.AddWithValue("@Freight", orders.Freight);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                int filasAfectadas = await command.ExecuteNonQueryAsync();

                if (filasAfectadas == 0) 
                {
                    return NotFound(new { exito = false, message = "No se encontró la orden con ese ID" });
                }

                return Ok(new
                {
                    exito = true,
                    message = "orden modificada"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, message = ex.Message });
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }

            }
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
