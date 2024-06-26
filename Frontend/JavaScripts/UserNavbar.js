function LoadUserNavbar() {
    const token = localStorage.getItem('token');  
    if (token === null) {
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[1].style.display = 'none';
    }  
    const role = localStorage.getItem('role');
    if(role==="User") {
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[0].style.display = 'none';
    }
    else{
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[1].style.display = 'none';
    }

    document.getElementById('logout').addEventListener('click', function(e) {
        e.preventDefault();
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('role');
        window.location.href = "Login.html";
    });
    document.getElementById('login').addEventListener('click', function(e) {
        e.preventDefault();
        window.location.href = "Login.html";  
    });
};

function loadTemplate() {
    fetch('UserNavbar.html')
        .then(response => response.text())
        .then(data => {
            const template = document.createElement('div');
            template.innerHTML = data;
            document.body.prepend(template.querySelector('template').content);
            LoadUserNavbar(); 
        })

        .catch(error => console.error('Error loading template:', error));
}
window.addEventListener('DOMContentLoaded', loadTemplate);