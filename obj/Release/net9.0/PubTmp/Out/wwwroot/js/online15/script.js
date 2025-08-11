$(document).ready(function () {
    var customer_id = document.getElementById('customer_id').value;


    fetch(`http://pfl.yogloans.com:8580/api/15g/find/${customer_id}`)
    .then(response => {
        if (!response.ok) {
            throw new Error("Unexpected error occurred");
        }
        return response.json();
    })
    .then(data => {
        if (data.exists) {
      /*       // Show the response in the popup
            document.getElementById('existMessage').innerText = `true ${JSON.stringify(data)}`; */
            document.getElementById('existPopup').style.display = 'flex';
        } else {
            // Customer does not exist — proceed with form submission or further logic
            console.log("Customer not found. Proceed.");
        }
    })
    .catch(error => {
        console.error("Error:", error);
    });

    $.ajax({
        url: "http://pfl.yogloans.com:8580/api/15g/get-application-data/" + customer_id,
        method: "GET",
        success: function (response) {
            console.log("Success:", response);

            const formHtml = `
<form id="form15g">
    <div>
        <label>Name of Assessee (Declarant):</label>
        <input type="text" name="assessee_name" value="${response.name}" readonly>
    </div>
    <div>
        <label>PAN of Assessee:</label>
        <input type="text" name="pan" value="${response.pan}" readonly>
    </div>
    <div>
        <label>Status:</label>
       <input type="text" name="status15g" value="${response.status}">

    </div>
    <div>
        <label>Residential Status:</label>
        <select name="residential_status" required>
            <option value="">--Select--</option>
            <option value="1">Resident</option>
            <option value="2">Non-Resident</option>
        </select>
    </div>
    <div>
        <label>Previous Year (P.Y):</label>
        <input type="text" name="previous_year" value="${response.py}" readonly>
    </div>
    <div>
        <label>Flat/Door/Block No:</label>
        <input type="text" name="flat_no" value="THERATTIL HOUSE" readonly>
    </div>
    <div>
        <label>Road/Street/Lane:</label>
        <input type="text" name="street" value="${response.block}" readonly>
    </div>
    <div>
        <label>Area/Locality:</label>
        <input type="text" name="locality" value="${response.location}" readonly>
    </div>
    <div>
        <label>Town/City:</label>
        <input type="text" name="city" value="${response.city}" readonly>
    </div>
    <div>
        <label>District:</label>
        <input type="text" name="district" value="${response.district}" readonly>
    </div>
    <div>
        <label>State:</label>
        <input type="text" name="state" value="${response.state}" readonly>
    </div>
    <div>
        <label>Pin:</label>
        <input type="text" name="pin" value="${response.pin}" readonly>
    </div>
    <div>
        <label>Mobile No:</label>
        <input type="text" name="mobile" value="${response.mobile}" readonly>
    </div>
       <div>
        <label>email:</label>
        <input type="text" name="email" value="" required>
    </div>
    <div >
        <label>Whether Assessed to Tax:</label>
        <input type="radio" name="assessed_to_tax" value="Yes">Yes
        <input type="radio" name="assessed_to_tax" value="No">No
    </div>
    <div>
        <label>Estimated income for which this declaration is made:</label>
        <input type="text" name="estimated_income" value="${response.estimate}" readonly>
    </div>
    <div>
        <label>Total interest projected and paid under CIF:</label>
        <input type="text" name="interest_paid" value="${response.interest}" readonly>
    </div>
    <div>
        <label>Estimated total income of P.Y:</label>
        <input type="text" name="estimated_total_income">
    </div>
    <div style="grid-column: span 2;">
        <h4>Details of Form No.15G other than this form filed during the previous year:</h4>
    </div><br>
    <div>
        <label>Total no of Form No.15G filed:</label>
        <input type="text" name="total_forms" value="${response.totalForms}" readonly>
    </div>
    <div>
        <label>Aggregate amount of income for which Form No.15G filed:</label>
        <input type="text" name="aggregate_income" value="${response.aggregate}" readonly>
    </div>
    <div style="grid-column: span 1;">
        <label>+ Details of income for which the declaration is filed:</label>
        <input type="text" name="income_details">
    </div>
    
    <div>
        <label>Date:</label>
        <input type="text" name="date" readonly id="todaydate">
    </div>
    <div>
        <label>Place:</label>
        <input type="text" name="place">
    </div>
    <div style="grid-column: span 2; display:flex; flex-direction:colum; justify-content:center; align-items:start;">
        <input type="checkbox" name="declaration_accepted" required>
        <label style="font-size:.98rem;">I have read and accepted the declaration provided by me is true and correct to the best of my knowledge</label>
    </div>
    <div class="buttons">
        <button type="submit">Submit</button>
        <button type="reset">Reset</button>
        <button type="button" onclick="window.location.href='cancel.aspx'">Cancel</button>
        <button type="button" onclick="redirectpdf_page()" disabled id="print-btn" style="cursor: not-allowed; background-color:white; color:gray;">Print</button>
    </div>
    <div >
        <input type="text" id="pantrack_hidden" name="pantrack" style="position:fixed; z-index:4000000;">
        <input type="text" id="pan_hidden" name="pan" style="position:fixed; z-index:4000000;">
    </div>
</form>
            `;

            $('#form').html(formHtml);

            const today = new Date();
            const day = String(today.getDate()).padStart(2, '0');
            const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            const month = monthNames[today.getMonth()];
            const year = today.getFullYear();
            const formattedDate = `${day}-${month}-${year}`;
            $('#todaydate').val(formattedDate);

            $('#form15g').on('submit', function (e) {
                e.preventDefault();

                // Collect form data
                var data = {
                    CustomerId: customer_id,
                    Residence: $('select[name="residential_status"]').val(),
                    Email: $('input[name="email"]').val(),
                    Telephone: $('input[name="mobile"]').val(),
                    TaxStatus: $('input[name="assessed_to_tax"]:checked').val(),
                    Est: parseFloat($('input[name="estimated_total_income"]').val()),
                    Status15G: $('input[name="status15g"]').val(),
                    Interest: parseFloat($('input[name="interest_paid"]').val()),
                    TotalForms: parseInt($('input[name="total_forms"]').val()),
                    Aggregate: parseFloat($('input[name="aggregate_income"]').val()),
                    EstimateInterest: parseFloat($('input[name="estimated_income"]').val()),
                    Place: $('input[name="place"]').val()
                };

                // Basic validation (optional)
                if (!data.Place || isNaN(data.Est) || isNaN(data.Interest)) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Validation Error',
                        text: 'Please fill all required fields correctly.'
                    });
                    return;
                }

                $.ajax({
                    url: "http://pfl.yogloans.com:8580/api/15g/submit-application",
                    method: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    success: function (response) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: response.message || "Form submitted successfully!"
                        });
                        $('#print-btn').prop('disabled', false);
                        $('#print-btn').css('cursor', 'pointer');
                        $('#print-btn').css('background-color', 'red');
                        $('#print-btn').css('color', 'white');
                        redirectvalues();
                    },
                    error: function (xhr) {
                        console.error(xhr);
                        let msg = "Submission failed: ";
                        if (xhr.responseJSON && xhr.responseJSON.Message) {
                            msg += xhr.responseJSON.Message;
                        } else if (xhr.responseText) {
                            msg += xhr.responseText;
                        } else {
                            msg += xhr.statusText;
                        }
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: msg
                        });
                    }
                });
            });
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
        }
    });

   
});
function redirectvalues() {
    var customer_id = document.getElementById('customer_id').value;
  
    $.ajax({
        url: "http://pfl.yogloans.com:8580/api/15g/get-pdf-data/" + customer_id, // ✅ route param
        method: "GET", // ✅ correct method
        contentType: "application/json",
        success: function(response) {
            console.log("success-response :", response);
            
            // Debug: Check if elements exist
            const pantrackElement = $('#pantrack_hidden');
            const panElement = $('#pan_hidden');
            
            console.log("pantrack element exists:", pantrackElement.length > 0);
            console.log("pan element exists:", panElement.length > 0);
            
            // Handle response as array - get first element
            const data = Array.isArray(response) ? response[0] : response;
            console.log("Extracted data:", data);
            
            if (pantrackElement.length > 0) {
                pantrackElement.val(data.pan_track_id);
                console.log("Set pantrack value to:", data.pan_track_id);
            } else {
                console.error("pantrack element not found!");
            }
            
            if (panElement.length > 0) {
                panElement.val(data.PancARdno);
                console.log("Set pan value to:", data.PancARdno);
            } else {
                console.error("pan element not found!");
            }
        },
        
        error: function(xhr) {
            console.error("error-response :", xhr.responseText);
        }
    });



}

function redirectpdf(){
    var pan_track_id = document.getElementById('pantrack_hidden').value;
    var pan = document.getElementById('pan_hidden').value;
    var j = "G"; // assuming "G" is the required value

    $.ajax({
        url: "http://pfl.yogloans.com:8580/api/15g/get-data",
        method: "GET",
        data: {
            pantrack: pan_track_id,
            pan: pan,
            j: j
        },
        success: function(response) {
            console.log("Success:", response);
        },
        error: function(xhr, status, error) {
            console.log("Error:", xhr.responseText);
        }
    });
}



function redirectpdf_page(){
    var customer_id = document.getElementById('customer_id').value;
    var pan_track_id = document.getElementById('pantrack_hidden').value;
    var pan = document.getElementById('pan_hidden').value;
    var j = "G"; 

    console.log("Redirecting to PDF with parameters:");
    console.log("customer_id:", customer_id);
    console.log("pan_track_id:", pan_track_id);
    console.log("pan:", pan);
    console.log("j:", j);

    var url = "http://localhost:5132/online15g/pdf?customer_id="+customer_id+"&pantrack="+pan_track_id+"&pan="+pan+"&j="+j;
    console.log("Redirect URL:", url);

    window.location.href = url;
}



   
