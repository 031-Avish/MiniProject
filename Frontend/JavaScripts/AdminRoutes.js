
let routes = [];
let dummyRoutes = [];
function getRoutes() {
    const token = localStorage.getItem('token');
    fetch('https://localhost:7067/api/AdminRoute/GetAllRouteInfos', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token,
        }
    })
    .then(response => response.json())
    .then(data =>
    {
        if(data.errorCode)
        {
            showAlert(data.errorMessage, 'danger');
        }
        else if(data.errors){
            showAlert("Something Went Wrong Please Try again", 'danger');
        }
        else{
            showAlert('Routes fetched successfully', 'success');
            routes = data;
            dummyRoutes = data;
            console.log(data);
            displayRoutes();
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });
}
getRoutes();

document.getElementById('searchInput').addEventListener('input', function() {
    const value = this.value;
    routes = dummyRoutes;
    if (value === '') {
        displayRoutes();
        return;
    }
    const filteredRoutes = routes.filter(route => {
        return route.routeId==value;
    });
    routes = filteredRoutes;
    displayRoutes();
});
document.getElementById('searchInputStart').addEventListener('input', function() {
    const value = this.value;
    routes = dummyRoutes;
    const endCityValue=document.getElementById('searchInputEnd').value;
    if (value === '' && endCityValue==='' ) {
        displayRoutes();
        return;
    }
    const filteredRoutes = routes.filter(route => {
        return route.startCity.toLowerCase().includes(value.toLowerCase()) && route.endCity.toLowerCase().includes(endCityValue.toLowerCase() );
    });
    routes = filteredRoutes;
    displayRoutes();
});
document.getElementById('searchInputEnd').addEventListener('input', function() {
    const value = this.value;
    routes = dummyRoutes;
    const startCityValue=document.getElementById('searchInputStart').value;
    if (value === '' && startCityValue==='' ) {
        displayRoutes();
        return;
    }
    const filteredRoutes = routes.filter(route => {
        return route.endCity.toLowerCase().includes(value.toLowerCase()) && route.startCity.toLowerCase().includes(startCityValue.toLowerCase() );
    });
    routes = filteredRoutes;
    displayRoutes();
});

document.getElementById('sortSelect').addEventListener('change', function() {
    const value = this.value;
    if (value === 'distanceAsc') {
        routes.sort((a, b) => a.distance - b.distance);
    } else if (value === 'distanceDesc') {
        routes.sort((a, b) => b.distance - a.distance);
    } else if (value === 'startCityAsc') {
        routes.sort((a, b) => a.startCity.localeCompare(b.startCity));
    } else if (value === 'startCityDesc') {
        routes.sort((a, b) => b.startCity.localeCompare(a.startCity));
    }
    displayRoutes();
});

function validateAddRoute() {
    const startCity = document.getElementById('startCity').value;
    const endCity = document.getElementById('endCity').value;
    const distance = document.getElementById('distance').value;
    if (startCity === '' || endCity === '' || distance === '') {
        document.getElementById('error').innerHTML = 'Please fill all the fields';
        document.getElementById('error').style.color = 'red';
        return false;
    }
    else if(startCity.toLowerCase()===endCity.toLowerCase())
    {
        document.getElementById('error').innerHTML = 'Start and End City cannot be same';
        document.getElementById('error').style.color = 'red';
        return false;
    } else if (distance < 0) {
        document.getElementById('error').innerHTML = 'Distance cannot be negative';
        document.getElementById('error').style.color = 'red';
        return false;
    } else {
        document.getElementById('error').innerHTML = '';
        return true;
    }
}

function validateUpdateRoute() {
    const startCity = document.getElementById('updateStartCity').value;
    const endCity = document.getElementById('updateEndCity').value;
    const distance = document.getElementById('updateDistance').value;
    if (startCity === '' || endCity === '' || distance === '') {
        document.getElementById('updateError').innerHTML = 'Please fill all the fields';
        document.getElementById('updateError').style.color = 'red';
        return false;
    }
    else if(startCity.toLowerCase()===endCity.toLowerCase())
    {
        document.getElementById('updateError').innerHTML = 'Start and End City cannot be same';
        document.getElementById('updateError').style.color = 'red';
        return false;
    }
    else if (distance < 0) {
        document.getElementById('updateError').innerHTML = 'Distance cannot be negative';
        document.getElementById('updateError').style.color = 'red';
        return false;
    } else {
        document.getElementById('updateError').innerHTML = '';
        return true;
    }
}

function addRoute() {
    if (!validateAddRoute()) {
        return;
    }
    const startCity = document.getElementById('startCity').value;
    const endCity = document.getElementById('endCity').value;
    const distance = document.getElementById('distance').value;
    const token = localStorage.getItem('token');
    fetch('https://localhost:7067/api/AdminRoute/AddRouteInfo', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token,
        },
        body: JSON.stringify({
            startCity: startCity,
            endCity: endCity,
            distance: distance
        })
    })
    .then(response => response.json())
    .then(data => {
        if(data.errorCode){
            displayRoutes();
            $('#addRouteModal').modal('hide');
            showAlert(data.errorMessage, 'danger');
            
        }
        else if(data.errors)
        {
            displayRoutes();
            $('#addRouteModal').modal('hide');
            showAlert("Something Went Wrong Please Try again", 'danger');
        }
        else {
            routes.push(data);
            dummyRoutes.push(data);
            displayRoutes();
            $('#addRouteModal').modal('hide');
            showAlert('Route added successfully!', 'success');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Failed to add route. Please try again.', 'danger');
    });
}

function updateRoute() {
    if (!validateUpdateRoute()) {
        return;
    }
    const id = document.getElementById('updateRouteId').value;
    const startCity = document.getElementById('updateStartCity').value;
    const endCity = document.getElementById('updateEndCity').value;
    const distance = document.getElementById('updateDistance').value;
    const token = localStorage.getItem('token');
    fetch('https://localhost:7067/api/AdminRoute/UpdateRouteInfo', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token,
        },
        body: JSON.stringify({
            routeId: id,
            startCity: startCity,
            endCity: endCity,
            distance: distance
        })
    })
    .then(response => response.json())
    .then(data => {
        if(data.errorCode)
        {
            displayRoutes();
            $('#updateRouteModal').modal('hide');
            showAlert(data.errorMessage, 'danger');
        }
        else if(data.errors)
        {
            displayRoutes();
            $('#updateRouteModal').modal('hide');
            showAlert("Something Went Wrong Please Try again", 'danger');
        }
        else{
            const index = routes.findIndex(route => route.routeId == id);
            if (index !== -1) {
                routes[index] = data;
                dummyRoutes[index] = data;
                displayRoutes();
                $('#updateRouteModal').modal('hide');
                showAlert('Route updated successfully!', 'success');
            } else {
                showAlert('Failed to update route. Please try again.', 'danger');
            }
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Failed to update route. Please try again.', 'danger');
    });
}

function deleteRoute(id) {
    const token = localStorage.getItem('token');
    fetch('https://localhost:7067/api/AdminRoute/DeleteRouteInfo/' + id, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer ' + token,
        },
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, 'danger');
        }
        else if(data.errors)
        {
            showAlert("Something Went Wrong Please Try again", 'danger');
        }
        else
        {
            console.log("deleted");
            getRoutes();
            showAlert('Route deleted successfully!', 'success');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Failed to delete route. Please try again.', 'danger');
    });
}

function editRoute(id) {
    const route = routes.find(route => route.routeId == id);
    if (route) {
        document.getElementById('updateRouteId').value = route.routeId;
        document.getElementById('updateStartCity').value = route.startCity;
        document.getElementById('updateEndCity').value = route.endCity;
        document.getElementById('updateDistance').value = route.distance;
        $('#updateRouteModal').modal('show');
    }
}

function displayRoutes() {
    const routesList = document.getElementById('routesList');
    if(routes.length<=0)
    {
        routesList.innerHTML = '<h3>No Routes Found</h3>';
        return;
    }
    routesList.innerHTML = '';
    routes.forEach(route => {
        const listItem = document.createElement('li');
        listItem.className = 'list-group-item ';
        listItem.innerHTML = `
            <div class="item1">
                <strong>Route ID:</strong> ${route.routeId}<br>
            </div>
            <div class="item2">
                <strong>Start City:</strong> ${route.startCity}<br>
            </div>
            <div class="item2">
                <strong>End City:</strong> ${route.endCity}<br>
            </div>
            <div class="item3">
                <strong>Distance:</strong> ${route.distance} km
            </div>
            <div class="btns-div">
                <button class="btn btn-primary btn-sm" onclick="editRoute('${route.routeId}')">Edit</button>
                <button class="btn btn-danger btn-sm" onclick="deleteRoute('${route.routeId}')">Delete</button>
            </div>
        `;
        routesList.appendChild(listItem);
    });
}

document.getElementById('addRoute').addEventListener('click', addRoute);
document.getElementById('updateRoute').addEventListener('click', updateRoute);

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

// Initially display all routes
displayRoutes();