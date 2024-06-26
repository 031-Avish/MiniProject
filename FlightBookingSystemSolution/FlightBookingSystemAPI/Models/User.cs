﻿using FlightBookingSystemAPI.Models.DTOs;
using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = "User";

    }
}
