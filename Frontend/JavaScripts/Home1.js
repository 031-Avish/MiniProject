validateForm = (e) => {
    e.preventDefault(); 
    //    console.log('Validating form');
       let startCity = document.getElementById('startCity').value;
       let endCity = document.getElementById('endCity').value;
       let date = document.getElementById('date').value;
       if(startCity === '' || endCity === '' || date === ''){
           showAlert('All fields are required', 'danger');
           return false;
       }
       else if(startCity === endCity){
           showAlert('Starting and Ending cities cannot be the same', 'danger');
           return false;
       }
       else if(new Date(date) < new Date()){
           showAlert('Travel date cannot be in the past', 'danger');
           return false;
       }
      //  showAlert('Form submitted successfully', 'success');
      window.location.href = `Flights.html?startCity=${startCity}&endCity=${endCity}&date=${date}`;
       return true;
   }
  document.querySelector('form').onsubmit = validateForm;

   function showAlert(message, type) {
        document.getElementById('model-content').innerHTML = message;
        if(type === 'danger'){
            document.getElementById('exampleModalLabel').innerHTML = 'Error!';
            document.getElementById('modal-header').classList.remove('bg-success');
            document.getElementById('modal-header').classList.add('bg-danger');
        }
        else{
            document.getElementById('exampleModalLabel').innerHTML = 'Success!';
            document.getElementById('modal-header').classList.remove('bg-danger');
            document.getElementById('modal-header').classList.add('bg-success');
        }
        $('.modal').modal('show');
        setTimeout(() => {
            $('.modal').modal('hide');
        }, 3000); 
    }
       