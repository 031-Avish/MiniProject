using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleReturnDTO> AddSchedule(ScheduleDTO scheduleDTO);
        Task<ScheduleReturnDTO> UpdateSchedule(ScheduleReturnDTO scheduleReturnDTO);
        Task<ScheduleReturnDTO> DeleteSchedule(int scheduleId);
        Task<List<ScheduleDetailDTO>> GetAllSchedules();
        Task<ScheduleDetailDTO> GetSchedule(int scheduleId);
    }
}
