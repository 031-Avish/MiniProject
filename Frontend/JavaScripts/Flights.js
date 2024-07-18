let flights = [];
      let connectingFlights=[];
      var vars = {};
      document.addEventListener("DOMContentLoaded", function () {
        
        var parts = window.location.href.replace(
          /[?&]+([^=&]+)=([^&]*)/gi,
          function (m, key, value) {
            vars[key] = value;
          }
        );
        const ISODate = new Date(vars.date).toISOString();
        // console.log(ISODate);
        vars = { ...vars, date: ISODate };

        document.getElementById("fromInput").value = vars.startCity;
        document.getElementById("toInput").value = vars.endCity;
        document.getElementById("dateInput").value = vars.date.split("T")[0];
        console.log(vars);
        getFlights(vars);
      });
      function getConnectingFlights(){
        const token = localStorage.getItem("token");
        fetch(
          "https://localhost:7067/api/ScheduleFlight/GetConnectingFlightsByUser",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              // Authorization: "Bearer " + token,
            },
            body: JSON.stringify(vars),
          }
        )
          .then((response) => response.json())
          .then((data) => {
            if (data.errorCode) {
              showAlert(data.errorMessage, "danger");
            } else if (data.error) {
              showAlert("Some Unwanted Error Occurred", "danger");
              // console.log(data.error);
            } else {
              showAlert("Flight Details Fetched Successfully", "success");
              connectingFlights = data;
              console.log(data);
              displayConnectingFlights(data);
            }
          })
          .catch((error) => {
            showAlert("Error:"+error, "danger");
            // console.error('Error:', error);
          });

      }
      function getFlights(vars) {
        const token = localStorage.getItem("token");
        fetch(
          "https://localhost:7067/api/ScheduleFlight/GetFlightDetailsOnDateByUser",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json"
              // Authorization: "Bearer " + token,
            },
            body: JSON.stringify(vars),
          }
        )
          .then((response) => response.json())
          .then((data) => {
            if (data.errorCode) {
              showAlert(data.errorMessage, "danger");
            } else if (data.error) {
              showAlert("Some Unwanted Error Occurred", "danger");
              // console.log(data.error);
            } else {
              showAlert("Flight Details Fetched Successfully", "success");
              flights = data;
              // console.log(data);
              displayFlights(data);
            }
          })
          .catch((error) => {
            showAlert("error Ocured "+error  , "danger");
            // console.error('Error:', error);
          });
      }

      function showAlert(message, type) {
        document.getElementById("model-content").innerHTML = message;
        if (type === "danger") {
          document.getElementById("exampleModalLabel").innerHTML = "Error!";
          document
            .getElementById("modal-header")
            .classList.remove("bg-success");
          document.getElementById("modal-header").classList.add("bg-danger");
        } else {
          document.getElementById("exampleModalLabel").innerHTML = "Success!";
          document.getElementById("modal-header").classList.remove("bg-danger");
          document.getElementById("modal-header").classList.add("bg-success");
        }
        $(".modal").modal("show");
        setTimeout(() => {
          $(".modal").modal("hide");
        }, 3000);
      }
      function displayConnectingFlights(data){
        const connectingflightResults = document.getElementById("connectingflightResults");
        connectingflightResults.innerHTML = "";
        data.forEach((flight)=>{
          const flightCard = document.createElement("div");
          flightCard.classList.add("each-flight-div-box", "show");

         

          flightCard.innerHTML=`<div class="each-flight-div" onclick="media_click(this)">
                <div class="flight-company">
                    <div class="flight-icon">
                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" aria-hidden="true" focusable="false" width="1.5em" height="1.3em" style="-ms-transform: rotate(360deg); -webkit-transform: rotate(360deg); transform: rotate(360deg);" preserveAspectRatio="xMidYMid meet" viewBox="0 0 440 384">
                            <path d="M14 335h405v43H14v-43zm417.5-199.5q3.5 12.5-3 24T409 175l-114 30l-92 25l-114 30l-34 10l-16-29l-39-67l31-9l42 33l106-28L91 17l41-11l147 137l113-30q13-4 24.5 3t15 19.5z" fill="#434445"/>
                            <rect x="0" y="0" width="440" height="384" fill="rgba(0, 0, 0, 0)"/>
                        </svg>
                    </div>
                    <div class="company-details">
                        <div class="company-name">${
                          flight[0].flightInfo.name
                        }</div>
                        
                    </div>
                </div>
                <div class="flight-time flight-time-div">
                    <div class="flight-origin-time">
                        <div class="flight-time">
                            <h5>${
                              new Date(flight[0].departureTime)
                                .toLocaleString()
                                .split(",")[1]
                            }</h5>
                        </div>
                        <div class="flight-place">
                            ${flight[0].routeInfo.startCity}
                        </div>
                    </div>
                    <div class="flight-stops tooltip">
                        <svg xmlns="http://www.w3.org/2000/svg" width="34" height="24" viewBox="0 0 24 24">
                            <path d="M13,9.03544443 C14.6961471,9.27805926 16,10.736764 16,12.5 C16,14.263236 14.6961471,15.7219407 13,15.9645556 L13,21.5207973 C13,21.7969397 12.7761424,22.0207973 12.5,22.0207973 C12.2238576,22.0207973 12,21.7969397 12,21.5207973 L12,15.9645556 C10.3038529,15.7219407 9,14.263236 9,12.5 C9,10.736764 10.3038529,9.27805926 12,9.03544443 L12,3.5 C12,3.22385763 12.2238576,3 12.5,3 C12.7761424,3 13,3.22385763 13,3.5 L13,9.03544443 L13,9.03544443 Z M12.5,15 C13.8807119,15 15,13.8807119 15,12.5 C15,11.1192881 13.8807119,10 12.5,10 C11.1192881,10 10,11.1192881 10,12.5 C10,13.8807119 11.1192881,15 12.5,15 Z" transform="rotate(90 12.5 12.51)"/>
                        </svg>
                        
                    </div>
                    <div class="flight-destination-time">
                        <div class="flight-time">
                            <h5>${
                              new Date(flight[0].reachingTime)
                                .toLocaleString()
                                .split(",")[1]
                            }</h5>
                        </div>
                        <div class="flight-place">
                            ${flight[0].routeInfo.endCity}
                        </div>
                    </div>
                </div>
                <div class="flight-details">
                    <div class="flight-price">
                        <h5>₹ ${flight[0].price}</h5>
                    </div>
                    
                </div>
            </div>
            <div class="each-flight-div" onclick="media_click(this)">
                <div class="flight-company">
                    <div class="flight-icon">
                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" aria-hidden="true" focusable="false" width="1.5em" height="1.3em" style="-ms-transform: rotate(360deg); -webkit-transform: rotate(360deg); transform: rotate(360deg);" preserveAspectRatio="xMidYMid meet" viewBox="0 0 440 384">
                            <path d="M14 335h405v43H14v-43zm417.5-199.5q3.5 12.5-3 24T409 175l-114 30l-92 25l-114 30l-34 10l-16-29l-39-67l31-9l42 33l106-28L91 17l41-11l147 137l113-30q13-4 24.5 3t15 19.5z" fill="#434445"/>
                            <rect x="0" y="0" width="440" height="384" fill="rgba(0, 0, 0, 0)"/>
                        </svg>
                    </div>
                    <div class="company-details">
                        <div class="company-name">${
                          flight[1].flightInfo.name
                        }</div>
                        
                    </div>
                </div>
                <div class="flight-time flight-time-div">
                    <div class="flight-origin-time">
                        <div class="flight-time">
                            <h5>${
                              new Date(flight[1].departureTime)
                                .toLocaleString()
                                .split(",")[1]
                            }</h5>
                        </div>
                        <div class="flight-place">
                            ${flight[1].routeInfo.startCity}
                        </div>
                    </div>
                    <div class="flight-stops tooltip">
                        <svg xmlns="http://www.w3.org/2000/svg" width="34" height="24" viewBox="0 0 24 24">
                            <path d="M13,9.03544443 C14.6961471,9.27805926 16,10.736764 16,12.5 C16,14.263236 14.6961471,15.7219407 13,15.9645556 L13,21.5207973 C13,21.7969397 12.7761424,22.0207973 12.5,22.0207973 C12.2238576,22.0207973 12,21.7969397 12,21.5207973 L12,15.9645556 C10.3038529,15.7219407 9,14.263236 9,12.5 C9,10.736764 10.3038529,9.27805926 12,9.03544443 L12,3.5 C12,3.22385763 12.2238576,3 12.5,3 C12.7761424,3 13,3.22385763 13,3.5 L13,9.03544443 L13,9.03544443 Z M12.5,15 C13.8807119,15 15,13.8807119 15,12.5 C15,11.1192881 13.8807119,10 12.5,10 C11.1192881,10 10,11.1192881 10,12.5 C10,13.8807119 11.1192881,15 12.5,15 Z" transform="rotate(90 12.5 12.51)"/>
                        </svg>
                        
                    </div>
                    <div class="flight-destination-time">
                        <div class="flight-time">
                            <h5>${
                              new Date(flight[1].reachingTime)
                                .toLocaleString()
                                .split(",")[1]
                            }</h5>
                        </div>
                        <div class="flight-place">
                            ${flight[1].routeInfo.endCity}
                        </div>
                    </div>
                </div>
                <div class="flight-details">
                    <div class="flight-price">
                        <h5>₹ ${flight[1].price}</h5>
                    </div>
                    <div class="flight-details-btn">
                        <button class="btn btn-primary" onclick="bookConnectingFlight(${
                          flight[0].scheduleId
                        },${flight[1].scheduleId})">Book Now</button>
                    </div>
                </div>
            </div>
            `
            connectingflightResults.appendChild(flightCard);
      });
    }
      function displayFlights(data) {
        const flightResults = document.getElementById("flightResults");
        flightResults.innerHTML = "";

        data.forEach((flight) => {
          const flightCard = document.createElement("div");
          flightCard.classList.add("each-flight-div-box", "show");

          const departureTime = new Date(
            flight.departureTime
          ).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
          const arrivalTime = new Date(flight.arrivalTime).toLocaleTimeString(
            [],
            { hour: "2-digit", minute: "2-digit" }
          );

          flightCard.innerHTML = `
            <div class="each-flight-div" onclick="media_click(this)">
                <div class="flight-company">
                    <div class="flight-icon">
                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" aria-hidden="true" focusable="false" width="1.5em" height="1.3em" style="-ms-transform: rotate(360deg); -webkit-transform: rotate(360deg); transform: rotate(360deg);" preserveAspectRatio="xMidYMid meet" viewBox="0 0 440 384">
                            <path d="M14 335h405v43H14v-43zm417.5-199.5q3.5 12.5-3 24T409 175l-114 30l-92 25l-114 30l-34 10l-16-29l-39-67l31-9l42 33l106-28L91 17l41-11l147 137l113-30q13-4 24.5 3t15 19.5z" fill="#434445"/>
                            <rect x="0" y="0" width="440" height="384" fill="rgba(0, 0, 0, 0)"/>
                        </svg>
                    </div>
                    <div class="company-details">
                        <div class="company-name">${
                          flight.flightInfo.name
                        }</div>
                        
                    </div>
                </div>
                <div class="flight-time flight-time-div">
                    <div class="flight-origin-time">
                        <div class="flight-time">
                            <h5>${
                              new Date(flight.departureTime)
                                .toLocaleString()
                                .split(",")[1]
                            }</h5>
                        </div>
                        <div class="flight-place">
                            ${flight.routeInfo.startCity}
                        </div>
                    </div>
                    <div class="flight-stops tooltip">
                        <svg xmlns="http://www.w3.org/2000/svg" width="34" height="24" viewBox="0 0 24 24">
                            <path d="M13,9.03544443 C14.6961471,9.27805926 16,10.736764 16,12.5 C16,14.263236 14.6961471,15.7219407 13,15.9645556 L13,21.5207973 C13,21.7969397 12.7761424,22.0207973 12.5,22.0207973 C12.2238576,22.0207973 12,21.7969397 12,21.5207973 L12,15.9645556 C10.3038529,15.7219407 9,14.263236 9,12.5 C9,10.736764 10.3038529,9.27805926 12,9.03544443 L12,3.5 C12,3.22385763 12.2238576,3 12.5,3 C12.7761424,3 13,3.22385763 13,3.5 L13,9.03544443 L13,9.03544443 Z M12.5,15 C13.8807119,15 15,13.8807119 15,12.5 C15,11.1192881 13.8807119,10 12.5,10 C11.1192881,10 10,11.1192881 10,12.5 C10,13.8807119 11.1192881,15 12.5,15 Z" transform="rotate(90 12.5 12.51)"/>
                        </svg>
                        
                    </div>
                    <div class="flight-destination-time">
                        <div class="flight-time">
                            <h5>${
                              new Date(flight.reachingTime)
                                .toLocaleString()
                                .split(",")[1]
                            }</h5>
                        </div>
                        <div class="flight-place">
                            ${flight.routeInfo.endCity}
                        </div>
                    </div>
                </div>
                <div class="flight-details">
                    <div class="flight-price">
                        <h5>₹ ${flight.price}</h5>
                    </div>
                    <div class="flight-details-btn">
                        <button class="btn btn-primary" onclick="bookFlight(${
                          flight.scheduleId
                        })">Book Now</button>
                    </div>
                </div>
            </div>
        `;
          flightResults.appendChild(flightCard);
        });
      }

      function modifySearchResult(event) {
        event.preventDefault();
        const from = document.getElementById("fromInput").value;
        const to = document.getElementById("toInput").value;
        const date = document.getElementById("dateInput").value;
       if(from === '' || to === '' || date === ''){
           showAlert('All fields are required', 'danger');
           return false;
       }
       else if(from === to){
           showAlert('Starting and Ending cities cannot be the same', 'danger');
           return false;
       }
       else if(new Date(date) < new Date()){
           showAlert('Travel date cannot be in the past', 'danger');
           return false;
       }
        // console.log(from, to, date);

        // Redirect to the search page with query parameters
        window.location.href = `./flights.html?startCity=${from}&endCity=${to}&date=${date}`;
      }
     
      document.getElementById("searchForm").addEventListener("submit", modifySearchResult);

      document.getElementById("priceRange").addEventListener("input", filterFlights);
      
      function filterFlights() {
        const priceRange = document.getElementById("priceRange").value;
        console.log(priceRange);
        const filteredFlights = flights.filter((flight) => {
          return flight.price <= priceRange;
        });
        displayFlights(filteredFlights);
      }
      function updatePriceRange(value) {
            document.getElementById('minPrice').textContent = 2000;
            document.getElementById('maxPrice').textContent = value;
          }
      

      function filterFlightsByTime(){
        const morningCheck = document.getElementById("morningCheck").checked;
        const dayCheck = document.getElementById("dayCheck").checked;
        const nightCheck = document.getElementById("nightCheck").checked;
        /* console.log(morningCheck, dayCheck, nightCheck); */
        if(!morningCheck && !dayCheck && !nightCheck) 
          return displayFlights(flights);

        const filteredFlights = flights.filter((flight) => {
          return (
            (morningCheck && flight.departureTime.split("T")[1].split(":")[0] < 12) ||
            (dayCheck && flight.departureTime.split("T")[1].split(":")[0] >= 12 && flight.departureTime.split("T")[1].split(":")[0] < 18) ||
            (nightCheck && flight.departureTime.split("T")[1].split(":")[0] >= 18)
          );
        });
        displayFlights(filteredFlights);
      }
      document.getElementById("morningCheck").addEventListener("change", filterFlightsByTime);
      document.getElementById("dayCheck").addEventListener("change", filterFlightsByTime);
      document.getElementById("nightCheck").addEventListener("change", filterFlightsByTime);

      document.getElementById("sortSelect").addEventListener("change", sortFlights);

      function sortFlights(){
        const sortSelect = document.getElementById("sortSelect").value;
        tempFlights=[...flights];
        if(sortSelect === "early"){
          tempFlights.sort((a, b) => new Date(a.departureTime) - new Date(b.departureTime));
        } else if(sortSelect === "late"){
          tempFlights.sort((a, b) => new Date(b.departureTime) - new Date(a.departureTime));
        }
        else
        {
          tempFlights=[...flights];
        }
        displayFlights(tempFlights);
      }

      function bookConnectingFlight(scheduleId1,scheduleId2) {
        connectingFlights.find((flight) => {
          console.log(flight);
          if (flight[0].scheduleId === scheduleId1 && flight[1].scheduleId === scheduleId2) {
            localStorage.removeItem("flight");
            localStorage.setItem("connectingflight", JSON.stringify(flight));
          }
        });
        window.location.href = `./Booking.html?scheduleId=${scheduleId1}&scheduleId1=${scheduleId2}`;
      }
      function bookFlight(scheduleId) {
        flights.find((flight) => {
          if (flight.scheduleId === scheduleId) {
            localStorage.removeItem("connectingflight");
            localStorage.setItem("flight", JSON.stringify(flight));
          }
        });
        window.location.href = `./Booking.html?scheduleId=${scheduleId}`;
      }