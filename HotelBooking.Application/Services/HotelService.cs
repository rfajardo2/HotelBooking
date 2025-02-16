using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(DateTime checkIn, DateTime checkOut, int guests, string city)
        {
            return await _hotelRepository.SearchHotelsAsync(checkIn, checkOut, guests, city);
        }
    }
}
