﻿using System.ComponentModel.DataAnnotations;

namespace FlightBookingSystemAPI.Models.DTOs.ScheduleDTO
{
    public class ScheduleUpdateDTO
    {
        [Required(ErrorMessage = "Schedule ID is required")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Departure time is required")]
        public DateTime DepartureTime { get; set; }

        [Required(ErrorMessage = "Reaching time is required")]
        public DateTime ReachingTime { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Route ID is required")]
        public int RouteId { get; set; }

        [Required(ErrorMessage = "Flight ID is required")]
        public int FlightId { get; set; }
    }
}
