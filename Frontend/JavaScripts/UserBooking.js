
document.addEventListener("DOMContentLoaded", getUpcomingBookings);

function getUpcomingBookings() {
    // Fetch upcoming bookings for the current user from API
    const userId = localStorage.getItem('userId'); // Assuming user ID is stored in localStorage
    const token = localStorage.getItem('token');
    const apiUrl = `https://localhost:7067/api/Booking/GetUpcomingFlightsByUser/${userId}`;

    fetch(apiUrl, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
    })
    .then(response => response.json())
    .then(bookings => {
        if(bookings.length==0){
            showAlert("No Bookings Found","danger");
        }else if (bookings.errorCode) {
            showAlert(bookings.errorMessage, "danger");
        } else if (bookings.errors) {
            showAlert("Some Unwanted Error Occurred", "danger");
        }
        else{
            showAlert("Bookings Fetched Successfully","success");
            const bookingsList = document.getElementById('bookingsList');
            bookingsList.innerHTML = '';
            const currentTime = new Date();
            // console.log('Upcoming bookings:', bookings);
            bookings.sort((a, b) => {
                const departureTimeA = new Date(a.flightDetails.departureTime);
                const departureTimeB = new Date(b.flightDetails.departureTime);

                if (departureTimeA > departureTimeB) {
                    return 1; // A comes before B (descending order)
                }
                if (departureTimeA < departureTimeB) {
                    return -1; // B comes before A (descending order)
                }
                return 0; // A and B are considered equal
                });
            bookings.forEach(booking => {
                const bookingCard = createBookingCard(booking);
                bookingsList.appendChild(bookingCard);
            });
        }
    })
    .catch(error => {
        showAlert('Error fetching upcoming bookings'+error, 'danger');
        // console.error('Error fetching upcoming bookings:', error);
    });

    // Function to create booking card
    function createBookingCard(booking) {
        const { bookingId, bookingStatus, bookingDate, paymentStatus, totalPrice, flightDetails, passengers } = booking;

        // Format date
        const formattedBookingDate = new Date(bookingDate).toLocaleDateString();

        // Create card element
        const card = document.createElement('div');
        card.classList.add('booking-card');
        const departureTime = new Date(flightDetails.departureTime);
        const currentTime = new Date();

        card.innerHTML = `
            <h5>Booking ID: ${bookingId}</h5>
            <p>Status: ${bookingStatus}</p>
            <p>Booking Date: ${formattedBookingDate}</p>
            <p>Payment Status: ${paymentStatus}</p>
            <p>Total Price: $${totalPrice}</p>
            <p>Flight Details:</p>
            <ul>
                <li>Departure Time: ${departureTime.toLocaleString()}</li>
                <li>Reaching Time: ${new Date(flightDetails.reachingTime).toLocaleString()}</li>
                <li>Route: ${flightDetails.routeInfo.startCity} to ${flightDetails.routeInfo.endCity}</li>
                <li>Flight Name: ${flightDetails.flightInfo.name}</li>
            </ul>
            <p>Passengers:</p>
            <ul>
                ${passengers.map(passenger => `<li>${passenger.name} (${passenger.age} years, ${passenger.gender})</li>`).join('')}
            </ul>
            <div class="booking-actions">
                ${departureTime > currentTime ? `<button class="btn btn-danger mr-2" onclick="confirmCancellation(${bookingId})">Cancel Ticket</button>` : ''}
                <a class="btn btn-primary" href="ticket.html?bookingId=${bookingId}" target="_blank">Print Ticket</a>
            </div>
        `;

        return card;
    }
}

// Function to handle cancellation confirmation
window.confirmCancellation = function(bookingId) {
    console.log('Cancelling booking:', bookingId);
    // Show confirmation modal
    $('#cancelModal').modal('show');

    // Handle cancellation on confirmation
    document.getElementById('confirmCancel').onclick = function() {
        // Perform cancellation logic (API call)
        const apiUrl = `https://localhost:7067/api/Booking/CancelBookingByUser/${bookingId}`;
        fetch(apiUrl, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify({
                bookingId: bookingId,
                refundAmount: 3500,  // Assuming refund amount is fixed for simplicity
                message: "Cancellation successful. Refund will be credited in the next 3 days."
            })
        })
        .then(response => response.json())
        .then(data => {
             if (data.errorCode) {
                showAlert(data.errorMessage, "danger");
            } else if (data.errors) {
                showAlert("Some Unwanted Error Occurred", "danger");
            }
            else{
            const resultMessage = `Message: ${data.message}<br>Refund Amount: $${data.refundAmount}`;
            document.getElementById('resultMessage').innerHTML = resultMessage;
            $('#resultModal').modal('show');

            getUpcomingBookings(); 
            } // Refresh upcoming bookings list
        })
        .catch(error => {
            showAlert('Error cancelling booking'+error, 'danger');
            // console.error('Error cancelling booking:', error);
        });

        // Close modal
        $('#cancelModal').modal('hide');
    };
};
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
