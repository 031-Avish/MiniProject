
let bookingData=[];
document.addEventListener("DOMContentLoaded", function() {
    const urlParams = new URLSearchParams(window.location.search);
    const bookingId = parseInt(urlParams.get('bookingId'));
    const bookingId1=parseInt(urlParams.get('bookingId1'));
    const token = localStorage.getItem('token');


    fetch(`https://localhost:7067/api/Booking/getBookingDetailsByUser/${bookingId}`, {
        method: 'GET',
        headers: {
            contentType: 'application/json',
            'Authorization': 'Bearer ' + token
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, "danger");
        } else {
             console.log(data);
            
            bookingData.push(data);

            if(isNaN(bookingId1))
            {
                displayTickets();
            }
        }
    })
    .catch(error => {
        showAlert('Error fetching booking details', 'danger');
        console.error('Error:', error);
    });
    console.log(bookingId);
    console.log(bookingId1);
    if(!isNaN(bookingId1)){
        fetch(`https://localhost:7067/api/Booking/getBookingDetailsByUser/${bookingId1}`, {
        method: 'GET',
        headers: {
            contentType: 'application/json',
            'Authorization': 'Bearer ' + token
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.errorCode) {
            showAlert(data.errorMessage, "danger");
        } else {
            bookingData.push(data);
            displayTickets()
        }
    })
    .catch(error => {
        showAlert('Error fetching booking details', 'danger');
        console.error('Error:', error);
    });
    }
});
function displayTickets() {
        
        const bookingDetailsDiv = document.getElementById('booking-details');
        bookingData.forEach(data => {
            
            data.passengers.forEach(passenger => {
                const ticketHTML = generateTicketHTML(data, passenger);
                bookingDetailsDiv.innerHTML += ticketHTML;
            });
        });
    }

    function generateTicketHTML(data, passenger) {
        console.log(data);
        const departureDate = new Date(data.flightDetails.departureTime).toLocaleDateString();
        const departureTime = new Date(data.flightDetails.departureTime).toLocaleTimeString();
        const arrivalDate = new Date(data.flightDetails.reachingTime).toLocaleDateString();
        const arrivalTime = new Date(data.flightDetails.reachingTime).toLocaleTimeString();
        return `
            <div class="row mb-5 tickets">
                <div class="col-9 out">
                    <div class="row">
                        <div class="col">
                            <h2 class="text-secondary mb-0 brand">Online Flight Booking</h2>
                        </div>
                        <div class="col">
                            <h2 class="mb-0">Normal CLASS</h2>
                        </div>
                    </div>
                    <hr>
                    <div class="row mb-3">
                        <div class="col-4">
                            <p class="head">Airline</p>
                            <p class="txt">${data.flightDetails.flightInfo.name}</p>
                        </div>
                        <div class="col-4">
                            <p class="head">From</p>
                            <p class="txt">${data.flightDetails.routeInfo.startCity}</p>
                        </div>
                        <div class="col-4">
                            <p class="head">To</p>
                            <p class="txt">${data.flightDetails.routeInfo.endCity}</p>
                        </div>
                    </div>
                    <div class="row mb-3">
                        <div class="col-4">
                            <p class="head">Passenger</p>
                            <p class="txt text-uppercase">${passenger.name}</p>
                        </div>
                        <div class="col-4">
                            <p class="head">Status</p>
                            <p class="txt text-uppercase">${data.bookingStatus}</p>
                        </div>
                        <div class="col-4">
                            <p class="head">Board Time</p>
                            <p class="txt">1 hour before Departure</p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-3">
                            <p class="head">Departure</p>
                            <p class="txt mb-1">${departureDate}</p>
                            <p class="h1  mb-3">${departureTime}</p>
                        </div>
                        <div class="col-3">
                            <p class="head">Arrival</p>
                            <p class="txt mb-1">${arrivalDate}</p>
                            <p class="h1  mb-3">${arrivalTime}</p>
                        </div>
                        <div class="col-3">
                            <p class="head">Gate</p>
                            <p class="txt">A22</p>
                        </div>
                        <div class="col-3">
                            <p class="head">Seat</p>
                            <p class="txt">At time of bording </p>
                        </div>
                    </div>
                </div>
                <div class="col-3 bord pl-0" style="background-color:#376b8d; padding:20px; border-top-right-radius: 25px; border-bottom-right-radius: 25px;">
                    <div class="row">
                        <div class="col">
                            <h2 class="text-light text-center brand">Online Flight Booking</h2>
                        </div>
                    </div>
                    <div class="row justify-content-center">
                        <div class="col-12">
                            <img src="../Media/qr-code.jpg" class="mx-auto d-block" height="180px" width="200px" alt="">
                        </div>
                    </div>
                    <div class="row">
                        <h3 class="text-light2 text-center mt-2 mb-0">
                            &nbsp; Thank you for choosing us. <br> <br>
                            Please be at the gate at boarding time
                        </h3>
                    </div>
                </div>
            </div>
        `;
    }

    function showAlert(message, type) {
        alert(`${type.toUpperCase()}: ${message}`);
    }