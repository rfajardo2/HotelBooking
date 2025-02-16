using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Interfaces;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelBookingDbContext _context;

        public HotelRepository(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsAsync(DateTime checkIn, DateTime checkOut, int guests, string city)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                .Where(h => h.Address.Contains(city) && h.IsActive)
                .Where(h => h.Rooms.Any(r => r.IsActive))
                .ToListAsync();
        }
    }
}
