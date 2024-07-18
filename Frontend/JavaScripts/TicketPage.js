let bookingData=[];
        document.addEventListener("DOMContentLoaded", function() {
            const urlParams = new URLSearchParams(window.location.search);
            const bookingId = parseInt(urlParams.get('BookingId'));
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
                    bookingData.push(data);
                    displayBookingDetails(data);
                    handleBookingActions(data);
                }
            })
            .catch(error => {
                showAlert('Error fetching booking details', 'danger');
                console.error('Error:', error);
            });
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
                    displayBookingDetails(data);
                    handleBookingActions(data);
                }
            })
            .catch(error => {
                showAlert('Error fetching booking details', 'danger');
                console.error('Error:', error);
            });
            }

            function displayBookingDetails(data) {
                const bookingDetailsDiv = document.getElementById('booking-details');
                bookingDetailsDiv.innerHTML += `
                    <h3>Booking ID: ${data.bookingId}</h3>
                    <p><strong>Status:</strong> ${data.bookingStatus}</p>
                    <p><strong>Booking Date:</strong> ${new Date(data.bookingDate).toLocaleString()}</p>
                    <p><strong>Payment Status:</strong> ${data.paymentStatus}</p>
                    <p><strong>Total Price:</strong> ${data.totalPrice}</p>
                    <h4>Flight Details</h4>
                    <p><strong>Departure Time:</strong> ${new Date(data.flightDetails.departureTime).toLocaleString()}</p>
                    <p><strong>Reaching Time:</strong> ${new Date(data.flightDetails.reachingTime).toLocaleString()}</p>
                    <h4>Passengers</h4>
                    ${data.passengers.map(p => `
                        <div class="ticket">
                            <p><strong>Name:</strong> ${p.name}</p>
                            <p><strong>Age:</strong> ${p.age}</p>
                            <p><strong>Gender:</strong> ${p.gender}</p>
                            
                        </div>
                    `).join('')}
                `;
            }

            function handleBookingActions(data) {
                const bookingActionsDiv = document.getElementById('booking-actions');
                if (data.bookingStatus === 'Completed' && data.paymentStatus === 'Success') {
                    bookingActionsDiv.innerHTML = `
                        <a class="btn btn-primary" id="print-btn" >Print Ticket</a>
                        <button class="btn btn-danger" id="cancel-btn">Cancel Ticket</button>
                    `;
                    document.getElementById('print-btn').addEventListener('click', function() {
                        if (bookingData.length === 1) {
                            window.location.href = `ticket.html?bookingId=${bookingData[0].bookingId}`;
                        } else {
                            window.location.href = `ticket.html?bookingId=${bookingData[0].bookingId}&bookingId1=${bookingData[1].bookingId}`;
                        }
                        
                    });
                    document.getElementById('cancel-btn').addEventListener('click', function() {
                        cancelBooking(data.bookingId);
                    });
                } else if (data.bookingStatus !== 'Completed') {
                    bookingActionsDiv.innerHTML = `
                        <button class="btn btn-primary" id="rebook-btn">Rebook Ticket</button>
                    `;
                    document.getElementById('rebook-btn').addEventListener('click', function() {
                        rebookTicket(data);
                    });
                }
            }

            function cancelBooking(bookingId) {
                fetch(`https://localhost:7067/api/Booking/CancelBooking/${bookingId}`, {
                    method: 'POST',
                    headers: {
                        'Authorization': 'Bearer ' + token
                    }
                })
                .then(response => response.json())
                .then(data => {
                    if (data.errorCode) {
                        showAlert(data.errorMessage, "danger");
                    } else {
                        showAlert('Booking cancelled successfully', 'success');
                        location.reload();
                    }
                })
                .catch(error => {
                    showAlert('Error cancelling booking', 'danger');
                    console.error('Error:', error);
                });
            }

            function rebookTicket(bookingData) {
                const urlParams = new URLSearchParams({
                    scheduleId: bookingData.scheduleId,
                    userId: bookingData.userId,
                    passengers: JSON.stringify(bookingData.passengers)
                }).toString();
                window.location.href = `bookingPage.html?${urlParams}`;
            }

            function showAlert(message, type) {
                alert(`${type.toUpperCase()}: ${message}`);
            }
        });