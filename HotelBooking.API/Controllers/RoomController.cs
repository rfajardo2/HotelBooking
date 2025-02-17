using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddRoom([FromBody] Room room)
        {
            if (room == null)
                return BadRequest("Room data is invalid.");

            // Verificar si el hotel existe antes de agregar la habitación
            var hotelExists = await _unitOfWork.Hotels.AnyAsync(h => h.Id == room.HotelId);
            if (!hotelExists)
                return BadRequest($"No hotel found with ID {room.HotelId}");

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetRoomsByHotel), new { hotelId = room.HotelId }, room);
        }

        [Authorize]
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotel(int hotelId)
        {
            var rooms = await _unitOfWork.Rooms.WhereAsync(r => r.HotelId == hotelId);


            if (rooms == null || !rooms.Any())
                return NotFound($"No rooms found for hotel with ID {hotelId}.");

            return Ok(rooms);
        }



        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
        {
            if (room == null || id != room.Id)
                return BadRequest("Invalid room data.");

            var existingRoom = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (existingRoom == null) return NotFound();

            existingRoom.BasePrice = room.BasePrice;
            existingRoom.Taxes = room.Taxes;
            existingRoom.Type = room.Type;
            existingRoom.HotelId = room.HotelId;
            existingRoom.Location = room.Location;


            await _unitOfWork.Rooms.UpdateAsync(existingRoom);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleRoomStatus(int id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null) return NotFound();

            room.IsActive = !room.IsActive;
            await _unitOfWork.Rooms.UpdateAsync(room);
            await _unitOfWork.CompleteAsync();

            return Ok(room);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _unitOfWork.Rooms.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }

        [Authorize]
        [HttpPost("{hotelId}/assign-rooms")]
        public async Task<IActionResult> AssignRoomsToHotel(int hotelId, [FromBody] List<Room> rooms)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);
            if (hotel == null) return NotFound("No se encontró el hotel especificado.");

            foreach (var room in rooms)
            {
                room.HotelId = hotelId; // Asignar el hotel a cada habitación
                await _unitOfWork.Rooms.AddAsync(room);
            }

            await _unitOfWork.CompleteAsync();
            return Ok(new { message = "Habitaciones asignadas correctamente.", hotel });
        }


    }
}
