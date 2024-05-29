using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Provides services for managing flight schedules.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly IRepository<int, RouteInfo> _routeRepository;
        private readonly IRepository<int, Flight> _flightRepository;
        private readonly IRepository<int, Booking> _bookingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleService"/> class.
        /// </summary>
        /// <param name="scheduleRepository">The schedule repository.</param>
        /// <param name="routeRepository">The route repository.</param>
        /// <param name="flightRepository">The flight repository.</param>
        /// <param name="bookingRepository">The booking repository.</param>
        public ScheduleService(
            IRepository<int, Schedule> scheduleRepository,
            IRepository<int, RouteInfo> routeRepository,
            IRepository<int, Flight> flightRepository,
            IRepository<int, Booking> bookingRepository)
        {
            _scheduleRepository = scheduleRepository;
            _routeRepository = routeRepository;
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
        }

        #region AddSchedule
        /// <summary>
        /// Adds a new schedule.
        /// </summary>
        /// <param name="scheduleDTO">The schedule data transfer object.</param>
        /// <returns>The added schedule data transfer object.</returns>
        public async Task<ScheduleReturnDTO> AddSchedule(ScheduleDTO scheduleDTO)
        {
            try
            {

                // Check if RouteId exists
                var routeExists = await _routeRepository.GetByKey(scheduleDTO.RouteId);
                if (routeExists.RouteStatus != "Enable")
                    throw new Exception("Cannot add Schedule Route is Not Enable");
                // Check if FlightId exists
                var flightExists = await _flightRepository.GetByKey(scheduleDTO.FlightId);
                if (flightExists.FlightStatus != "Enable")
                    throw new Exception("Cannot add Schedule Flight is Not Enable");

                Schedule schedule = MapScheduleWithScheduleDTO(scheduleDTO);
                schedule.AvailableSeat = flightExists.TotalSeats;
                Schedule addedSchedule = await _scheduleRepository.Add(schedule);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleWithScheduleReturnDTO(addedSchedule);
                return scheduleReturnDTO;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (FlightRepositoryException)
            {
                throw;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Cannot add Schedule at this moment, some unwanted error occurred: "+ex.Message, ex);
            }
        }
        #endregion

        #region UpdateSchedule
        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        /// <param name="ScheduleUpdateDTO">The schedule update data transfer object.</param>
        /// <returns>The updated schedule data transfer object.</returns>
        public async Task<ScheduleReturnDTO> UpdateSchedule(ScheduleUpdateDTO ScheduleUpdateDTO)
        {
            try
            {
                // Check if RouteId exists
                var routeExists = await _routeRepository.GetByKey(ScheduleUpdateDTO.RouteId);
                if (routeExists.RouteStatus != "Enable")
                    throw new Exception("Cannot Update Schedule Route is Not Enable");
                // Check if FlightId exists
                var flightExists = await _flightRepository.GetByKey(ScheduleUpdateDTO.FlightId);
                if (flightExists.FlightStatus != "Enable")
                    throw new Exception("Cannot add Schedule Flight is Not Enable");

                Schedule schedule = MapScheduleUpdateDTOWithSchedule(ScheduleUpdateDTO);
                Schedule existingSchedule = await _scheduleRepository.GetByKey(schedule.ScheduleId);
                schedule.AvailableSeat = existingSchedule.AvailableSeat;
                Schedule updatedSchedule = await _scheduleRepository.Update(schedule);
                ScheduleReturnDTO updatedScheduleReturnDTO = MapScheduleWithScheduleReturnDTO(updatedSchedule);
                return updatedScheduleReturnDTO;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (FlightRepositoryException)
            {
                throw;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while updating the schedule.", ex);
            }
        }
        #endregion

        #region DeleteSchedule
        /// <summary>
        /// Deletes a schedule by its ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <returns>The deleted schedule data transfer object.</returns>
        public async Task<ScheduleReturnDTO> DeleteSchedule(int scheduleId)
        {
            try
            {
                IEnumerable<Booking> existingBooking = null;
                // Check if any booking exists with the given ScheduleId
                try
                {
                    existingBooking = await _bookingRepository.GetAll();
                }
                catch (BookingRepositoryException ex) when (ex.Message.Contains("No bookings present"))
                {
                    Schedule schedule1 = await _scheduleRepository.DeleteByKey(scheduleId);
                    ScheduleReturnDTO scheduleReturnDTO1 = MapScheduleWithScheduleReturnDTO(schedule1);
                    return scheduleReturnDTO1;
                }
                if (existingBooking.Any(b => b.ScheduleId == scheduleId))
                {
                    var updateSchedule = await _scheduleRepository.GetByKey(scheduleId);
                    if (existingBooking.Any(booking => booking.ScheduleId == scheduleId &&( booking.BookingStatus == "Processing" || booking.BookingStatus == "Completed" )&& booking.FlightDetails.DepartureTime > DateTime.Now))
                    {
                        throw new ScheduleServiceException("cannot update Schedule !! Booking has this Schedule Update that first");
                    }
                    else
                    {
                        updateSchedule.ScheduleStatus = "Disabled";
                        Schedule schedule2 = await _scheduleRepository.Update(updateSchedule);
                        ScheduleReturnDTO scheduleReturnDTO2 = MapScheduleWithScheduleReturnDTO(schedule2);
                        return scheduleReturnDTO2;
                    }
                }
                Schedule schedule = await _scheduleRepository.DeleteByKey(scheduleId);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleWithScheduleReturnDTO(schedule);
                return scheduleReturnDTO;
            }
            catch (BookingRepositoryException)
            {
                throw;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while deleting the schedule."+ex.Message, ex);
            }
        }
        #endregion

        #region GetAllSchedules
        /// <summary>
        /// Retrieves all schedules.
        /// </summary>
        /// <returns>A list of all schedule detail data transfer objects.</returns>
        public async Task<List<ScheduleDetailDTO>> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleRepository.GetAll();
                List<ScheduleDetailDTO> scheduleDetailDTOs = new List<ScheduleDetailDTO>();
                foreach (Schedule schedule in schedules)
                {
                    scheduleDetailDTOs.Add(MapScheduleWithScheduleDetailDTO(schedule));
                }
                return scheduleDetailDTOs;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while retrieving all schedules.", ex);
            }
        }
        #endregion

        #region GetSchedule
        /// <summary>
        /// Retrieves a schedule by its ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <returns>The schedule detail data transfer object.</returns>
        public async Task<ScheduleDetailDTO> GetSchedule(int scheduleId)
        {
            try
            {
                Schedule schedule = await _scheduleRepository.GetByKey(scheduleId);
                ScheduleDetailDTO scheduleDetailDTO = MapScheduleWithScheduleDetailDTO(schedule);
                return scheduleDetailDTO;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while retrieving the schedule.", ex);
            }
        }
        #endregion

        #region GetFlightDetailsOnDate
        /// <summary>
        /// Retrieves flight details for a specific date.
        /// </summary>
        /// <param name="searchDTO">The schedule search data transfer object.</param>
        /// <returns>A list of schedule detail data transfer objects matching the search criteria.</returns>
        public async Task<List<ScheduleDetailDTO>> GetFlightDetailsOnDate(ScheduleSearchDTO searchDTO)
        {
            try
            {
                var schedules = await _scheduleRepository.GetAll();
                List<ScheduleDetailDTO> upcomingSchedules = schedules
                    .Where(s => s.DepartureTime.Date == searchDTO.Date.Date &&
                                s.RouteInfo.StartCity == searchDTO.StartCity &&
                                s.RouteInfo.EndCity == searchDTO.EndCity 
                                && s.ScheduleStatus == "Enable")
                    .Select(s => MapScheduleWithScheduleDetailDTO(s))
                    .ToList();

                if (upcomingSchedules.Count <= 0)
                {
                    throw new NotPresentException("No upcoming schedules found for the specified criteria.");
                }

                return upcomingSchedules;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleServiceException("No schedules found for the provided criteria: " + ex.Message, ex);
            }
            catch (ScheduleRepositoryException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while getting flight details on the specified date.", ex);
            }
        }
        #endregion

        #region GetConnectingFlights
        /// <summary>
        /// Retrieves connecting flights for a specific search criteria.
        /// </summary>
        /// <param name="searchDTO">The schedule search data transfer object.</param>
        /// <returns>A list of lists of schedule detail data transfer objects representing connecting flights.</returns>
        public async Task<List<List<ScheduleDetailDTO>>> GetConnectingFlights(ScheduleSearchDTO searchDTO)
        {
            try
            {
                var schedules = await _scheduleRepository.GetAll();
                List<ScheduleDetailDTO> connectingSchedules = new List<ScheduleDetailDTO>();
                List<List<ScheduleDetailDTO>> returnDTO = new List<List<ScheduleDetailDTO>>();
                var departingFlights = schedules
                    .Where(s => s.RouteInfo.StartCity == searchDTO.StartCity && 
                        s.DepartureTime.Date == searchDTO.Date.Date
                    && s.ScheduleStatus == "Enable")
                    .ToList();

                var arrivingFlights = schedules
                    .Where(s => s.RouteInfo.EndCity == searchDTO.EndCity && s.ScheduleStatus == "Enable")
                    .ToList();

                foreach (var departingFlight in departingFlights)
                {
                    var potentialConnections = arrivingFlights
                        .Where(a => a.RouteInfo.StartCity == departingFlight.RouteInfo.EndCity &&
                                    a.DepartureTime >= departingFlight.ReachingTime.AddHours(1))
                        .ToList();

                    if (potentialConnections.Any())
                    {
                        foreach (var connection in potentialConnections)
                        {
                            connectingSchedules.Add(MapScheduleWithScheduleDetailDTO(departingFlight));
                            connectingSchedules.Add(MapScheduleWithScheduleDetailDTO(connection));
                        }
                        returnDTO.Add(connectingSchedules);
                    }
                }

                if (returnDTO.Count <= 0)
                {
                    throw new NotPresentException("No connecting schedules found for the specified criteria.");
                }

                return returnDTO;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleServiceException("No connecting flights found for the provided criteria: " + ex.Message, ex);
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while getting connecting flights." + ex.Message, ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Maps a <see cref="ScheduleDTO"/> to a <see cref="Schedule"/>.
        /// </summary>
        /// <param name="scheduleDTO">The schedule data transfer object.</param>
        /// <returns>The schedule entity.</returns>
        private Schedule MapScheduleWithScheduleDTO(ScheduleDTO scheduleDTO)
        {
            return new Schedule
            {
                DepartureTime = scheduleDTO.DepartureTime,
                ReachingTime = scheduleDTO.ReachingTime,
                Price = scheduleDTO.Price,
                RouteId = scheduleDTO.RouteId,
                FlightId = scheduleDTO.FlightId
            };
        }

        /// <summary>
        /// Maps a <see cref="Schedule"/> to a <see cref="ScheduleReturnDTO"/>.
        /// </summary>
        /// <param name="schedule">The schedule entity.</param>
        /// <returns>The schedule return data transfer object.</returns>
        private ScheduleReturnDTO MapScheduleWithScheduleReturnDTO(Schedule schedule)
        {
            return new ScheduleReturnDTO
            {
                ScheduleId = schedule.ScheduleId,
                DepartureTime = schedule.DepartureTime,
                ReachingTime = schedule.ReachingTime,
                AvailableSeat = schedule.AvailableSeat,
                Price = schedule.Price,
                RouteId = schedule.RouteId,
                FlightId = schedule.FlightId
            };
        }

        /// <summary>
        /// Maps a <see cref="ScheduleReturnDTO"/> to a <see cref="Schedule"/>.
        /// </summary>
        /// <param name="scheduleReturnDTO">The schedule return data transfer object.</param>
        /// <returns>The schedule entity.</returns>
        private Schedule MapScheduleReturnDTOWithSchedule(ScheduleReturnDTO scheduleReturnDTO)
        {
            return new Schedule
            {
                ScheduleId = scheduleReturnDTO.ScheduleId,
                DepartureTime = scheduleReturnDTO.DepartureTime,
                ReachingTime = scheduleReturnDTO.ReachingTime,
                AvailableSeat = scheduleReturnDTO.AvailableSeat,
                Price = scheduleReturnDTO.Price,
                RouteId = scheduleReturnDTO.RouteId,
                FlightId = scheduleReturnDTO.FlightId
            };
        }

        /// <summary>
        /// Maps a <see cref="ScheduleUpdateDTO"/> to a <see cref="Schedule"/>.
        /// </summary>
        /// <param name="scheduleUpdateDTO">The schedule update data transfer object.</param>
        /// <returns>The schedule entity.</returns>
        private Schedule MapScheduleUpdateDTOWithSchedule(ScheduleUpdateDTO scheduleUpdateDTO)
        {
            return new Schedule
            {
                ScheduleId = scheduleUpdateDTO.ScheduleId,
                DepartureTime = scheduleUpdateDTO.DepartureTime,
                ReachingTime = scheduleUpdateDTO.ReachingTime,
                Price = scheduleUpdateDTO.Price,
                RouteId = scheduleUpdateDTO.RouteId,
                FlightId = scheduleUpdateDTO.FlightId
            };
        }

        /// <summary>
        /// Maps a <see cref="Schedule"/> to a <see cref="ScheduleDetailDTO"/>.
        /// </summary>
        /// <param name="schedule">The schedule entity.</param>
        /// <returns>The schedule detail data transfer object.</returns>
        private ScheduleDetailDTO MapScheduleWithScheduleDetailDTO(Schedule schedule)
        {
            return new ScheduleDetailDTO
            {
                ScheduleId = schedule.ScheduleId,
                DepartureTime = schedule.DepartureTime,
                ReachingTime = schedule.ReachingTime,
                AvailableSeat = schedule.AvailableSeat,
                Price = schedule.Price,

                RouteInfo = new RouteInfoReturnDTO
                {
                    RouteId = schedule.RouteInfo.RouteId,
                    StartCity = schedule.RouteInfo.StartCity,
                    EndCity = schedule.RouteInfo.EndCity,
                    Distance = schedule.RouteInfo.Distance
                },
                FlightInfo = new FlightReturnDTO
                {
                    FlightId = schedule.FlightInfo.FlightId,
                    Name = schedule.FlightInfo.Name,
                    TotalSeats = schedule.FlightInfo.TotalSeats
                }
            };
        }
        #endregion
    }
}
