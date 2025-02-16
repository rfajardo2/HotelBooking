using HotelBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Application.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<Hotel>> SearchHotelsAsync(DateTime checkIn, DateTime checkOut, int guests, string city);
    }
}
