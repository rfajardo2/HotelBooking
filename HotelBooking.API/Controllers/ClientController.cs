using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.API.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly HotelBookingDbContext _context;

        public ClientController(HotelBookingDbContext context)
        {
            _context = context;
        }

        // GET /api/clients (Obtener todos los clientes)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // GET /api/clients/{id} (Obtener un cliente por ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado." });
            }
            return client;
        }

        // POST /api/clients (Crear un cliente)
        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            if (client == null)
            {
                return BadRequest(new { message = "Datos inválidos." });
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        // PUT /api/clients/{id} (Actualizar un cliente)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest(new { message = "ID no coincide." });
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound(new { message = "Cliente no encontrado." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //  DELETE /api/clients/{id} (Eliminar un cliente)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado." });
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
