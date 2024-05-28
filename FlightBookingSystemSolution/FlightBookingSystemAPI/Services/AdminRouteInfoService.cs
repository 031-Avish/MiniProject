using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Exceptions.ServiceExceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using FlightBookingSystemAPI.Models.DTOs.RouteInfoDTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Services
{
    /// <summary>
    /// Provides services for managing route information for admins.
    /// </summary>
    public class AdminRouteInfoService : IAdminRouteInfoService
    {
        private readonly IRepository<int, RouteInfo> _repository;
        private readonly IRepository<int, Schedule> _scheduleRepository;
        private readonly ILogger<AdminRouteInfoService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminRouteInfoService"/> class.
        /// </summary>
        /// <param name="routeInfoRepository">The route information repository.</param>
        /// <param name="scheduleRepository">The schedule repository.</param>
        /// <param name="logger">The logger instance.</param>
        public AdminRouteInfoService(IRepository<int, RouteInfo> routeInfoRepository, IRepository<int, Schedule> scheduleRepository, ILogger<AdminRouteInfoService> logger)
        {
            _repository = routeInfoRepository;
            _scheduleRepository = scheduleRepository;
            _logger = logger;
        }

        #region AddRouteInfo
        /// <summary>
        /// Adds new route information.
        /// </summary>
        /// <param name="routeInfoDTO">The route information data transfer object.</param>
        /// <returns>The added route information as a data transfer object.</returns>
        public async Task<RouteInfoReturnDTO> AddRouteInfo(RouteInfoDTO routeInfoDTO)
        {
            try
            {
                _logger.LogInformation("Adding new route information.");
                RouteInfo routeInfo = MapRouteInfoWithRouteInfoDTO(routeInfoDTO);
                RouteInfo addedRouteInfo = await _repository.Add(routeInfo);
                RouteInfoReturnDTO routeInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(addedRouteInfo);
                _logger.LogInformation("Route information added successfully.");
                return routeInfoReturnDTO;
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while adding route information.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding route information.");
                throw new AdminRouteInfoServiceException("Cannot add RouteInfo at this moment, some unwanted error occurred: ", ex);
            }
        }
        #endregion

        #region DeleteRouteInfo
        /// <summary>
        /// Deletes route information by its ID.
        /// </summary>
        /// <param name="routeInfoId">The ID of the route information to delete.</param>
        /// <returns>The deleted route information as a data transfer object.</returns>
        public async Task<RouteInfoReturnDTO> DeleteRouteInfo(int routeInfoId)
        {
            try
            {
                _logger.LogInformation("Deleting route information with ID: {RouteInfoId}", routeInfoId);
                RouteInfo routeInfo = await _repository.DeleteByKey(routeInfoId);
                RouteInfoReturnDTO routeInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(routeInfo);
                _logger.LogInformation("Route information deleted successfully.");
                return routeInfoReturnDTO;
            }
            catch (UnableToDeleteRouteInfoException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting route information.");
                throw new AdminRouteInfoServiceException(ex.Message, ex);
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting route information.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting route information.");
                throw new AdminRouteInfoServiceException("Error occurred while deleting RouteInfo: " + ex.Message, ex);
            }
        }
        #endregion

        #region GetAllRouteInfos
        /// <summary>
        /// Gets all route information.
        /// </summary>
        /// <returns>A list of all route information as data transfer objects.</returns>
        public async Task<List<RouteInfoReturnDTO>> GetAllRouteInfos()
        {
            try
            {
                _logger.LogInformation("Retrieving all route information.");
                var routeInfos = await _repository.GetAll();
                List<RouteInfoReturnDTO> routeInfoReturnDTOs = new List<RouteInfoReturnDTO>();
                foreach (RouteInfo routeInfo in routeInfos)
                {
                    routeInfoReturnDTOs.Add(MapRouteInfoWithRouteInfoReturnDTO(routeInfo));
                }
                _logger.LogInformation("All route information retrieved successfully.");
                return routeInfoReturnDTOs;
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all route information.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all route information.");
                throw new AdminRouteInfoServiceException("Error occurred while getting all RouteInfos: " + ex.Message, ex);
            }
        }
        #endregion

        #region GetRouteInfo
        /// <summary>
        /// Gets route information by its ID.
        /// </summary>
        /// <param name="routeInfoId">The ID of the route information to retrieve.</param>
        /// <returns>The route information as a data transfer object.</returns>
        public async Task<RouteInfoReturnDTO> GetRouteInfo(int routeInfoId)
        {
            try
            {
                _logger.LogInformation("Retrieving route information with ID: {RouteInfoId}", routeInfoId);
                RouteInfo routeInfo = await _repository.GetByKey(routeInfoId);
                RouteInfoReturnDTO routeInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(routeInfo);
                _logger.LogInformation("Route information retrieved successfully.");
                return routeInfoReturnDTO;
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving route information.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving route information.");
                throw new AdminRouteInfoServiceException("Error occurred while getting the RouteInfo: " + ex.Message, ex);
            }
        }
        #endregion

        #region UpdateRouteInfo
        /// <summary>
        /// Updates route information.
        /// </summary>
        /// <param name="routeInfoReturnDTO">The route information data transfer object containing the updated information.</param>
        /// <returns>The updated route information as a data transfer object.</returns>
        public async Task<RouteInfoReturnDTO> UpdateRouteInfo(RouteInfoReturnDTO routeInfoReturnDTO)
        {
            try
            {
                _logger.LogInformation("Updating route information with ID: {RouteInfoId}", routeInfoReturnDTO.RouteId);
                RouteInfo routeInfo = MapRouteInfoReturnDTOWithRouteInfo(routeInfoReturnDTO);
                RouteInfo updatedRouteInfo = await _repository.Update(routeInfo);
                RouteInfoReturnDTO updatedRouteInfoReturnDTO = MapRouteInfoWithRouteInfoReturnDTO(updatedRouteInfo);
                _logger.LogInformation("Route information updated successfully.");
                return updatedRouteInfoReturnDTO;
            }
            catch (RouteInfoRepositoryException ex)
            {
                _logger.LogError(ex, "Error occurred while updating route information.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating route information.");
                throw new AdminRouteInfoServiceException("Error occurred while updating the RouteInfo: " + ex.Message, ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Maps a <see cref="RouteInfoDTO"/> to a <see cref="RouteInfo"/>.
        /// </summary>
        /// <param name="routeInfoDTO">The route information data transfer object.</param>
        /// <returns>A new <see cref="RouteInfo"/> object.</returns>
        private RouteInfo MapRouteInfoWithRouteInfoDTO(RouteInfoDTO routeInfoDTO)
        {
            return new RouteInfo
            {
                StartCity = routeInfoDTO.StartCity,
                EndCity = routeInfoDTO.EndCity,
                Distance = routeInfoDTO.Distance
            };
        }

        /// <summary>
        /// Maps a <see cref="RouteInfo"/> to a <see cref="RouteInfoReturnDTO"/>.
        /// </summary>
        /// <param name="routeInfo">The route information.</param>
        /// <returns>A new <see cref="RouteInfoReturnDTO"/> object.</returns>
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

        /// <summary>
        /// Maps a <see cref="RouteInfoReturnDTO"/> to a <see cref="RouteInfo"/>.
        /// </summary>
        /// <param name="routeInfoReturnDTO">The route information return data transfer object.</param>
        /// <returns>A new <see cref="RouteInfo"/> object.</returns>
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
        #endregion
    }
}
