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
    if (gold.classList.contains('active')) {
        var data = goldData ? JSON.parse(goldData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/front-view-arrangement-economy-elements.jpg")})`;
    } else if (business.classList.contains('active')) {
        var data = businessData ? JSON.parse(businessData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/business-background.jpg")})`;
    } else if (cd.classList.contains('active')) {
        var data = cdData ? JSON.parse(cdData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/consumer-durables-background.jpg")})`;
    } else if (vehicle.classList.contains('active')) {
        var data = vehicleData ? JSON.parse(vehicleData) : null;
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/bike-background.jpg")})`;
    }
}

// Add resize event listener
window.addEventListener('resize', updateBackgroundImage);

gold.onclick = function () {
    gold.classList.add('active');
    business.classList.remove('active');
    cd.classList.remove('active');
    vehicle.classList.remove('active');
    
    if (goldData) {
        var data = JSON.parse(goldData);
        head.textContent = data.header || "Gold Loan";
        sub.textContent = data.subContent || "Experience Door Step Gold Loan Services";
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/front-view-arrangement-economy-elements.jpg")})`;
        detail.textContent = data.detail || "A Gold Loan is a convenient and secure way to access instant funds by pledging your gold ornaments as collateral. With minimal documentation and quick processing, it is an ideal solution for urgent financial needs such as business expansion, medical emergencies, or education expenses. The loan amount is determined based on the purity and weight of the gold, offering competitive interest rates and flexible repayment options to suit your needs.";
    } else {
        head.textContent = "Gold Loan";
        sub.textContent = "Experience Door Step Gold Loan Services";
        welcome.style.backgroundImage = `url(${setBackgroundImage(null, "images/welcome/front-view-arrangement-economy-elements.jpg")})`;
        detail.textContent = "A Gold Loan is a convenient and secure way to access instant funds by pledging your gold ornaments as collateral. With minimal documentation and quick processing, it is an ideal solution for urgent financial needs such as business expansion, medical emergencies, or education expenses. The loan amount is determined based on the purity and weight of the gold, offering competitive interest rates and flexible repayment options to suit your needs.";
    }
    
    point3.style.display = "block";
    point1.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Quick Processing`;
    point2.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Minimal Documentation`;
    point3.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Competitive Rates`;
}

business.onclick = function () {
    gold.classList.remove('active');
    business.classList.add('active');
    cd.classList.remove('active');
    vehicle.classList.remove('active');
    
    if (businessData) {
        var data = JSON.parse(businessData);
        head.textContent = data.header || "Business Loan";
        sub.textContent = data.subContent || "Loan Provided Minimum 5 Members And Maximum 1 Member";
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/business-background.jpg")})`;
        detail.textContent = data.detail || "Business Loan is a small value unsecured loan granted to individuals engaged in retail trade for development of their business for augmenting the working capital and/or for meeting capital expenditure.";
    } else {
        head.textContent = "Business Loan";
        sub.textContent = "Loan Provided Minimum 5 Members And Maximum 1 Member";
        welcome.style.backgroundImage = `url(${setBackgroundImage(null, "images/welcome/business-background.jpg")})`;
        detail.textContent = "Business Loan is a small value unsecured loan granted to individuals engaged in retail trade for development of their business for augmenting the working capital and/or for meeting capital expenditure.";
    }
    
    point3.style.display = "block";
    point1.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Low interest rate`;
    point2.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Unsecured upto Rs.200000 for eligible customers`;
    point3.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Daily collection facility`;
}

cd.onclick = function () {
    gold.classList.remove('active');
    business.classList.remove('active');
    cd.classList.add('active');
    vehicle.classList.remove('active');
    
    if (cdData) {
        var data = JSON.parse(cdData);
        head.textContent = data.header || "Consumer Durable Loan";
        sub.textContent = data.subContent || "The Second Co-Applicant Shall Be A Person From The Same Group";
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/consumer-durables-background.jpg")})`;
        detail.textContent = data.detail || "";
    } else {
        head.textContent = "Consumer Durable Loan";
        sub.textContent = "The Second Co-Applicant Shall Be A Person From The Same Group";
        welcome.style.backgroundImage = `url(${setBackgroundImage(null, "images/welcome/consumer-durables-background.jpg")})`;
        detail.textContent = "";
    }
    
    point1.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Low interest rate.`;
    point2.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Unsecured upto Rs.200000 for eligible customers`;
    point3.style.display = "none";
}

vehicle.onclick = function () {
    gold.classList.remove('active');
    business.classList.remove('active');
    cd.classList.remove('active');
    vehicle.classList.add('active');
    
    if (vehicleData) {
        var data = JSON.parse(vehicleData);
        head.textContent = data.header || "Vehicle Loan";
        sub.textContent = data.subContent || "Finance Facility Upto Onroad Price For Eligible Customers";
        welcome.style.backgroundImage = `url(${setBackgroundImage(data, "images/welcome/bike-background.jpg")})`;
        detail.textContent = data.detail || "A Vehicle Loan is a convenient and secure way to access instant funds by pledging your vehicle as collateral. With minimal documentation and quick processing, it is an ideal solution for urgent financial needs such as business expansion, medical emergencies, or education expenses. The loan amount is determined based on the value of the vehicle, offering competitive interest rates and flexible repayment options to suit your needs.";
    } else {
        head.textContent = "Vehicle Loan";
        sub.textContent = "Finance Facility Upto Onroad Price For Eligible Customers";
        welcome.style.backgroundImage = `url(${setBackgroundImage(null, "images/welcome/bike-background.jpg")})`;
        detail.textContent = "A Vehicle Loan is a convenient and secure way to access instant funds by pledging your vehicle as collateral. With minimal documentation and quick processing, it is an ideal solution for urgent financial needs such as business expansion, medical emergencies, or education expenses. The loan amount is determined based on the value of the vehicle, offering competitive interest rates and flexible repayment options to suit your needs.";
    }
    
    point3.style.display = "block";
    point1.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Low interest rate.`;
    point2.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Loan amount upto Rs.200000.`;
    point3.innerHTML = `<img src="images/icon/checklist.png" alt="" srcset=""> Maximum duration 48 month.`;
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
    
// --- AUTO-CYCLING LOAN SECTIONS EVERY 5 SECONDS ---
(function() {
    var loanElements = [gold, business, cd, vehicle];
    var currentIndex = 0;
    function cycleLoan() {
        loanElements[currentIndex].click();
        currentIndex = (currentIndex + 1) % loanElements.length;
    }
    setInterval(cycleLoan, 5000);
})();
    