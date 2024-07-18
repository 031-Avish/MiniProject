document.addEventListener("DOMContentLoaded", function() {
    const urlParams = new URLSearchParams(window.location.search);
    const scheduleId = parseInt(urlParams.get('scheduleId'));
    const scheduleId1 = parseInt(urlParams.get('scheduleId1'));
    console.log(scheduleId);
    console.log(scheduleId1);
    const userId = parseInt(localStorage.getItem('userId'));
    const token = localStorage.getItem('token');
    let flight=[];
    console.log(isNaN(scheduleId1));
    if(isNaN(scheduleId1)){
        flight.push(JSON.parse(localStorage.getItem('flight')));
    }else{
        flight=JSON.parse(localStorage.getItem('connectingflight'));
    }
    console.log(flight);
    displayFlightDetails(flight);
    let passengers = [];
    // Add passenger
    document.getElementById('add-passenger-btn').addEventListener('click', function() {
        const passengerList = document.getElementById('passenger-list');
        const passengerCount = passengerList.children.length + 1;
        const passengerDiv = document.createElement('div');
        passengerDiv.classList.add('form-group', 'passenger');
        passengerDiv.innerHTML = `
            <h4>Passenger ${passengerCount}</h4>
            <label for="passenger-name-${passengerCount}">Name:</label>
            <input type="text" class="form-control" id="passenger-name-${passengerCount}" required>
            <label for="passenger-age-${passengerCount}">Age:</label>
            <input type="number" class="form-control" id="passenger-age-${passengerCount}" required>
            <label for="passenger-gender-${passengerCount}">Gender:</label>
            <select class="form-control" id="passenger-gender-${passengerCount}" required>
                <option value="Male">Male</option>
                <option value="Female">Female</option>
            </select>
            <button type="button" class="btn btn-danger mt-2 remove-passenger-btn">Remove Passenger</button>
        `;
        passengerList.appendChild(passengerDiv);
        calculateTotalPrice();
        updateRemoveButtons();
    });
function displayFlightDetails(data) {

    const flightResults = document.getElementById("flightResults");
    flightResults.innerHTML = "";
    data.forEach((flight) => {
    const flightCard = document.createElement("div");
    flightCard.classList.add("each-flight-div-box", "show");
  flightCard.innerHTML = `
    <div class="each-flight-div" onclick="media_click(this)">
        <div class="flight-company">
            <div class="flight-icon">
                <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" aria-hidden="true" focusable="false" width="1.5em" height="1.3em" style="-ms-transform: rotate(360deg); -webkit-transform: rotate(360deg); transform: rotate(360deg);" preserveAspectRatio="xMidYMid meet" viewBox="0 0 440 384">
                    <path d="M14 335h405v43H14v-43zm417.5-199.5q3.5 12.5-3 24T409 175l-114 30l-92 25l-114 30l-34 10l-16-29l-39-67l31-9l42 33l106-28L91 17l41-11l147 137l113-30q13-4 24.5 3t15 19.5z" fill="#434445"/>
                    <rect x="0" y="0" width="440" height="384" fill="rgba(0, 0, 0, 0)"/>
                </svg>
            </div>
            <div class="company-details">
                <div class="company-name">${
                  flight.flightInfo.name
                }</div>
                
            </div>
        </div>
        <div class="flight-time flight-time-div">
            <div class="flight-origin-time">
                <div class="flight-time">
                    <h5>${
                      new Date(flight.departureTime)
                        .toLocaleString()
                        .split(",")[1]
                    }</h5>
                </div>
                <div class="flight-place">
                    ${flight.routeInfo.startCity}
                </div>
            </div>
            <div class="flight-stops tooltip">
                <svg xmlns="http://www.w3.org/2000/svg" width="34" height="24" viewBox="0 0 24 24">
                    <path d="M13,9.03544443 C14.6961471,9.27805926 16,10.736764 16,12.5 C16,14.263236 14.6961471,15.7219407 13,15.9645556 L13,21.5207973 C13,21.7969397 12.7761424,22.0207973 12.5,22.0207973 C12.2238576,22.0207973 12,21.7969397 12,21.5207973 L12,15.9645556 C10.3038529,15.7219407 9,14.263236 9,12.5 C9,10.736764 10.3038529,9.27805926 12,9.03544443 L12,3.5 C12,3.22385763 12.2238576,3 12.5,3 C12.7761424,3 13,3.22385763 13,3.5 L13,9.03544443 L13,9.03544443 Z M12.5,15 C13.8807119,15 15,13.8807119 15,12.5 C15,11.1192881 13.8807119,10 12.5,10 C11.1192881,10 10,11.1192881 10,12.5 C10,13.8807119 11.1192881,15 12.5,15 Z" transform="rotate(90 12.5 12.51)"/>
                </svg>
                
            </div>
            <div class="flight-destination-time">
                <div class="flight-time">
                    <h5>${
                      new Date(flight.reachingTime)
                        .toLocaleString()
                        .split(",")[1]
                    }</h5>
                </div>
                <div class="flight-place">
                    ${flight.routeInfo.endCity}
                </div>
            </div>
        </div>
      
    </div>
`;
  flightResults.appendChild(flightCard);
});
}

    // Remove passenger
    document.addEventListener('click', function(event) {
        if (event.target && event.target.classList.contains('remove-passenger-btn')) {
            event.target.closest('.passenger').remove();
            calculateTotalPrice();
            updateRemoveButtons();
        }
    });

    function updateRemoveButtons() {
        const removeButtons = document.querySelectorAll('.remove-passenger-btn');
        if (removeButtons.length === 0) {
            document.getElementById('add-passenger-btn').disabled = false;
        } else {
            document.getElementById('add-passenger-btn').disabled = false;
        }
    }

    function calculateTotalPrice(){
        const passengerList = document.getElementById('passenger-list');
        if(passengerList.children.length===0){
            document.getElementById('totalPrice').innerHTML=0;
            return;
        }
        if(flight.length===1){
            document.getElementById('totalPrice').
            innerHTML=`${passengerList.children.length*flight[0].price}`;
        }else{
            document.getElementById('totalPrice').
            innerHTML=`${passengerList.children.length*(flight[0].price+flight[1].price)}`;
        }
    }

    // Book ticket
    document.getElementById('book-ticket-btn').addEventListener('click', async function() {
        const passengerList = document.getElementById('passenger-list');
        passengers = [];
        for (let i = 0; i < passengerList.children.length; i++) {
            const passengerDiv = passengerList.children[i];
            const name = passengerDiv.querySelector(`#passenger-name-${i + 1}`).value;
            const age = passengerDiv.querySelector(`#passenger-age-${i + 1}`).value;
            const gender = passengerDiv.querySelector(`#passenger-gender-${i + 1}`).value;

            if (!name || !age || !gender) {
                showAlert('All passenger fields are required', 'danger');
                return;
            }

            passengers.push({ name, age, gender });
        }

        if (passengers.length === 0) {
            showAlert('At least one passenger is required', 'danger');
            return;
        }
        // Check if there is any passenger whose age is more than 2
        const hasPassengerAboveAge2 = passengers.some(passenger => passenger.age > 10);

        if (!hasPassengerAboveAge2) {
            showAlert('At least adult passenger is required','danger');
            return;
        }
        console.log(scheduleId, userId, passengers);
        let bookingDetails = [];
        if(isNaN(scheduleId1)){
            bookingDetails =[ {
                scheduleId: scheduleId,
                userId: userId,
                passengers: passengers
            }];

        }else{
            bookingDetails =[{
                scheduleId: scheduleId,
                userId: userId,
                passengers: passengers
            },
            {
                scheduleId: scheduleId1,
                userId: userId,
                passengers: passengers
            }];
        }
        let bookingData = [];
        fetch('https://localhost:7067/api/Booking/BookFlightByUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify(bookingDetails)
        })
        .then(response => response.json())
        .then( (data) => {
            if (data.errorCode) {
                showAlert(data.errorMessage, "danger");
            } else if (data.errors) {
                showAlert("Some Unwanted Error Occurred", "danger");
            } else {
                console.log(data);
                showAlert('Booking Successful', 'success');
                data.forEach( (booking)=>{
                     makePayment(booking.bookingId, booking.totalPrice);
                });
                setTimeout(() => {
                    window.location.href = `TicketPage.html?BookingId=${data[0].bookingId}&bookingId1=${data[1]?.bookingId}`;
                }, 4000);
                // window.location.href = `TicketPage.html?BookingId=${data[0].bookingId}&bookingId1=${data[1]?.bookingId}`;

            }
        })
        .catch(error => {
            if (error.errorMessage) {
                showAlert(error.errorMessage, "danger");
                setTimeout(() => {
                    window.location.href = `TicketPage.html?BookingId=${data[0].bookingId}&bookingId1=${data[1]?.bookingId}`;
                }, 4000);
                // window.location.href = `TicketPage.html?BookingId=${data[0].bookingId}&bookingId1=${data[1]?.bookingId}`;
            } else {
                showAlert('Some unwanted error occurred', 'danger');
            }
        });
        
        
    });

    function showBookingPreview(data) {
        const bookingPreview = document.getElementById('booking-preview');
        const bookingDetailsDiv = document.getElementById('booking-details');
        bookingDetailsDiv.innerHTML = `
            <p><strong>Booking ID:</strong> ${data[0].bookingId}</p>
            <p><strong>Booking Status:</strong> ${data[0].bookingStatus}</p>
            <p><strong>Booking Date:</strong> ${new Date(data[0].bookingDate).toLocaleString()}</p>
            <p><strong>Payment Status:</strong> ${data[0].paymentStatus}</p>
            <p><strong>Total Price:</strong> ${data[0].totalPrice}</p>
            <h4>Flight Details</h4>
            <p><strong>Departure Time:</strong> ${new Date(data[0].flightDetails.departureTime).toLocaleString()}</p>
            <p><strong>Reaching Time:</strong> ${new Date(data[0].flightDetails.reachingTime).toLocaleString()}</p>
            <h4>Passengers</h4>
            ${data[0].passengers.map(passenger => `
                <p><strong>Name:</strong> ${passenger.name}</p>
                <p><strong>Age:</strong> ${passenger.age}</p>
                <p><strong>Gender:</strong> ${passenger.gender}</p>
            `).join('')}
        `;
        bookingPreview.style.display = 'block';
    }

    function makePayment(bookingId, totalPrice) {
        const paymentDetails = {
            bookingId: bookingId,
            amount: totalPrice
        };

        fetch('https://localhost:7067/api/Payment/ProcessPaymentByUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify(paymentDetails)
        })
        .then(response => response.json())
        .then(data => {
             if (data.errorCode) {
                showAlert(data.errorMessage, "danger");
                throw data.errorMessage;
                // window.location.href = `TicketPage.html?BookingId=${bookingId}`
            }
            else if (data.errors) {
                showAlert('Some unwanted error occurred', 'danger');
                throw data.errors;
            } else {
                showAlert('Payment Successful', 'success');
                // window.location.href = `TicketPage.html?BookingId=${bookingId}`
            }
        })
        .catch(error => {
            showAlert('Error making payment !! Try Again', 'danger');
            console.error('Error:', error);
            throw error;
        });
    }

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
});