﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal Taxes { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        [JsonIgnore]
        public Hotel? Hotel { get; set; } = null!;
    }
}
