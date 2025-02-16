using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
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

        //[HttpPost]
        //public async Task<IActionResult> AddRoom_([FromBody] Room room)
        //{
        //    await _unitOfWork.Rooms.AddAsync(room);
        //    await _unitOfWork.CompleteAsync();
        //    return CreatedAtAction(nameof(GetRoomsByHotel), new { hotelId = room.HotelId }, room);
        //}

        //[HttpGet("hotel/{hotelId}")]
        //public async Task<IActionResult> GetRoomsByHotel_(int hotelId)
        //{
        //    var rooms = await _unitOfWork.Rooms.GetAllAsync();
        //    return Ok(rooms);
        //}


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

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotel(int hotelId)
        {
            var rooms = await _unitOfWork.Rooms.WhereAsync(r => r.HotelId == hotelId);


            if (rooms == null || !rooms.Any())
                return NotFound($"No rooms found for hotel with ID {hotelId}.");

            return Ok(rooms);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
        {
            var existingRoom = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (existingRoom == null) return NotFound();

            existingRoom.BasePrice = room.BasePrice;
            existingRoom.Taxes = room.Taxes;

            await _unitOfWork.Rooms.UpdateAsync(existingRoom);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _unitOfWork.Rooms.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
    }
}
