window.addEventListener('load', function() {
    if(localStorage.getItem('token')) {
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[0].style.display = 'none';
    }
    else{
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[1].style.display = 'none';
    }
});

document.getElementById('logout').addEventListener('click', function(e) {
    e.preventDefault();
    localStorage.removeItem('token');
    window.location.href = "Login.html";
});
document.getElementById('login').addEventListener('click', function(e) {
    e.preventDefault();
    window.location.href = "Login.html";  
});