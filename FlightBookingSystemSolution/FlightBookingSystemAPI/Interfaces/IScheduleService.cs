using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleReturnDTO> AddSchedule(ScheduleDTO scheduleDTO);
        Task<ScheduleReturnDTO> UpdateSchedule(ScheduleUpdateDTO ScheduleUpdateDTO);
        Task<ScheduleReturnDTO> DeleteSchedule(int scheduleId);
        Task<List<ScheduleDetailDTO>> GetAllSchedules();
        Task<ScheduleDetailDTO> GetSchedule(int scheduleId);
        Task<List<ScheduleDetailDTO>> GetFlightDetailsOnDate(ScheduleSearchDTO searchDTO);
        Task<List<List<ScheduleDetailDTO>>> GetConnectingFlights(ScheduleSearchDTO searchDTO);
    }

}
