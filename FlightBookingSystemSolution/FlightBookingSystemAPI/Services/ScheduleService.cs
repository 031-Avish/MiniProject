using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
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
    public class ScheduleService : IScheduleService
    {
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly IRepository<int, RouteInfo> _routeRepository;
        private readonly IRepository<int, Flight> _flightRepository;
        private readonly IRepository<int, Booking> _bookingRepository;

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

        public async Task<ScheduleReturnDTO> AddSchedule(ScheduleDTO scheduleDTO)
        {
            try
            {
                // Check if RouteId exists
                var routeExists = await _routeRepository.GetByKey(scheduleDTO.RouteId);
                // Check if FlightId exists
                var flightExists = await _flightRepository.GetByKey(scheduleDTO.FlightId);

                Schedule schedule = MapScheduleWithScheduleDTO(scheduleDTO);
                schedule.AvailableSeat= flightExists.TotalSeats;
                Schedule addedSchedule = await _scheduleRepository.Add(schedule);
                ScheduleReturnDTO scheduleReturnDTO = MapScheduleWithScheduleReturnDTO(addedSchedule);
                return scheduleReturnDTO;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch(FlightRepositoryException)
            {
                throw;
            }
            catch (ScheduleRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Cannot add Schedule at this moment, some unwanted error occurred: ", ex);
            }
        }

        public async Task<ScheduleReturnDTO> UpdateSchedule(ScheduleUpdateDTO ScheduleUpdateDTO)
        {
            try
            {
                // Check if RouteId exists
                var routeExists = await _routeRepository.GetByKey(ScheduleUpdateDTO.RouteId);
                
                // Check if FlightId exists
                var flightExists = await _flightRepository.GetByKey(ScheduleUpdateDTO.FlightId);
                
                Schedule schedule = MapScheduleUpdateDTOWithSchedule(ScheduleUpdateDTO);
                Schedule existingSchedule =await _scheduleRepository.GetByKey(schedule.ScheduleId);
                schedule.AvailableSeat=existingSchedule.AvailableSeat;
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

        public async Task<ScheduleReturnDTO> DeleteSchedule(int scheduleId)
        {
            try
            {
                // Check if any booking exists with the given ScheduleId
                var existingBooking = await _bookingRepository.GetAll();
                if (existingBooking.Any(b => b.ScheduleId == scheduleId))
                {
                    throw new ScheduleServiceException("Cannot delete the schedule as there are existing bookings with this schedule.");
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
                throw new ScheduleServiceException("Error occurred while deleting the schedule.", ex);
            }
        }

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

        private Schedule MapScheduleWithScheduleDTO(ScheduleDTO scheduleDTO)
        {
            return new Schedule
            {
                DepartureTime = scheduleDTO.DepartureTime,
                ReachingTime = scheduleDTO.ReachingTime,
                //AvailableSeat = scheduleDTO.AvailableSeat,
                Price = scheduleDTO.Price,
                RouteId = scheduleDTO.RouteId,
                FlightId = scheduleDTO.FlightId
            };
        }

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

        public async Task<List<ScheduleDetailDTO>> GetFlightDetailsOnDate(ScheduleSearchDTO searchDTO)
        {
            try
            {
                var schedules = await _scheduleRepository.GetAll();
                List<ScheduleDetailDTO> upcomingSchedules = schedules
                    .Where(s => DateOnly.FromDateTime(s.DepartureTime) == searchDTO.Date &&
                                s.RouteInfo.StartCity == searchDTO.StartCity &&
                                s.RouteInfo.EndCity == searchDTO.EndCity)
                    .Select(s => MapScheduleWithScheduleDetailDTO(s))
                    .ToList();

                if (upcomingSchedules.Count <= 0 )
                {
                    throw new NotPresentException("No upcoming schedules found for the specified criteria.");
                }

                return upcomingSchedules;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleServiceException("No schedules found for the provided criteria: " + ex.Message, ex);
            }
            catch(ScheduleRepositoryException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while getting flight details on the specified date.", ex);
            }
        }

        public async Task<List<List<ScheduleDetailDTO>>> GetConnectingFlights(ScheduleSearchDTO searchDTO)
        {
            try
            {
                var schedules = await _scheduleRepository.GetAll();
                List<ScheduleDetailDTO> connectingSchedules = new List<ScheduleDetailDTO>();
                List<List<ScheduleDetailDTO>> returnDTO= new List<List<ScheduleDetailDTO>>();   
                var departingFlights = schedules
                    .Where(s => s.RouteInfo.StartCity == searchDTO.StartCity && DateOnly.FromDateTime(s.DepartureTime) == searchDTO.Date)
                    .ToList();

                var arrivingFlights = schedules
                    .Where(s => s.RouteInfo.EndCity == searchDTO.EndCity)
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

                if (returnDTO.Count <=0 )
                {
                    throw new NotPresentException("No connecting schedules found for the specified criteria.");
                }

                return returnDTO;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleServiceException("No connecting flights found for the provided criteria: " + ex.Message, ex);
            }
            catch(ScheduleRepositoryException )
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ScheduleServiceException("Error occurred while getting connecting flights."+ex.Message, ex);
            }
        }
    }
}
