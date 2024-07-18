document.getElementById('searchInput').addEventListener('input', function() {
    const value = this.value;
    flights = dummyFlights;
    const filteredFlights = flights.filter(flight => {
        return flight.flightId.toString().includes(value) || flight.name.toLowerCase().includes(value.toLowerCase());
    });
    flights = filteredFlights;
    DisplayFlights();
});

// sort functionality   
document.getElementById('sortSelect').addEventListener('change', function() {
    let dummyFlights = [...flights];
    const value = this.value;
    if (value === 'idA') {
        flights.sort((a, b) => a.flightId - b.flightId);
    } else if (value === 'idD') {
        console.log('idD');
        flights.sort((a, b) => b.flightId - a.flightId);
    } else if (value === 'nameAsc') {
        flights.sort((a, b) => a.name.localeCompare(b.name));
    } else if (value === 'nameDesc') {
        flights.sort((a, b) => b.name.localeCompare(a.name));
    }
    DisplayFlights();
});

// validation of add flight 
function validateAddFlight()
{
    const flightName =document.getElementById('flightName').value;
    const totalSeats = document.getElementById('totalSeats').value;
    if(flightName === '' || totalSeats === ''){
        document.getElementById('error').innerHTML = 'Please fill all the fields';
        document.getElementById('error').style.color = 'red';
        return false;
    }
    else if(totalSeats < 0 && totalSeats>=200){
        document.getElementById('error').innerHTML = 'Total Seats cannot be negative or more than 200';
        document.getElementById('error').style.color = 'red';
        return false;
    }
    else{
        document.getElementById('error').innerHTML = '';
        return true;
    }
}

// validation of update flight 
function validateUpdateFlight()
{
    const flightName =document.getElementById('updateFlightName').value;
    const totalSeats = document.getElementById('updateTotalSeats').value;
    if(flightName === '' || totalSeats === ''){
        document.getElementById('updateError').innerHTML = 'Please fill all the fields';
        document.getElementById('updateError').style.color = 'red';
        return false;
    }
    else if(totalSeats < 0 && totalSeats>=200){
        document.getElementById('updateError').innerHTML = 'Total Seats cannot be negative or more than 200';
        document.getElementById('updateError').style.color = 'red';
        return false;
    }
    else{
        document.getElementById('updateError').innerHTML = '';
        return true;
    }
}

// function to add flight
function AddFlight()
{
    if(validateAddFlight() === false){
        return;
    }
    const flightName =document.getElementById('flightName').value;
    const totalSeats = document.getElementById('totalSeats').value;
    const token = localStorage.getItem('token');
    fetch('https://localhost:7067/api/AdminFlight/AddFlight', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer '+token, 
        },
        body: JSON.stringify({
            name: flightName,
            totalSeats: totalSeats
        })
    })
    .then(response => response.json())
    .then(data => {
        if(data.errorCode){
            showAlert(data.errorMessage, "danger");
        }
        else if(data.errors){
            showAlert("Some Unwanted Error Occured", "danger");
        }
        else{
            console.log(data);
            document.getElementById('flightName').value='';
            document.getElementById('totalSeats').value='';
            $('#exampleModalCenter').modal('hide');    
            showAlert('Flight added successfully', 'success');
            getFlights();
        }
    })
    .catch(error => {
        alert('Error:', error); 
    });
}

document.getElementById('add').addEventListener('click', AddFlight);
document.getElementById('update').addEventListener('click', UpdateFlight);

//update flight 
function UpdateFlight() {
    if (!validateUpdateFlight()) {
        return;
    }
    const flightId = document.getElementById('updateFlightId').value;
    const flightName = document.getElementById('updateFlightName').value;
    const totalSeats = document.getElementById('updateTotalSeats').value;
    const token = localStorage.getItem('token');
    fetch(`https://localhost:7067/api/AdminFlight/UpdateFlight`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer '+token, 
        },
        body: JSON.stringify({
            flightId: flightId,
            name: flightName,
            totalSeats: totalSeats
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, 'danger');
        } else if (data.errors) {
            showAlert('Some unwanted error occured', 'danger');   
        } else {
            console.log(data);
            $('#updateModal').modal('hide'); // Close the modal
            document.getElementById('updateFlightForm').reset(); // Reset the form
            showAlert('Flight updated successfully', 'success');
            getFlights(); // Refresh the flight list
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });
}

// Function to delete a flight
function deleteFlight(flightId)
{
    const token = localStorage.getItem('token');
    fetch(`https://localhost:7067/api/AdminFlight/DeleteFlight/${flightId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer '+token, 
        }
    })
    .then(response => response.json())
    .then(data => {
        if(data.errorCode){
            showAlert(data.errorMessage, "danger");
        }
        else if(data.errors){
            showAlert("Some Unwanted Error Occured", "danger");
        }
        else{
            console.log(data);
            if(data.flightStatus === 'Disabled'){
                showAlert('Flight cannot be deleted as it is scheduled So It is Disabled', 'danger');
            }
            else{
            showAlert('Flight deleted successfully', 'success');
            }
            getFlights();
        }
    })
    .catch(error => {
        showAlert(error,'danger'); 
    });
}

// Function to show the update modal
function showUpdateModal(flightId, flightName, totalSeats) {
    document.getElementById('updateFlightId').value = flightId;
    document.getElementById('updateFlightName').value = flightName;
    document.getElementById('updateTotalSeats').value = totalSeats;
    $('#updateModal').modal('show');
}

// Function to show an alert
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

// Function to get all flights
let flights = {};
let dummyFlights = {};
function getFlights()
{
    const token = localStorage.getItem('token');
    document.getElementById('spinner').style.display = 'block';
    document.querySelector('.list-group').innerHTML = '';
    fetch('https://localhost:7067/api/AdminFlight/GetAllFlights',
    {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer '+token, 
        }
    })
    .then(response => response.json())
    .then(data => {
        document.getElementById('spinner').style.display = 'none';
        if(data.errorCode){
            showAlert(data.errorMessage, "danger");
        }
        else if(data.error){
            showAlert("Some Unwanted Error Occured", "danger");
            console.log(data.error);
        }
        else{
            showAlert('Flights fetched successfully', 'success');
            flights = data;
            dummyFlights = data;
            if(flights.length === 0){
                document.getElementById('no-results').style.display = 'block';
            }
            else{
                document.getElementById('no-results').style.display = 'none';
            }
            DisplayFlights();
            console.log(flights);
        }
    })
    .catch(error => {
        showAlert('Error:', 'danger');
        console.error('Error:', error);
    });
}

getFlights();

// Function to display flights
function DisplayFlights()
{
    console.log(flights);
    if(flights.length === 0){
        document.getElementById('no-results').style.display = 'block';
    }
    else{
        document.getElementById('no-results').style.display = 'none';
    }
    let flightsList = document.querySelector('.list-group');
    flightsList.innerHTML = '';
    console.log(flights);
    flights.forEach(flight => {
    let listItem = document.createElement('li');
    listItem.classList.add('list-group-item');
    listItem.innerHTML = `
        
        <div class="item1">
            <strong>Flight ID:</strong> ${flight.flightId}
        </div>
        <div class="item2" >
            <strong>Flight Name:</strong> ${flight.name}
        </div>
        <div class="item3">
            <strong>Total Seats:</strong> ${flight.totalSeats}
        </div>
        <div class="btns-div">
            <button class="btn btn-primary" onclick="showUpdateModal(${flight.flightId}, '${flight.name}', ${flight.totalSeats})">Update</button> 
            <button class="btn btn-danger" onclick="deleteFlight(${flight.flightId})">Delete</button>
        </div>  
    `;
    flightsList.appendChild(listItem);
});
}