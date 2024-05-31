using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.FlightDTO;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using FlightBookingSystemAPI.Models.DTOs.ScheduleDTO;


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
        private readonly ILogger<ScheduleService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleService"/> class.
        /// </summary>
        /// <param name="scheduleRepository">The schedule repository.</param>
        /// <param name="routeRepository">The route repository.</param>
        /// <param name="flightRepository">The flight repository.</param>
        /// <param name="bookingRepository">The booking repository.</param>
        /// <param name="logger">The logger.</param>
        #region Constructor 
        public ScheduleService(
            IRepository<int, Schedule> scheduleRepository,
            IRepository<int, RouteInfo> routeRepository,
            IRepository<int, Flight> flightRepository,
            IRepository<int, Booking> bookingRepository,
            ILogger<ScheduleService> logger)
        {
            _scheduleRepository = scheduleRepository;
            _routeRepository = routeRepository;
            _flightRepository = flightRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new schedule.
        /// </summary>
        /// <param name="scheduleDTO">The schedule data transfer object.</param>
        /// <returns>The added schedule data transfer object.</returns>
        #region AddSchedule
        public async Task<ScheduleReturnDTO> AddSchedule(ScheduleDTO scheduleDTO)
        {
            try
            {
                _logger.LogInformation("Adding new schedule: {@ScheduleDTO}", scheduleDTO);

                // Check if RouteId exists and its status is enable
                var routeExists = await _routeRepository.GetByKey(scheduleDTO.RouteId);
                if (routeExists.RouteStatus != "Enable")
                    throw new Exception("Cannot add Schedule, Route is Not Enabled");

                // Check if FlightId exists and its status is enable 
                var flightExists = await _flightRepository.GetByKey(scheduleDTO.FlightId);
                if (flightExists.FlightStatus != "Enable")
                    throw new Exception("Cannot add Schedule, Flight is Not Enabled");

                // Get all schedules for the given flight
                IEnumerable<Schedule> existingSchedules = null;
                try
                {
                    existingSchedules = await _scheduleRepository.GetAll();
                }
                catch (ScheduleRepositoryException)
                {

                }
                if (existingSchedules != null)
                {
                    var flightSchedules = existingSchedules
                                .Where(s => s.FlightId == scheduleDTO.FlightId
                                && s.DepartureTime.Date == scheduleDTO.DepartureTime.Date)
                                .ToList();

                    // Find the schedule with the highest reaching time
                    var maxReachingTimeSchedule = flightSchedules.OrderByDescending(s => s.ReachingTime).FirstOrDefault();

                    // Check if the flight can be scheduled
                    if (maxReachingTimeSchedule != null)
                    {
                        // Check if the new schedule starts within the existing schedule's time range
                        if (scheduleDTO.DepartureTime <= maxReachingTimeSchedule.ReachingTime)
                        {
                            throw new Exception("Cannot add Schedule, Flight is already scheduled to depart on the same date within the same time range");
                        }

                        // Check if the new schedule starts at least 2 hours after the existing schedule ends, and from the same end city
                        if (maxReachingTimeSchedule.ReachingTime.AddHours(2) > scheduleDTO.DepartureTime &&
                            routeExists.EndCity != maxReachingTimeSchedule.RouteInfo.StartCity)
                        {
                            throw new Exception("Cannot add Schedule, Flight can only be scheduled from the end city of the previous schedule after 2 hours");
                        }
                    }

                }

                // Create new Schedule
                Schedule newSchedule = MapScheduleWithScheduleDTO(scheduleDTO);
                newSchedule.AvailableSeat = flightExists.TotalSeats;
                Schedule addedSchedule = await _scheduleRepository.Add(newSchedule);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleWithScheduleReturnDTO(addedSchedule);
                _logger.LogInformation("Schedule added successfully: {@ScheduleReturnDTO}", scheduleReturnDTO);
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
                _logger.LogError(ex, "An unexpected error occurred while adding schedule: {Message}", ex.Message);
                throw new ScheduleServiceException("Cannot add Schedule at this moment, some unwanted error occurred: " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        /// <param name="ScheduleUpdateDTO">The schedule update data transfer object.</param>
        /// <returns>The updated schedule data transfer object.</returns>
        #region UpdateSchedule
        public async Task<ScheduleReturnDTO> UpdateSchedule(ScheduleUpdateDTO ScheduleUpdateDTO)
        {
            try
            {

                _logger.LogInformation("Updating schedule: {@ScheduleUpdateDTO}", ScheduleUpdateDTO);

                // Check if RouteId exists and it is Enable 
                var routeExists = await _routeRepository.GetByKey(ScheduleUpdateDTO.RouteId);
                if (routeExists.RouteStatus != "Enable")
                    throw new Exception("Cannot Update Schedule Route is Not Enable");
                // Check if FlightId exists and it is Enable 
                var flightExists = await _flightRepository.GetByKey(ScheduleUpdateDTO.FlightId);
                if (flightExists.FlightStatus != "Enable")
                    throw new Exception("Cannot Update Schedule Flight is Not Enable");
                var sc = await _scheduleRepository.GetByKey(ScheduleUpdateDTO.ScheduleId);
                if (sc.RouteId != ScheduleUpdateDTO.RouteId)
                {
                    checkValidRouteOrNot(ScheduleUpdateDTO);
                }
                    // Get all schedules for the given flight
                IEnumerable<Schedule> existingSchedules = null;
                try
                {
                    existingSchedules = await _scheduleRepository.GetAll();
                }
                catch (ScheduleRepositoryException)
                {

                }

                if (existingSchedules != null)
                {
                    var flightSchedules = existingSchedules
                                .Where(s => s.FlightId == ScheduleUpdateDTO.FlightId
                                && s.ScheduleId != ScheduleUpdateDTO.ScheduleId
                                && s.DepartureTime.Date == ScheduleUpdateDTO.DepartureTime.Date)
                                .ToList();

                    // Find the schedule with the highest reaching time
                    var maxReachingTimeSchedule = flightSchedules.OrderByDescending(s => s.ReachingTime).FirstOrDefault();

                    // Check if the flight can be scheduled
                    if (maxReachingTimeSchedule != null)
                    {
                        // Check if the new schedule starts within the existing schedule's time range
                        if (ScheduleUpdateDTO.DepartureTime <= maxReachingTimeSchedule.ReachingTime)
                        {
                            throw new Exception("Cannot add Schedule, Flight is already scheduled to depart on the same date within the same time range");
                        }

                        // Check if the new schedule starts at least 2 hours after the existing schedule ends, and from the same end city
                        if (maxReachingTimeSchedule.ReachingTime.AddHours(2) > ScheduleUpdateDTO.DepartureTime &&
                            routeExists.EndCity != maxReachingTimeSchedule.RouteInfo.StartCity)
                        {
                            throw new Exception("Cannot add Schedule, Flight can only be scheduled from the end city of the previous schedule after 2 hours");
                        }
                    }

                }

                Schedule schedule = MapScheduleUpdateDTOWithSchedule(ScheduleUpdateDTO);
                Schedule existingSchedule = await _scheduleRepository.GetByKey(schedule.ScheduleId);
                schedule.AvailableSeat = existingSchedule.AvailableSeat;
                Schedule updatedSchedule = await _scheduleRepository.Update(schedule);
                ScheduleReturnDTO updatedScheduleReturnDTO = MapScheduleWithScheduleReturnDTO(updatedSchedule);
                _logger.LogInformation("Schedule updated successfully: {@ScheduleReturnDTO}", updatedScheduleReturnDTO);
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
                _logger.LogError(ex, "An unexpected error occurred while updating the schedule: {Message}", ex.Message);
                throw new ScheduleServiceException("Error occurred while updating the schedule."+ex.Message, ex);
            }
        }

        private async Task checkValidRouteOrNot(ScheduleUpdateDTO scheduleUpdateDTO)
        {
            IEnumerable<Booking> existingBooking = null;
            // Check if any booking exists with the given ScheduleId
            try
            {
                existingBooking = await _bookingRepository.GetAll();
            } // if no booking then Directly delete the Schedule 
            catch (BookingRepositoryException ex) when (ex.Message.Contains("No bookings present"))
            {

            } // If Schedule Exist then Check if it has any booking 
            if (existingBooking.Any(b => b.ScheduleId == scheduleUpdateDTO.ScheduleId))
            {
                var updateSchedule = await _scheduleRepository.GetByKey(scheduleUpdateDTO.ScheduleId);
                // if active booking then Cant Update 
                if (existingBooking.Any(booking => booking.ScheduleId == scheduleUpdateDTO.ScheduleId && (booking.BookingStatus == "Processing" || booking.BookingStatus == "Completed") && booking.FlightDetails.DepartureTime > DateTime.Now))
                {
                    throw new ScheduleServiceException("cannot update Schedule !! Booking has this Schedule Update that first");
                }
            }
        }
        #endregion

        /// <summary>
        /// Deletes a schedule by its ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <returns>The deleted schedule data transfer object.</returns>
        #region DeleteSchedule
        public async Task<ScheduleReturnDTO> DeleteSchedule(int scheduleId)
        {
            try
            {
                IEnumerable<Booking> existingBooking = null;
                // Check if any booking exists with the given ScheduleId
                try
                {
                    existingBooking = await _bookingRepository.GetAll();
                } // if no booking then Directly delete the Schedule 
                catch (BookingRepositoryException ex) when (ex.Message.Contains("No bookings present"))
                {
                    Schedule schedule1 = await _scheduleRepository.DeleteByKey(scheduleId);
                    ScheduleReturnDTO scheduleReturnDTO1 = MapScheduleWithScheduleReturnDTO(schedule1);
                    _logger.LogInformation("Schedule deleted successfully: {@ScheduleReturnDTO}", scheduleReturnDTO1);
                    return scheduleReturnDTO1;
                } // If Schedule Exist then Check if it has any booking 
                if (existingBooking.Any(b => b.ScheduleId == scheduleId))
                {
                    var updateSchedule = await _scheduleRepository.GetByKey(scheduleId);
                    // if active booking then Cant Update 
                    if (existingBooking.Any(booking => booking.ScheduleId == scheduleId && (booking.BookingStatus == "Processing" || booking.BookingStatus == "Completed") && booking.FlightDetails.DepartureTime > DateTime.Now))
                    {
                        throw new ScheduleServiceException("cannot update Schedule !! Booking has this Schedule Update that first");
                    }
                    // if old bookings then we can Update that Schedule 
                    else
                    {
                        updateSchedule.ScheduleStatus = "Disabled";
                        Schedule schedule2 = await _scheduleRepository.Update(updateSchedule);
                        ScheduleReturnDTO scheduleReturnDTO2 = MapScheduleWithScheduleReturnDTO(schedule2);
                        _logger.LogInformation("Schedule updated successfully: {@ScheduleReturnDTO}", scheduleReturnDTO2);
                        return scheduleReturnDTO2;
                    }
                }
                // delete the schedule 
                Schedule schedule = await _scheduleRepository.DeleteByKey(scheduleId);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleWithScheduleReturnDTO(schedule);
                _logger.LogInformation("Schedule deleted successfully: {@ScheduleReturnDTO}", scheduleReturnDTO);
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
                _logger.LogError(ex, "An unexpected error occurred while deleting the schedule: {Message}", ex.Message);
                throw new ScheduleServiceException("Error occurred while deleting the schedule." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all schedules.
        /// </summary>
        /// <returns>A list of all schedule detail data transfer objects.</returns>
        #region GetAllSchedules
        public async Task<List<ScheduleDetailDTO>> GetAllSchedules()
        {
            try
            {
                // get all schedule and map them to return DTO
                var schedules = await _scheduleRepository.GetAll();
                List<ScheduleDetailDTO> scheduleDetailDTOs = new List<ScheduleDetailDTO>();
                foreach (Schedule schedule in schedules)
                {
                    scheduleDetailDTOs.Add(MapScheduleWithScheduleDetailDTO(schedule));
                }
                _logger.LogInformation("Retrieved all schedules successfully.");
                return scheduleDetailDTOs;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all schedules: {Message}", ex.Message);
                throw new ScheduleServiceException("Error occurred while retrieving all schedules.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a schedule by its ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <returns>The schedule detail data transfer object.</returns>
        #region GetSchedule
        public async Task<ScheduleDetailDTO> GetSchedule(int scheduleId)
        {
            try
            {
                //GetAllSchedules schedule based On ID and Map to DTO 
                Schedule schedule = await _scheduleRepository.GetByKey(scheduleId);
                ScheduleDetailDTO scheduleDetailDTO = MapScheduleWithScheduleDetailDTO(schedule);
                _logger.LogInformation("Retrieved schedule with ID {ScheduleId} successfully.", scheduleId);
                return scheduleDetailDTO;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the schedule with ID {ScheduleId}: {Message}", scheduleId, ex.Message);
                throw new ScheduleServiceException("Error occurred while retrieving the schedule.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves flight details for a specific date.
        /// </summary>
        /// <param name="searchDTO">The schedule search data transfer object.</param>
        /// <returns>A list of schedule detail data transfer objects matching the search criteria.</returns>
        #region GetFlightDetailsOnDate
        public async Task<List<ScheduleDetailDTO>> GetFlightDetailsOnDate(ScheduleSearchDTO searchDTO)
        {
            try
            {
                // get all Schedule 
                var schedules = await _scheduleRepository.GetAll();
                //Filter them Based on the start city and end city and the given date 
                List<ScheduleDetailDTO> upcomingSchedules = schedules
                    .Where(s => s.DepartureTime.Date == searchDTO.Date.Date &&
                                s.RouteInfo.StartCity == searchDTO.StartCity &&
                                s.RouteInfo.EndCity == searchDTO.EndCity
                                && s.ScheduleStatus == "Enable")
                    .Select(s => MapScheduleWithScheduleDetailDTO(s))
                    .ToList();
                // if no schedule found then no flight 
                if (upcomingSchedules.Count <= 0)
                {
                    throw new NotPresentException("No upcoming schedules found for the specified criteria.");
                }

                _logger.LogInformation("Retrieved flight details on date {Date} successfully.", searchDTO.Date);
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
                _logger.LogError(ex, "An unexpected error occurred while getting flight details on date {Date}: {Message}", searchDTO.Date, ex.Message);
                throw new ScheduleServiceException("Error occurred while getting flight details on the specified date.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves connecting flights when there is no Direct Flight 
        /// </summary>
        /// <param name="searchDTO">The schedule search data transfer object.</param>
        /// <returns>A list of lists of schedule detail data transfer objects representing connecting flights.</returns>
        #region GetConnectingFlights
        public async Task<List<List<ScheduleDetailDTO>>> GetConnectingFlights(ScheduleSearchDTO searchDTO)
        {
            try
            {   // get all Schedules 
                var schedules = await _scheduleRepository.GetAll();

                // Get Schedule by Fliter
                List<ScheduleDetailDTO> connectingSchedules = new List
    <ScheduleDetailDTO>();
                List<List<ScheduleDetailDTO>> returnDTO = new List<List<ScheduleDetailDTO>>();
                // fliter all flight having Specified start city on given date
                var departingFlights = schedules
                    .Where(s => s.RouteInfo.StartCity == searchDTO.StartCity &&
                        s.DepartureTime.Date == searchDTO.Date.Date
                    && s.ScheduleStatus == "Enable")
                    .ToList();
                // all flight having specified End City 
                var arrivingFlights = schedules
                    .Where(s => s.RouteInfo.EndCity == searchDTO.EndCity && s.ScheduleStatus == "Enable")
                    .ToList();

                foreach (var departingFlight in departingFlights)
                {
                    // Check for Connection and At least 1 hour gap
                    var potentialConnections = arrivingFlights
                        .Where(a => a.RouteInfo.StartCity == departingFlight.RouteInfo.EndCity &&
                                    a.DepartureTime >= departingFlight.ReachingTime.AddHours(1))
                        .ToList();
                    // if any connecting flight then map them and send them 
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
                // if not then throw error 
                if (returnDTO.Count <= 0)
                {
                    throw new NotPresentException("No connecting schedules found for the specified criteria.");
                }

                _logger.LogInformation("Retrieved connecting flights successfully.");
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
                _logger.LogError(ex, "An unexpected error occurred while getting connecting flights: {Message}", ex.Message);
                throw new ScheduleServiceException("Error occurred while getting connecting flights." + ex.Message, ex);
            }
        }
        #endregion
        #region All DTOS
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
