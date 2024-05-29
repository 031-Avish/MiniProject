using FlightBookingSystemAPI.Models.DTOs.BookingDTO;

namespace FlightBookingSystemAPI.Interfaces
{
    public interface IUserBookingService
    {
        Task<BookingReturnDTO> BookFlight(BookingDTO bookingDTO);
        Task<BookingCancelReturnDTO> CancelBooking(int bookingId);
        Task<List<BookingReturnDTO>> GetUpcomingFlightsByUser(int userId);
        Task<List<BookingReturnDTO>> GetOldFlightsByUser(int userId);
        Task<BookingReturnDTO> GetBookingDetails(int bookingId);
        Task<List<BookingReturnDTO>> GetAllBookingsByAdmin();
        Task<List<BookingReturnDTO>> GetAllBookingsByUser(int userId);
    }
}
