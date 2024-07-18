let payments = [];
        let filteredPayments = [];

        document.getElementById('searchInput').addEventListener('input', function() {
            const value = this.value.toLowerCase();
            filteredPayments = payments.filter(payment => {
                return payment.paymentStatus.toLowerCase().includes(value);
            });
            displayPayments();
        });

        document.getElementById('sortSelect').addEventListener('change', function() {
            const value = this.value;
            if (value === 'default') {
                filteredPayments = payments;
                console.log(filteredPayments);
            } else if (value === 'amountAsc') {
                filteredPayments.sort((a, b) => a.amount - b.amount);
            } else if (value === 'amountDesc') {
                filteredPayments.sort((a, b) => b.amount - a.amount);
            } else if (value === 'dateAsc') {
                filteredPayments.sort((a, b) => new Date(a.paymentDate) - new Date(b.paymentDate));
            } else if (value === 'dateDesc') {
                filteredPayments.sort((a, b) => new Date(b.paymentDate) - new Date(a.paymentDate));
            }
            console.log(payments);
            displayPayments();
        });

        document.getElementById('filterSelect').addEventListener('change', function() {
            const value = this.value;
            const filterInputContainer = document.getElementById('filterInputContainer');
            filterInputContainer.innerHTML = '';

            if (value === 'default') {
                filteredPayments = payments;
                displayPayments();
            } else if (value === 'date') {
                const input = document.createElement('input');
                input.type = 'date';
                input.classList.add('form-control');
                input.id = 'filterDate';
                input.addEventListener('change', function() {
                    const value = this.value;
                    filteredPayments = payments.filter(payment => payment.paymentDate.includes(value));
                    // console.log(filteredPayments);
                    // console.log(payments);
                    displayPayments();
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
                        filteredPayments = payments;
                    } else {
                        filteredPayments = payments.filter(payment => payment.bookingId.toString() === value);
                    }
                    displayPayments();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'paymentId') {
                const input = document.createElement('input');
                input.type = 'number';
                input.classList.add('form-control');
                input.id = 'filterPaymentId';
                input.placeholder = 'Payment ID';
                input.addEventListener('input', function() {
                    const value = this.value;
                    if (value === '') {
                        filteredPayments = payments;
                    } else {
                        filteredPayments = payments.filter(payment => payment.paymentId.toString() === value);
                    }
                    displayPayments();
                });
                filterInputContainer.appendChild(input);
            } else if (value === 'amountRange') {
                const input1 = document.createElement('input');
                input1.type = 'number';
                input1.classList.add('form-control');
                input1.id = 'filterAmount1';
                input1.placeholder = 'Min Amount';
                input1.addEventListener('input', function() {
                    const value1 = this.value;
                    const value2 = document.getElementById('filterAmount2').value;
                    if (value1 === '' && value2 === '') {
                        filteredPayments = payments;
                    } else {
                        filteredPayments = payments.filter(payment => (payment.amount >= value1 && payment.amount <= value2) || (value1 === '' && payment.amount <= value2) || (value2 === '' && payment.amount >= value1) );
                    }
                    displayPayments();
                });
                filterInputContainer.appendChild(input1);

                const input2 = document.createElement('input');
                input2.type = 'number';
                input2.classList.add('form-control');
                input2.id = 'filterAmount2';
                input2.placeholder = 'Max Amount';
                input2.addEventListener('input', function() {
                    const value1 = document.getElementById('filterAmount1').value;
                    const value2 = this.value;
                    if (value1 === '' && value2 === '') {
                        filteredPayments = payments;
                    } else {
                        filteredPayments = payments.filter(payment => (payment.amount >= value1 && payment.amount <= value2) || (value1 === '' && payment.amount <= value2) || (value2 === '' && payment.amount >= value1) );
                    }
                    displayPayments();
                });
                filterInputContainer.appendChild(input2);

}
        });

        function fetchPayments() {
            const token = localStorage.getItem('token');
            fetch('https://localhost:7067/api/Payment/GetAllPaymentsByAdmin', {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + token,
                }
            })
            .then(response => response.json())
            .then(data => {
                if (data.errorCode) {
                    showAlert(data.message, 'danger');
                } else if (data.errors) {
                    showAlert("Something went wrong! Please try again", 'danger');
                } else {
                    showAlert('Payment fetched successfully', 'success');
                    payments =[...data];
                    filteredPayments = data;
                    displayPayments();
                }
            })
            .catch(error => {
                showAlert('Error fetching payments'+error, 'danger');
                // console.error('Error:', error);
            });
        }

        function displayPayments() {
            const paymentsList = document.getElementById('paymentsList');
            paymentsList.innerHTML = '';
            filteredPayments.forEach(payment => {
                const listItem = document.createElement('li');
                listItem.classList.add('list-group-item');
                listItem.innerHTML = `
                    <div>
                        <strong>Payment ID:</strong> ${payment.paymentId}
                    </div>
                    <div>
                        <strong>Amount:</strong> ${payment.amount}
                    </div>
                    <div>
                        <strong>Payment Status:</strong> ${payment.paymentStatus}
                    </div>
                    <div>
                        <strong>Payment Date:</strong> ${new Date(payment.paymentDate).toLocaleString()}
                    </div>
                    <div>
                        <strong>Booking ID:</strong> ${payment.bookingId}
                    </div>
                `;
                paymentsList.appendChild(listItem);
            });
        }

        fetchPayments();
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