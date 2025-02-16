using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Interfaces
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> SearchHotelsAsync(DateTime checkIn, DateTime checkOut, int guests, string city);
    }
}
