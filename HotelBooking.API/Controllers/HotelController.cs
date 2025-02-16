using HotelBooking.Application.Interfaces;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.API.Controllers
{
    namespace HotelBooking.API.Controllers
    {
        [Route("api/hotels")]
        [ApiController]
        public class HotelController : ControllerBase
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IHotelService _hotelService;

            public HotelController(IUnitOfWork unitOfWork , IHotelService hotelService)
            {
                _unitOfWork = unitOfWork;
                _hotelService = hotelService;
            }

            [HttpGet("search")]
            public async Task<ActionResult<IEnumerable<Hotel>>> SearchHotels(
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime checkOut,
            [FromQuery] int guests,
            [FromQuery] string city)
            {
                var hotels = await _hotelService.SearchHotelsAsync(checkIn, checkOut, guests, city);

                if (hotels == null || !hotels.Any())
                    return NotFound(new { message = "No hotels found for the given criteria." });

                return Ok(hotels);
            }



            [HttpGet]
            public async Task<IActionResult> GetHotels()
            {
                var hotels = await _unitOfWork.Hotels.GetAllAsync();
                return Ok(hotels);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetHotelById(int id)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
                return hotel == null ? NotFound() : Ok(hotel);
            }

            [HttpPost]
            public async Task<IActionResult> AddHotel([FromBody] Hotel hotel)
            {
                await _unitOfWork.Hotels.AddAsync(hotel);
                await _unitOfWork.CompleteAsync();
                return CreatedAtAction(nameof(GetHotelById), new { id = hotel.Id }, hotel);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateHotel(int id, [FromBody] Hotel hotel)
            {
                if (hotel == null || id != hotel.Id)
                    return BadRequest("Invalid hotel data.");

                var existingHotel = await _unitOfWork.Hotels.GetByIdAsync(id);
                if (existingHotel == null) return NotFound();

                existingHotel.Name = hotel.Name;
                existingHotel.Address = hotel.Address;
                existingHotel.Address = hotel.Address;

                await _unitOfWork.Hotels.UpdateAsync(existingHotel);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }

            [HttpPatch("{id}/status")]
            public async Task<IActionResult> ToggleHotelStatus(int id)
            {
                var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
                if (hotel == null) return NotFound();

                hotel.IsActive = !hotel.IsActive;
                await _unitOfWork.Hotels.UpdateAsync(hotel);
                await _unitOfWork.CompleteAsync();

                return Ok(hotel);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteHotel(int id)
            {
                await _unitOfWork.Hotels.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return NoContent();
            }

        




        }
    }
}
