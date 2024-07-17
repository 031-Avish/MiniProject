
const token = localStorage.getItem('token');
if (token === null) {
    alert("You are not authorized to view this page !! You are not logged in");
    window.location.href = "Login.html";
}

data = parseJwt(token);
function parseJwt (token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}

if(data.role !== "User"){
    alert("You are not authorized to view this page !! You are not an User");
    window.location.href = "Home.html";
}


