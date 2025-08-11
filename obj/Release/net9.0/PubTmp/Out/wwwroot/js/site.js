var menu = document.getElementById('menu');
var close = document.getElementById('close');
var res = document.getElementById('res-menu');

menu.onclick = function() {
 
    res.style.width = '60%';
}

close.onclick = function() {
    res.style.width = '0%';
   
}

var gold = document.getElementById('gold');
var business = document.getElementById('business');
var cd = document.getElementById('cd');
var vehicle = document.getElementById('vehicle');
var head = document.getElementById('header');
var sub = document.getElementById('sub');
var detail = document.getElementById('detail-para');
var point1 = document.getElementById('point1');
var point2 = document.getElementById('point2');
var point3 = document.getElementById('point3');
/* var loanId = document.getElementById('loan-id'); */
var welcome = document.getElementsByClassName('welcome-container')[0];

// Get the data from the model
var goldData = document.querySelector('input[name="GoldData"]')?.value;
var businessData = document.querySelector('input[name="BusinessData"]')?.value;
var cdData = document.querySelector('input[name="CDData"]')?.value;
var vehicleData = document.querySelector('input[name="VehicleData"]')?.value;

// Function to set background image based on screen width
function setBackgroundImage(data, defaultImage) {
    if (window.innerWidth <= 780) {
        return data?.image2 || defaultImage;
    }
    return data?.image1 || defaultImage;
}

// Function to update background image on resize
function updateBackgroundImage() {
    console.log("Function called");
    console.log("gold:", gold, "business:", business, "cd:", cd, "vehicle:", vehicle, "welcome:", welcome);
    if (gold.classList.contains('active')) {
        console.log("Gold active");
        var data = goldData ? JSON.parse(goldData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/front-view-arrangement-economy-elements.jpg")})`;
    } else if (business.classList.contains('active')) {
        console.log("Business active");
        var data = businessData ? JSON.parse(businessData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/business-background.jpg")})`;
    } else if (cd.classList.contains('active')) {
        console.log("CD active");
        var data = cdData ? JSON.parse(cdData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/consumer-durables-background.jpg")})`;
    } else if (vehicle.classList.contains('active')) {
        console.log("Vehicle active");
        var data = vehicleData ? JSON.parse(vehicleData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/bike-background.jpg")})`;
    }
}

// Add resize event listener
window.addEventListener('resize', updateBackgroundImage);

// --- AUTO-CYCLING LOAN SECTIONS EVERY 5 SECONDS ---
var loanElements = [gold, business, cd, vehicle];
var currentIndex = 0;
var cycleInterval = null;
var pauseTimeout = null;

function cycleLoan() {
    loanElements[currentIndex].click();
    currentIndex = (currentIndex + 1) % loanElements.length;
}

function startCycle() {
    if (cycleInterval) clearInterval(cycleInterval);
    cycleInterval = setInterval(cycleLoan, 5000);
}

function pauseCycle() {
    if (cycleInterval) clearInterval(cycleInterval);
    if (pauseTimeout) clearTimeout(pauseTimeout);
    pauseTimeout = setTimeout(() => {
        startCycle();
    }, 5000);
}

startCycle();

// Add pauseCycle to all loan button click handlers

gold.onclick = function () {
    pauseCycle();
    gold.classList.add('active');
    business.classList.remove('active');
    cd.classList.remove('active');
    vehicle.classList.remove('active');

    if (goldData) {
        let allLoans;
        try {
            allLoans = JSON.parse(goldData);
        } catch (e) {
            console.error("Invalid JSON in goldData:", e);
            allLoans = null;
        }

        console.log("allLoans:", allLoans);

        let goldLoan = null;

        if (Array.isArray(allLoans)) {
            goldLoan = allLoans.find(loan =>
                loan.LoanType?.toLowerCase() === "gold" ||
                loan.loanType?.toLowerCase() === "gold" ||
                loan.LoanType?.toLowerCase() === "gold loan" ||
                loan.loanType?.toLowerCase() === "gold loan"
            );
        } else if (typeof allLoans === "object" && allLoans !== null) {
            // If it's a single object
            goldLoan = allLoans;
        }

        console.log("goldLoan:", goldLoan);

        if (goldLoan) {
            head.textContent = goldLoan.header || goldLoan.name || '';
            sub.textContent = goldLoan.subContent || '';
            welcome.style.backgroundImage = `url(${setBackgroundImage(goldLoan, "images/welcome/front-view-arrangement-economy-elements.jpg")})`;
        } else {
            console.warn("Gold loan data not found or not matching!");
        }
    }

    updateLoanDisplay("Gold Loan");
}

business.onclick = function () {
    pauseCycle();
    gold.classList.remove('active');
    business.classList.add('active');
    cd.classList.remove('active');
    vehicle.classList.remove('active');
    
    if (businessData) {
        var data = JSON.parse(businessData);
        head.textContent = data.header ;
        sub.textContent = data.subContent;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/business-background.jpg")})`;
    }
    updateLoanDisplay("Business Loan");
}

cd.onclick = function () {
    pauseCycle();
    gold.classList.remove('active');
    business.classList.remove('active');
    cd.classList.add('active');
    vehicle.classList.remove('active');
    
    if (cdData) {
        var data = JSON.parse(cdData);
        head.textContent = data.header;
        sub.textContent = data.subContent;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/consumer-durables-background.jpg")})`;
    } 
    
    // Only use database data for content and points
    updateLoanDisplay("CD Loan");
}

vehicle.onclick = function () {
    pauseCycle();
    gold.classList.remove('active');
    business.classList.remove('active');
    cd.classList.remove('active');
    vehicle.classList.add('active');
    
    if (vehicleData) {
        var data = JSON.parse(vehicleData);
        head.textContent = data.header;
        sub.textContent = data.subContent ;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/bike-background.jpg")})`;
    } 
    
    // Only use database data for content and points
    updateLoanDisplay("Vehicle Loan");
}

document.addEventListener('DOMContentLoaded', function() {
    const servicesLink = document.querySelector('.services-link');
    const servicesList = document.querySelector('.service-list');

    servicesLink.addEventListener('mouseover', function(e) {
        servicesList.style.opacity = '1';
        servicesList.style.display = 'flex';
    });

    servicesLink.addEventListener('mouseout', function(e) {
        servicesList.style.opacity = '0';
    });

    servicesList.addEventListener('mouseover', function(e) {
        servicesList.style.opacity = '1';
        servicesList.style.display = 'flex';
    });

    servicesList.addEventListener('mouseout', function(e) {
        servicesList.style.opacity = '0';
        servicesList.style.zIndex = '0';
        servicesList.style.display = 'none';
    });
});

document.addEventListener('DOMContentLoaded', function() {
    // Mobile menu toggle
    const menuBtn = document.getElementById('menu');
    const closeBtn = document.getElementById('close');
    const resMenu = document.getElementById('res-menu');
    
    menuBtn.addEventListener('click', function() {
        resMenu.style.width = '100%';
    });
    
    closeBtn.addEventListener('click', function() {
        resMenu.style.width = '0';
    });

    // Mobile services dropdown
const servicesLinks = document.querySelector('.services-link2');
const servicesList = document.querySelector('.service-list2');

servicesLinks.onclick = function () {
  const isActive = servicesList.classList.contains('active');

  if (isActive) {
    // Collapse
    servicesList.style.height = servicesList.scrollHeight + 'px'; // set current height to allow transition
    requestAnimationFrame(() => {
      servicesList.style.height = '0px';
    });
    servicesList.classList.remove('active');
  } else {
    // Expand
    servicesList.classList.add('active');
    const fullHeight = servicesList.scrollHeight + 'px';
    servicesList.style.height = fullHeight;

    // After transition ends, remove the fixed height so it adapts naturally
    servicesList.addEventListener(
      'transitionend',
      () => {
        if (servicesList.classList.contains('active')) {
          servicesList.style.height = 'auto';
        }
      },
      { once: true }
    );
  }
};

    
    });
 document.addEventListener('DOMContentLoaded', function() {
        const carousel = document.querySelector('.reviews-carousel');
        const cards = Array.from(document.querySelectorAll('.review-card'));
        const leftBtn = document.querySelector('.review-arrow.left');
        const rightBtn = document.querySelector('.review-arrow.right');
        
        // Function to update button visibility based on scroll position
        function updateButtonVisibility() {
            leftBtn.style.visibility = carousel.scrollLeft === 0 ? 'hidden' : 'visible';
            rightBtn.style.visibility = carousel.scrollLeft + carousel.clientWidth >= carousel.scrollWidth ? 'hidden' : 'visible';
        }

        leftBtn.addEventListener('click', function() {
            // Scroll left by the width of one card
            carousel.scrollBy({ left: -cards[0].offsetWidth - 20, behavior: 'smooth' }); // Subtracting gap
        });

        rightBtn.addEventListener('click', function() {
            // Scroll right by the width of one card
             carousel.scrollBy({ left: cards[0].offsetWidth + 20, behavior: 'smooth' }); // Adding gap
        });

        // Update button visibility on scroll
        carousel.addEventListener('scroll', updateButtonVisibility);

        // Initial update
        updateButtonVisibility();
    });
    
// --- LOAN DATA MANAGEMENT ---
var loanDataCache = {};

// Fetch loan data from API
async function fetchLoanData() {
    try {
        const response = await fetch('/Home/GetLoanData');
        const result = await response.json();
        
        if (result.success) {
            // Cache the loan data
            result.data.forEach(loan => {
                loanDataCache[loan.name.toLowerCase()] = loan;
            });
            console.log('Loan data loaded successfully:', loanDataCache);
        } else {
            console.error('Failed to load loan data:', result.message);
        }
    } catch (error) {
        console.error('Error fetching loan data:', error);
    }
}

// Get loan data by name
function getLoanData(loanName) {
    return loanDataCache[loanName.toLowerCase()] || null;
}

// Update loan display with data from database
function updateLoanDisplay(loanName) {
    const loan = getLoanData(loanName);
    
    if (loan) {
        /* loanId.value = loan.id */
        window.GetValue(loan.id);
        head.textContent = loan.name;
        detail.textContent = loan.content;
        // Print all points as <li> elements
        const pointsContainer = document.getElementById('points');
        pointsContainer.innerHTML = '';
        if (loan.points && loan.points.length > 0) {
            loan.points.forEach(point => {
                const li = document.createElement('li');
                li.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> ${point}`;
                pointsContainer.appendChild(li);
            });
        }

        document.getElementById('redirect').addEventListener('click', function() {
            var loanType = loan.name;;
            var loanId = loan.id; // No quotes around this!

            if (loanType && loanId && loanId != 0) {
                window.location.href = `/services/${loanType}/${loanId}`;
            } else {
                alert('Loan information not available.');
            }
        });

        document.getElementById('redirect2').addEventListener('click', function() {
            var loanType = loan.name;;
            var loanId = loan.id; // No quotes around this!

            if (loanType && loanId && loanId != 0) {
                window.location.href = `/services/${loanType}/${loanId}`;
            } else {
                alert('Loan information not available.');
            }
        });
    } else {
      
        head.textContent = '';
        detail.textContent = '';
        document.getElementById('points').innerHTML = '';
    }
}

// Initialize loan data when page loads
document.addEventListener('DOMContentLoaded', function() {
    fetchLoanData();
});
function animateCount(id, target) {
    $({ countNum: 0 }).animate({ countNum: target }, {
        duration: 2000,
        easing: 'swing',
        step: function () {
            $('#' + id).text(Math.floor(this.countNum));
        },
        complete: function () {
            $('#' + id).text(this.countNum);
        }
    });
}
$.ajax({
    url: 'http://pfl.yogloans.com:8580/api/count/get-count',
    method: "GET",
    success: function(response) {
        animateCount('Employee', parseInt(response.employee));
        animateCount('customer', parseInt(response.customer));
        animateCount('branch', parseInt(response.branch));
        $('#asset').text(response.asset); // Skip animation for text
    },
    error: function(xhr, status, error) {
        console.error("Error:", error);
    }
});
