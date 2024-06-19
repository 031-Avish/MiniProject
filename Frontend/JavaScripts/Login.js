// function to Register the User
document.querySelector('#register-user').addEventListener("click", (e)=>{
    e.preventDefault(); 
    document.getElementById('error').innerHTML = "";
    if (validateRegistationForm()) {
        const email = document.getElementById('registerEmail').value;
        const name = document.getElementById('registerName').value;
        const phone = document.getElementById('registerPhone').value;
        const password = document.getElementById('registerPassword').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const data = {
            email,
            name,
            phone,
            password,
            confirmPassword
        };
        fetch('https://localhost:7067/api/UserLoginRegister/Register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            return response.json();
        }).then((data) => {
            if (data.errorCode) {
                document.getElementById('error').innerHTML = data.errorMessage;
                document.getElementById('error').style.color = 'Red';
            } else {
                console.log(data);
                alert(`${data.name} Your Registration is Successful Please Login to Continue`);
                window.location.href = "Login.html";
            }
        }).catch((error) => {
            console.log(error);
            alert("Some Error Occured Please Try Again");
            window.location.href = "Login.html";
        });
    }
    else{
        document.getElementById('error').innerHTML = "Please Enter All the Details Correctly";
        document.getElementById('error').style.color = 'Red';
    }
});


// function to Login the User
document.querySelector('#login-user').addEventListener("click", (e)=>{
    e.preventDefault();
    document.getElementById('loginerror').innerHTML = "";
    const id =parseInt(document.getElementById('loginId').value);
    const password = document.getElementById('loginPassword').value;
    if(id === "" || password === ""){
        document.getElementById('loginerror').innerHTML = "Please Enter All the Details Correctly";
        document.getElementById('loginerror').style.color = 'Red';
        return;
    }
    const data = {
        UserId: id,
        Password: password
    };
    fetch('https://localhost:7067/api/UserLoginRegister/Login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    }).then((response) => {
        return response.json();
    }).then((data) => {
        if (data.errorCode) {
            document.getElementById('loginerror').innerHTML = data.errorMessage;
            document.getElementById('loginerror').style.color = 'Red';
        }else if(data.errors){
            document.getElementById('loginerror').innerHTML = "Some Unwanted Error Occured Please Try Again";
            document.getElementById('loginerror').style.color = 'Red';
        }else{
            console.log(data);
            localStorage.setItem('token', data.token);
            alert(`Your Login is Successful`);
            window.location.href = "Home.html";
        }
    }).catch((error) => {
        console.log(error);
        alert("Some Error Occured Please Try Again");
        window.location.href = "Login.html";
    });
});


// function to validate the Registration Form
function validateRegistationForm()
{
    const email = document.getElementById('registerEmail').value;
    const name = document.getElementById('registerName').value;
    const phone = document.getElementById('registerPhone').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const phoneRegex = /^\d+$/;

    if (!emailRegex.test(email)) {
        return false;
    }
    if (name.length < 3) {
        return false;
    }
    if (!phoneRegex.test(phone) || phone.length !== 10) {
        return false;
    }
    if (password.length < 6) {
        return false;
    }
    if (password !== confirmPassword) {
        return false;
    }
    return true;
}


document.getElementById('registerEmail').addEventListener('blur', function() {
    const email = document.getElementById('registerEmail').value;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (emailRegex.test(email)) {
        document.getElementById('registerEmail').style.borderColor = 'green';
        document.getElementById('remail').innerHTML = "";
    } else {
        document.getElementById('registerEmail').style.borderColor = 'red';
        document.getElementById('remail').innerHTML = "Enter Correct email";
        document.getElementById('remail').style.color = 'Red';
    }
});

document.getElementById('registerName').addEventListener('blur', function() {
    const registerName = document.getElementById('registerName').value;
    if (registerName.length >= 3) {
        document.getElementById('registerName').style.borderColor = 'green';
        document.getElementById('rname').innerHTML = "";
    } else {
        document.getElementById('registerName').style.borderColor = 'red';
        document.getElementById('rname').innerHTML = "Enter Correct Name at least 3 character";
        document.getElementById('rname').style.color = 'Red';
    }
});
document.getElementById('registerPhone').addEventListener('blur', function() {
    const phone = document.getElementById('registerPhone').value;
    const phoneRegex = /^\d+$/;
    if (phoneRegex.test(phone) && phone.length === 10) {
        document.getElementById('registerPhone').style.borderColor = 'green';
        document.getElementById('rphone').innerHTML = "";
    } else {
        document.getElementById('registerPhone').style.borderColor = 'red';
        document.getElementById('rphone').innerHTML = "Enter Correct 10 digit Phone Number";
        document.getElementById('rphone').style.color = 'Red';
    }
});
document.getElementById('registerPassword').addEventListener('blur', function() {
    const password = document.getElementById('registerPassword').value;
    if (password.length >= 6) {
        document.getElementById('registerPassword').style.borderColor = 'green';
        document.getElementById('rpassword').innerHTML = "";
    } else {
        document.getElementById('registerPassword').style.borderColor = 'red';
        document.getElementById('rpassword').innerHTML = "Enter More then 5 digit Password";
        document.getElementById('rpassword').style.color = 'Red';
    }
});
document.getElementById('confirmPassword').addEventListener('blur', function() {
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    if (password === confirmPassword) {
        document.getElementById('confirmPassword').style.borderColor = 'green';
        document.getElementById('rcpassword').innerHTML = "";
    } else {
        document.getElementById('confirmPassword').style.borderColor = 'red';
        document.getElementById('rcpassword').innerHTML = "Password does not match";
        document.getElementById('rcpassword').style.color = 'Red';
    }
});