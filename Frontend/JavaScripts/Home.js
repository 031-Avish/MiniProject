
document.getElementById('logout').addEventListener('click', function(e) {
    e.preventDefault();
    localStorage.removeItem('token');
    window.location.href = "Login.html";
});
document.getElementById('login').addEventListener('click', function(e) {
    e.preventDefault();
    window.location.href = "Login.html";  
});