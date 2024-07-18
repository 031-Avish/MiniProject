let bookings = [];
        let filteredBookings = [];

        document.getElementById('searchInput').addEventListener('input', function() {
            const value = this.value.toLowerCase();
            filteredBookings = bookings.filter(booking => {
                return booking.paymentStatus.toLowerCase().includes(value) ||
                       booking.bookingStatus.toLowerCase().includes(value);
            });
            displayBookings();
        });

        document.getElementById('sortSelect').addEventListener('change', function() {
            const value = this.value;
            if (value === 'idA') {
                filteredBookings.sort((a, b) => a.bookingId - b.bookingId);
            } else if (value === 'idD') {
                filteredBookings.sort((a, b) => b.bookingId - a.bookingId);
            } else if (value === 'nameAsc') {
                filteredBookings.sort((a, b) => a.passengers[0].name.localeCompare(b.passengers[0].name));
            } else if (value === 'nameDesc') {
                filteredBookings.sort((a, b) => b.passengers[0].name.localeCompare(a.passengers[0].name));
            }
            displayBookings();
        });

        document.getElementById('filterSelect').addEventListener('change', function() {
            const value = this.value;
            const filterInputContainer = document.getElementById('filterInputContainer');
            filterInputContainer.innerHTML = '';

            if (value === 'default') {
                filteredBookings = bookings;
                displayBookings();
            } else if (value === 'bookingDate') {
                const input = document.createElement('input');
                input.type = 'date';
                input.classList.add('form-control');
                input.id = 'filterBookingDate';
                input.addEventListener('change', function() {
                    const value = this.value;
                    filteredBookings = bookings.filter(booking => booking.bookingDate.includes(value));
                    displayBookings();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'bookingId') {
                const input = document.createElement('input');
                input.type = 'number';
                input.classList.add('form-control');
                input.id = 'filterBookingId';
                input.placeholder = 'Booking ID';
                input.addEventListener('input', function() {
                    const value = this.value;
                    if (value === '') {
                        filteredBookings = bookings;
                    } else {
                        filteredBookings = bookings.filter(booking => booking.bookingId.toString() === value);
                    }
                    displayBookings();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'scheduleId') {
                const input = document.createElement('input');
                input.type = 'number';
                input.classList.add('form-control');
                input.id = 'filterScheduleId';
                input.placeholder = 'Schedule ID';
                input.addEventListener('input', function() {
                    const value = this.value;
                    if (value === '') {
                        filteredBookings = bookings;
                    } else {
                        filteredBookings = bookings.filter(booking => booking.scheduleId.toString() === value);
                    }
                    displayBookings();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'userId') {
                const input = document.createElement('input');
                input.type = 'number';
                input.classList.add('form-control');
                input.id = 'filterUserId';
                input.placeholder = 'User ID';
                input.addEventListener('input', function() {
                    const value = this.value;
                    if (value === '') {
                        filteredBookings = bookings;
                    } else {
                        filteredBookings = bookings.filter(booking => booking.userId.toString() === value);
                    }
                    displayBookings();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'flightDepartureDate') {
                const input = document.createElement('input');
                input.type = 'date';
                input.classList.add('form-control');
                input.id = 'filterFlightDepartureDate';
                input.addEventListener('change', function() {
                    const value = this.value;
                    filteredBookings = bookings.filter(booking => booking.flightDetails.departureTime.includes(value));
                    displayBookings();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'priceRange') {
                const input1 = document.createElement('input');
                input1.type = 'number';
                input1.classList.add('form-control');
                input1.id = 'filterPriceRange1';
                input1.placeholder = 'Min Price';
                const input2 = document.createElement('input');
                input2.type = 'number';
                input2.classList.add('form-control');
                input2.id = 'filterPriceRange2';
                input2.placeholder = 'Max Price';
                input1.addEventListener('input', function() {
                    const value1 = this.value;
                    const value2 = input2.value;
                    if (value1 === '' && value2 === '') {
                        filteredBookings = bookings;
                    } else {
                        filteredBookings = bookings.filter(booking => {
                            return (value1 === '' || booking.totalPrice >= value1) &&
                                   (value2 === '' || booking.totalPrice <= value2);
                        });
                    }
                    displayBookings();
                });
                input2.addEventListener('input', function() {
                    const value1 = input1.value;
                    const value2 = this.value;
                    if (value1 === '' && value2 === '') {
                        filteredBookings = bookings;
                    } else {
                        filteredBookings = bookings.filter(booking => {
                            return (value1 === '' || booking.totalPrice >= value1) &&
                                   (value2 === '' || booking.totalPrice <= value2);
                        });
                    }
                    displayBookings();
                });
                filterInputContainer.appendChild(input1);
                filterInputContainer.appendChild(input2);
            } else if (value === 'passengerName') {
                const input = document.createElement('input');
                input.type = 'text';
                input.classList.add('form-control');
                input.id = 'filterPassengerName';
                input.placeholder = 'Passenger Name';
                input.addEventListener('input', function() {
                    const value = this.value.toLowerCase();
                    if (value === '') {
                        filteredBookings = bookings;
                    } else {
                        filteredBookings = bookings.filter(booking => booking.passengers.some(p => p.name.toLowerCase().includes(value)));
                    }
                    displayBookings();
                });
                filterInputContainer.appendChild(input);
            }
        });

        function fetchBookings() {
            const token = localStorage.getItem('token');
            fetch('https://localhost:7067/api/Booking/GetAllBookingsByAdmin', {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + token,
                }
            })
            .then(response => response.json())
            .then(data => {
                bookings = data;
                filteredBookings = data;
                showAlert('Bookings fetched successfully', 'success');
                displayBookings();
            })
            .catch(error => {
                showAlert('Error fetching bookings'+error, 'danger');
                // console.error('Error:', error);
            });
        }

        function displayBookings() {
            const bookingsList = document.getElementById('bookingsList');
            bookingsList.innerHTML = '';
            filteredBookings.forEach(booking => {
                const listItem = document.createElement('li');
                listItem.classList.add('list-group-item');
                listItem.innerHTML = `
                    <div>
                        <strong>Booking ID:</strong> ${booking.bookingId}
                    </div>
                    <div>
                        <strong>Booking Status:</strong> ${booking.bookingStatus}
                    </div>
                    <div>
                        <strong>Booking Date:</strong> ${new Date(booking.bookingDate).toLocaleString()}
                    </div>
                    <div>
                        <strong>Payment Status:</strong> ${booking.paymentStatus}
                    </div>
                    <div>
                        <strong>Total Price:</strong> ${booking.totalPrice}
                    </div>
                    <div>
                        <strong>User ID:</strong> ${booking.userId}
                    </div>
                    <div>
                        <strong>Schedule ID:</strong> ${booking.scheduleId}
                    </div>
                    <div>
                        <strong>Flight Details:</strong>
                        <ul>
                            <li>Departure Time: ${new Date(booking.flightDetails.departureTime).toLocaleString()}</li>
                            <li>Reaching Time: ${new Date(booking.flightDetails.reachingTime).toLocaleString()}</li>
                            <li>Route ID: ${booking.flightDetails.routeId}</li>
                            <li>Flight ID: ${booking.flightDetails.flightId}</li>
                        </ul>
                    </div>
                    <div>
                        <strong>Passengers:</strong>
                        <ul>
                            ${booking.passengers.map(p => `<li>${p.name}, ${p.age} years old, ${p.gender}</li>`).join('')}
                        </ul>
                    </div>
                `;
                bookingsList.appendChild(listItem);
            });
        }

        fetchBookings();

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