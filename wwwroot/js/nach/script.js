$(document).ready(function () {
    const customer_id = $('#customer_id').val(); // jQuery way is cleaner

    if (!customer_id) {
        console.error("Customer ID is missing.");
        return;
    }

    // Show loader
    $('#form').html("<div class='loader'><div class='loader-spinner'></div></div>");

    $.ajax({
        url: "http://localhost:8085/api/enach/get-loans/" + customer_id,
        method: "GET",
        success: function (response) {
            if (!response || response.length === 0) {
                window.location.href = "/Onlineservices?loan=notfound";
                return;
            }

            console.log("Success:", response);

            const table = document.createElement("table");

            // Create header row
            const header = table.insertRow();
            const headers = ["Select", "Loan Number", "Loan Date", "Maturity Date"];
            headers.forEach(text => {
                const th = document.createElement("th");
                th.textContent = text;
                header.appendChild(th);
            });

            // Add rows for each record if multiple loans are present
            response.forEach((loan, idx) => {
                const row = table.insertRow();
                // Radio button cell
                const selectCell = row.insertCell();
                const label = document.createElement('label');
                label.className = 'switch-radio';
                const radio = document.createElement("input");
                radio.type = "radio";
                radio.name = "loanSelect";
                radio.value = loan.loan_no;
                radio.style.cursor = "pointer";
                // Custom logic for deselectable radio
                radio.addEventListener('mousedown', function(e) {
                    if (radio.checked) {
                        radio.wasChecked = true;
                    } else {
                        radio.wasChecked = false;
                    }
                });
                radio.addEventListener('click', function(e) {
                    if (radio.wasChecked) {
                        // Deselect
                        radio.checked = false;
                        document.getElementById('loan_no').value = '';
                        document.getElementById('nachStopBtn').disabled = true;
                        document.getElementById('nachStopBtn2').disabled = true;
                        radio.wasChecked = false;
                        e.preventDefault();
                        return false;
                    } else {
                        document.getElementById('loan_no').value = this.value;
                        document.getElementById('nachStopBtn').disabled = false;
                        document.getElementById('nachStopBtn2').disabled = false;
                    }
                });
                const slider = document.createElement('span');
                slider.className = 'slider';
                label.appendChild(radio);
                label.appendChild(slider);
                selectCell.appendChild(label);
                // Data cells
                const data = [loan.loan_no, loan.loan_dt, loan.maturiy_date];
                data.forEach(text => {
                    const td = row.insertCell();
                    td.textContent = text;
                });
            });

            // Clear previous content if any and append the new table
            const container = document.getElementById("form");
            container.innerHTML = ''; // clear old content
            container.appendChild(table);

            // Disable button if no radio is selected
            document.getElementById('nachStopBtn').disabled = true;
            document.getElementById('nachStopBtn2').disabled = true;
            document.getElementById('loan_no').value = '';
        },
        error: function (xhr, status, error) {
            // Remove loader
            $('#form').html('');
            window.location.href = "/Onlineservices?loan=notfound";
            $('#form').html('<p style="color:red;">Error loading loan information. Please try again.</p>');
        }
    });
});

function stop() {
    // Set today's date as min value
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('stop_date_input').setAttribute('min', today);
    document.getElementById('stop_date_input').value = today; // Optional: pre-fill with today

    // Show the modal
    document.getElementById('stopModal').style.display = 'flex';
}

function confirmStop() {
    const stopDate = document.getElementById('stop_date_input').value;
    const customerId = document.getElementById('customer_id').value;
    const loanNo = document.getElementById('loan_no').value;
    const mob_no = document.getElementById('mobile_no').value;

    if (!stopDate) {
        Swal.fire({
            icon: 'warning',
            title: 'Missing Date',
            text: 'Please select a stop date.'
        });
        return;
    }
    $('#stopModal').hide();

    const today = new Date().toISOString().split('T')[0];
    if (stopDate <= today) {
        Swal.fire({
            icon: 'warning',
            title: 'Invalid Date',
            text: 'Please select a future date.'
        });
        return;
    }

    // Hide the modal before showing SweetAlert2
    $('#stopModal').hide();

    Swal.fire({
        title: `Are you sure you want to stop the ENACH with stop date ${stopDate}?`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes, stop it!',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "http://localhost:8085/api/enach/stop",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    CustomerId: customerId,
                    LoanNo: loanNo,
                    MobileNo : mob_no,
                    StopDate: stopDate
                }),
                success: function(response) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: response.message || "Stop request successful!"
                    });
                },
                error: function(xhr) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: xhr.responseText || "Unknown error"
                    });
                }
            });
        } else {
            // If cancelled, show the modal again
            $('#stopModal').show();
        }
    });
}
