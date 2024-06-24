

function navbar() {
    if(localStorage.getItem('token')) {
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[0].style.display = 'none';
    }
    else{
        const ele = document.getElementById('login-logout');
        ele.getElementsByTagName('button')[1].style.display = 'none';
    }
  }
function loadTemplate() {
      fetch('Navbar.html')
          .then(response => response.text())
          .then(data => {
              const template = document.createElement('div');
              template.innerHTML = data;
              document.body.prepend(template.querySelector('template').content);
              navbar();  
          })
          .catch(error => console.error('Error loading template:', error));
  }
  window.addEventListener('DOMContentLoaded', loadTemplate);