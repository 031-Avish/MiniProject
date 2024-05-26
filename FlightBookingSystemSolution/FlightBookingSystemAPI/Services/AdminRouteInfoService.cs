using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using FlightBookingSystemAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Services
{
    public class AdminRouteInfoService : IAdminRouteInfoService
    {
        private readonly IRepository<int, RouteInfo> _repository;
        private readonly IRepository<int, Schedule> _scheduleRepository;

        public AdminRouteInfoService(IRepository<int, RouteInfo> routeInfoRepository, IRepository<int, Schedule> scheduleRepository)
        {
            _repository = routeInfoRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<RouteInfoReturnDTO> AddRouteInfo(RouteInfoDTO routeInfoDTO)
        {
            try
            {
                RouteInfo routeInfo = MapRouteInfoWithRouteInfoDTO(routeInfoDTO);
                RouteInfo addedRouteInfo = await _repository.Add(routeInfo);
                RouteInfoReturnDTO routeInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(addedRouteInfo);
                return routeInfoReturnDTO;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdminRouteInfoServiceException("Cannot add RouteInfo at this moment, some unwanted error occurred: ", ex);
            }
        }

        private RouteInfoReturnDTO MapRouteInfoWithRouteInfoReturnDTO(RouteInfo routeInfo)
        {
            return new RouteInfoReturnDTO
            {
                RouteId = routeInfo.RouteId,
                StartCity = routeInfo.StartCity,
                EndCity = routeInfo.EndCity,
                Distance = routeInfo.Distance
            };
        }

        private RouteInfo MapRouteInfoWithRouteInfoDTO(RouteInfoDTO routeInfoDTO)
        {
            return new RouteInfo
            {
                StartCity = routeInfoDTO.StartCity,
                EndCity = routeInfoDTO.EndCity,
                Distance = routeInfoDTO.Distance
            };
        }

        public async Task<RouteInfoReturnDTO> DeleteRouteInfo(int routeInfoId)
        {
            try
            {
                //var schedules = await _scheduleRepository.GetAll();
                //bool hasSchedules = schedules.Any(s => s.RouteId == routeInfoId);
                //if (hasSchedules)
                //{
                //    throw new UnableToDeleteRouteInfoException("Cannot delete RouteInfo because it has associated schedules.");
                //}

                RouteInfo routeInfo = await _repository.DeleteByKey(routeInfoId);
                RouteInfoReturnDTO routeInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(routeInfo);
                return routeInfoReturnDTO;
            }
            catch (UnableToDeleteRouteInfoException ex)
            {
                throw new AdminRouteInfoServiceException(ex.Message, ex);
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdminRouteInfoServiceException("Error occurred while deleting RouteInfo: " + ex.Message, ex);
            }
        }

        public async Task<List<RouteInfoReturnDTO>> GetAllRouteInfos()
        {
            try
            {
                var routeInfos = await _repository.GetAll();
                List<RouteInfoReturnDTO> routeInfoReturnDTOs = new List<RouteInfoReturnDTO>();
                foreach (RouteInfo routeInfo in routeInfos)
                {
                    routeInfoReturnDTOs.Add(MapRouteInfoWithRouteInfoReturnDTO(routeInfo));
                }
                return routeInfoReturnDTOs;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdminRouteInfoServiceException("Error occurred while getting all RouteInfos: " + ex.Message, ex);
            }
        }

        public async Task<RouteInfoReturnDTO> GetRouteInfo(int routeInfoId)
        {
            try
            {
                RouteInfo routeInfo = await _repository.GetByKey(routeInfoId);
                RouteInfoReturnDTO routeInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(routeInfo);
                return routeInfoReturnDTO;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AdminRouteInfoServiceException("Error occurred while getting the RouteInfo: " + e.Message, e);
            }
        }

        public async Task<RouteInfoReturnDTO> UpdateRouteInfo(RouteInfoReturnDTO routeInfoReturnDTO)
        {
            try
            {
                RouteInfo routeInfo = MapRouteInfoReturnDTOWithRouteInfo(routeInfoReturnDTO);
                RouteInfo updatedRouteInfo = await _repository.Update(routeInfo);
                RouteInfoReturnDTO updatedRouteInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(updatedRouteInfo);
                return updatedRouteInfoReturnDTO;
            }
            catch (RouteInfoRepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AdminRouteInfoServiceException("Error occurred while updating the RouteInfo: " + e.Message, e);
            }
        }

        private RouteInfo MapRouteInfoReturnDTOWithRouteInfo(RouteInfoReturnDTO routeInfoReturnDTO)
        {
            return new RouteInfo
            {
                RouteId = routeInfoReturnDTO.RouteId,
                StartCity = routeInfoReturnDTO.StartCity,
                EndCity = routeInfoReturnDTO.EndCity,
                Distance = routeInfoReturnDTO.Distance
            };
        }
    }
}
