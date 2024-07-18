let schedules = [];
let dummySchedules = [];

document.getElementById('searchInput').addEventListener('input', function() {
    const value = this.value;
    schedules = dummySchedules;
    const filteredSchedules = schedules.filter(schedule => {
        return schedule.departureTime.includes(value) || schedule.price.toString().includes(value);
    });
    schedules = filteredSchedules;
    displaySchedules();
});

document.getElementById('sortSelect').addEventListener('change', function() {
    const value = this.value;
    if (value === 'priceAsc') {
        schedules.sort((a, b) => a.price - b.price);
    } else if (value === 'priceDesc') {
        schedules.sort((a, b) => b.price - a.price);
    } else if (value === 'departureTimeAsc') {
        schedules.sort((a, b) => new Date(a.departureTime) - new Date(b.departureTime));
    } else if (value === 'departureTimeDesc') {
        schedules.sort((a, b) => new Date(b.departureTime) - new Date(a.departureTime));
    }
    displaySchedules();
});

function validateAddSchedule() {
    const departureTime = document.getElementById('departureTime').value;
    const reachingTime = document.getElementById('reachingTime').value;
    const price = document.getElementById('price').value;
    const routeId = document.getElementById('routeId').value;
    const flightId = document.getElementById('flightId').value;

    if (departureTime === '' || reachingTime === '' || price === '' || routeId === '' || flightId === '') {
        document.getElementById('error').innerHTML = 'Please fill all the fields';
        document.getElementById('error').style.color = 'red';
        return false;
    }
    else if(reachingTime <= departureTime)
    {
        document.getElementById('error').innerHTML = 'Reaching time should be greater than departure time';
        document.getElementById('error').style.color = 'red';
        return false;
    }else if(price<0)
    {
        document.getElementById('error').innerHTML = 'Price should be greater than 0';
        document.getElementById('error').style.color = 'red';
        return false;
    }
    else {
        document.getElementById('error').innerHTML = '';
        return true;
    }
}

function validateUpdateSchedule() {
    const departureTime = document.getElementById('updateDepartureTime').value;
    const reachingTime = document.getElementById('updateReachingTime').value;
    const price = document.getElementById('updatePrice').value;
    const routeId = document.getElementById('updateRouteId').value;
    const flightId = document.getElementById('updateFlightId').value;

    if (departureTime === '' || reachingTime === '' || price === '' || routeId === '' || flightId === '') {
        document.getElementById('updateError').innerHTML = 'Please fill all the fields';
        document.getElementById('updateError').style.color = 'red';
        return false;
    } else {
        document.getElementById('updateError').innerHTML = '';
        return true;
    }
}

function addSchedule() {
    if (!validateAddSchedule()) {
        return;
    }

    const departureTime = document.getElementById('departureTime').value;
    const reachingTime = document.getElementById('reachingTime').value;
    const price = parseFloat(document.getElementById('price').value);
    const routeId = parseInt(document.getElementById('routeId').value);
    const flightId = parseInt(document.getElementById('flightId').value);

    const token = localStorage.getItem('token');
    fetch('https://localhost:7067/api/ScheduleFlight/AddScheduleByAdmin', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token,
        },
        body: JSON.stringify({
            departureTime: departureTime,
            reachingTime: reachingTime,
            price: price,
            routeId: routeId,
            flightId: flightId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            $('#addScheduleModal').modal('hide');
            showAlert(data.errorMessage, "danger");
        } else if (data.errors) {
            $('#addScheduleModal').modal('hide');
            showAlert("Some Unwanted Error Occurred", "danger");
        } else {
            console.log(data);
            document.getElementById('departureTime').value = '';
            document.getElementById('reachingTime').value = '';
            document.getElementById('price').value = '';
            document.getElementById('routeId').value = '';
            document.getElementById('flightId').value = '';
            $('#addScheduleModal').modal('hide');
            showAlert('Schedule added successfully', 'success');
            getSchedules();
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Error occurred while adding schedule', 'danger');
    });
}

document.getElementById('addSchedule').addEventListener('click', addSchedule);

// Function to update a schedule
function updateSchedule() {
    if (!validateUpdateSchedule()) {
        return;
    }

    const scheduleId = parseInt(document.getElementById('updateScheduleId').value);
    const departureTime = document.getElementById('updateDepartureTime').value;
    const reachingTime = document.getElementById('updateReachingTime').value;
    const price = parseFloat(document.getElementById('updatePrice').value);
    const routeId = parseInt(document.getElementById('updateRouteId').value);
    const flightId = parseInt(document.getElementById('updateFlightId').value);

    const token = localStorage.getItem('token');
    fetch(`https://localhost:7067/api/ScheduleFlight/UpdateScheduleByAdmin`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token,
        },
        body: JSON.stringify({
            scheduleId: scheduleId,
            departureTime: departureTime,
            reachingTime: reachingTime,
            price: price,
            routeId: routeId,
            flightId: flightId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, 'danger');
        } else if (data.errors) {
            showAlert('Some unwanted error occurred', 'danger');
        } else {
            console.log(data);
            $('#updateScheduleModal').modal('hide'); // Close the modal
            document.getElementById('updateScheduleForm').reset(); // Reset the form
            showAlert('Schedule updated successfully', 'success');
            getSchedules(); // Refresh the schedule list
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Error occurred while updating schedule', 'danger');
    });
}

document.getElementById('updateSchedule').addEventListener('click', updateSchedule);

// Function to delete a schedule
function deleteSchedule(scheduleId) {
    const token = localStorage.getItem('token');
    fetch(`https://localhost:7067/api/ScheduleFlight/DeleteScheduleByAdmin/${scheduleId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer ' + token,
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, "danger");
        } else if (data.errors) {
            showAlert("Some Unwanted Error Occurred", "danger");
        } else {
            console.log(data);
            showAlert('Schedule deleted successfully', 'success');
            getSchedules();
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Error occurred while deleting schedule', 'danger');
    });
}

// Function to show update modal with schedule details
function showUpdateScheduleModal(scheduleId, departureTime, reachingTime, price, routeId, flightId) {
    document.getElementById('updateScheduleId').value = scheduleId;
    document.getElementById('updateDepartureTime').value = departureTime.slice(0, 16); // Format to 'YYYY-MM-DDTHH:MM'
    document.getElementById('updateReachingTime').value = reachingTime.slice(0, 16);
    document.getElementById('updatePrice').value = price;
    document.getElementById('updateRouteId').value = routeId;
    document.getElementById('updateFlightId').value = flightId;
    $('#updateScheduleModal').modal('show');
}

// Function to get all schedules
let flights=[];
let routes=[];
function getSchedules() {
    const token = localStorage.getItem('token');
    document.getElementById('schedulesList').innerHTML = '';
    fetch('https://localhost:7067/api/ScheduleFlight/GetAllSchedulesByAdmin', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token,
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, "danger");
        } else if (data.errors) {
            showAlert("Some Unwanted Error Occurred", "danger");
        } else {
            schedules = data;
            dummySchedules = data;

            displaySchedules();
            // console.log(schedules);
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Error occurred while fetching schedules', 'danger');
    });
}
function getFlights(){
    const token = localStorage.getItem('token');
    document.getElementById('schedulesList').innerHTML = '';
    fetch('https://localhost:7067/api/AdminFlight/GetAllFlights', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token,
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, "danger");
        } else if (data.errors) {
            showAlert("Some Unwanted Error Occurred", "danger");
        } else {
            flights = data;
            // console.log(flights);  
            getRoutes(); 
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Error occurred while fetching flights', 'danger');
    });
}
function getRoutes(){
    const token = localStorage.getItem('token');
    document.getElementById('schedulesList').innerHTML = '';
    fetch('https://localhost:7067/api/AdminRoute/GetAllRouteInfos', {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + token,
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, "danger");
        } else if (data.errors) {
            showAlert("Some Unwanted Error Occurred", "danger");
        } else {
            showAlert('schedules fetched successfully', 'success');

            routes = data;
            // console.log(routes);
            populateDropdowns();
        }
    })
    .catch(error => {
        // console.error('Error:', error);
        showAlert('Error occurred while fetching routes', 'danger');
    });
}

getFlights();
getSchedules();
function populateDropdowns() {
    // console.log("called");  
    // console.log(routes);
    const routeSelect = document.getElementById('routeId');
    const updateRouteSelect = document.getElementById('updateRouteId');
    routeSelect.innerHTML = '<option value="">Select Route</option>';
    updateRouteSelect.innerHTML = '<option value="">Select Route</option>';
    routes.forEach(route => {
        // console.log(route);
        const option = document.createElement('option');
        option.value = route.routeId;
        option.textContent =route.routeId+' - '+ route.startCity + ' - ' + route.endCity;
        routeSelect.appendChild(option);
        updateRouteSelect.appendChild(option.cloneNode(true));
    });

    const flightSelect = document.getElementById('flightId');
    const updateFlightSelect = document.getElementById('updateFlightId');
    flightSelect.innerHTML = '<option value="">Select Flight</option>';
    updateFlightSelect.innerHTML = '<option value="">Select Flight</option>';
    flights.forEach(flight => {
       
        const option = document.createElement('option');
        option.value = flight.flightId;
        option.textContent = flight.flightId+' - '+ flight.name;
        flightSelect.appendChild(option);
        updateFlightSelect.appendChild(option.cloneNode(true));
    });
}
// Function to display schedules
function displaySchedules() {
    let schedulesList = document.getElementById('schedulesList');
    schedulesList.innerHTML = '';
    schedules.forEach(schedule => {
        let listItem = document.createElement('li');
        listItem.classList.add('list-group-item');
        listItem.innerHTML = `
            <div class="item1">
                <strong>Schedule ID:</strong> ${schedule.scheduleId}
            </div>
            <div>
                <strong>Departure Time:</strong> ${new Date(schedule.departureTime).toLocaleString()}
            </div>
            <div>
                <strong>Reaching Time:</strong> ${new Date(schedule.reachingTime).toLocaleString()}
            </div>
            <div>
                <strong>Price:</strong> Rs ${schedule.price.toFixed(2)}
            </div>
            <div>
                <strong>Route:</strong> ${schedule.routeInfo.startCity} - ${schedule.routeInfo.endCity}
            </div>
            <div class="item3">
                <strong>Flight:</strong> ${schedule.flightInfo.name}
            </div>
            <div class="btns-div">
                <button class="btn btn-primary" onclick="showUpdateScheduleModal(${schedule.scheduleId}, '${schedule.departureTime}', '${schedule.reachingTime}', ${schedule.price}, ${schedule.routeInfo.routeId}, ${schedule.flightInfo.flightId})">Update</button> 
                <button class="btn btn-danger" onclick="deleteSchedule(${schedule.scheduleId})">Delete</button>
                <button class="btn btn-info" onclick="viewDetails(${schedule.scheduleId})">View Details</button>
            </div>
        `;
        schedulesList.appendChild(listItem);
    });
}

// Function to view schedule details including route and flight info
function viewDetails(scheduleId) {
    const schedule = schedules.find(s => s.scheduleId == scheduleId);
   if (schedule) {
        console.log(schedule);
        const routeDetails = `Route ID: ${schedule.routeInfo.routeId}<br>
                              Start City: ${schedule.routeInfo.startCity}<br>
                              End City: ${schedule.routeInfo.endCity}<br>
                              Distance: ${schedule.routeInfo.distance} km`;
        const flightDetails = `Flight ID: ${schedule.flightInfo.flightId}<br>
                               Name: ${schedule.flightInfo.name}<br>
                               Total Seats: ${schedule.flightInfo.totalSeats}`;
        details = `
            <div>
                <h5>Route Details:</h5>
                ${routeDetails}
            </div>
            <div>
                <h5>Flight Details:</h5>
                ${flightDetails}
            </div>
        `;

        // Bootstrap modal to show details
        const modal = `
            <div class="modal fade" id="detailsModal" tabindex="-1" role="dialog" aria-labelledby="detailsModalTitle" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="detailsModalTitle">Schedule Details</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body" id="show-detail">
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Append modal HTML to the body and show it
        document.body.insertAdjacentHTML('beforeend', modal);
        $('#show-detail').html("");
        $('#show-detail').html(details);
        $('#detailsModal').modal('show');
    } else {
        showAlert('Schedule not found', 'danger');
    }
}

// Function to display alerts
function showAlert(message, type) {
            document.getElementById('model-content').innerHTML = message;
            if (type === 'danger') {
                document.getElementById('exampleModalLabel').innerHTML = 'Error!';
                document.getElementById('modal-header').classList.remove('bg-success');
                document.getElementById('modal-header').classList.add('bg-danger');
            } else {
                document.getElementById('exampleModalLabel').innerHTML = 'Success!';
                document.getElementById('modal-header').classList.remove('bg-danger');
                document.getElementById('modal-header').classList.add('bg-success');
            }
            $('.alertModel').modal('show');
            setTimeout(() => {
                $('.alertModel').modal('hide');
            }, 4000);
        }