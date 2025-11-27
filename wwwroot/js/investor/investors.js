var report = $('#annual-reports');
var Announcements = $('#announcement');
var InvestorsContact = $('#Investors-Contact');
var PublicDisclosure  = $('#Public-Disclosure ');
var appointmentorder = $('#appointment-order');
var FormsTDS = $('Forms-TDS');
$(document).ready(function() {
    /* Annual Report */
    report.html("");
    $.ajax({
        url : "/annual-report",
        method : "GET",
        success : function(response){
            console.log(response); // Shows the array
            if (Array.isArray(response) && response.length > 0) {
                response.forEach(function(item) {
                    $('#annual-reports').append(`
                        <div class="reports view-pdf" data-src="${item.filePath || ''}">
                            <div class="img">
                                <img src="images/icon/pdf.png" alt="PDF Icon">
                            </div>
                            <div class="detail">
                                <p>
                                    ${item.title}
                                </p>
                            </div>
                        </div>
                    `);
                });
            } else {
                console.warn("Response is empty or not an array");
            }
        }
    });
/* Announcements */
Announcements.html("");
$.ajax({
    url : "/Announcements",
    method : "GET",
    success : function(response){
        console.log(response); // Shows the array
        if (Array.isArray(response) && response.length > 0) {
            response.forEach(function(item) {
                $('#announcement').append(`
                    <div class="reports view-pdf" data-src="${item.filePath || ''}">
                        <div class="img">
                            <img src="images/icon/pdf.png" alt="PDF Icon">
                        </div>
                        <div class="detail">
                            <p>
                                ${item.title}
                            </p>
                        </div>
                    </div>
                `);
            });
        } else {
            console.warn("Response is empty or not an array");
        }
    }
});
    
/* Investors Contact */
InvestorsContact.html("");
$.ajax({
    url : "/Investors-Contact",
    method : "GET",
    success : function(response){
        console.log( response); // Shows the array
        if (Array.isArray(response) && response.length > 0) {
            response.forEach(function(item) {
                $('#Investors-Contact').append(`
                    <div class="investors-conatct-content">
                    <div class="card">
                        <div class="top">
                            <div class="image">
                                <img src="${item.profile}" alt="" srcset="">
                            </div>
                            <div class="name">
                                <h2>
                                  ${item.fullName}
                                </h2>
                                <p>
                                    ${item.role} </p>
                            </div>
                        </div>
                        <div class="address">
                            <p style="color: black;">${item.phone}</p>
                            <p style="color: black;">${item.mobile}</p>
                             <p style="color: black;">${item.email}</p>
                            <p style="color: black;">${item.address} </p>
                           
                        </div>
                    </div>
                </div>
                `);
            });
        } else {
            console.warn("Response is empty or not an array");
        }
    }
});


/* Public Disclosure */



PublicDisclosure.html("");
$.ajax({
    url : "/Public-Disclosure",
    method : "GET",
    success : function(response){
        console.log( response); // Shows the array
        if (Array.isArray(response) && response.length > 0) {
            response.forEach(function(item) {
                $('#Public-Disclosure').append(`
                  <div class="reports view-pdf" data-src="${item.filePath || ''}">
                        <div class="img">
                            <img src="images/icon/pdf.png" alt="PDF Icon">
                        </div>
                        <div class="detail">
                            <p>
                                ${item.title}
                            </p>
                        </div>
                    </div>
                `);
            });
        } else {
            console.warn("Response is empty or not an array");
        }
    }
});





/* Forms for TDS on Dividend */



FormsTDS.html("");
$.ajax({
    url : "/Forms-TDS",
    method : "GET",
    success : function(response){
        console.log( response); // Shows the array
        if (Array.isArray(response) && response.length > 0) {
            response.forEach(function(item) {
                $('#Forms-TDS').append(`
                  <div class="reports view-pdf" data-src="${item.filePath || ''}">
                        <div class="img">
                            <img src="images/icon/pdf.png" alt="PDF Icon">
                        </div>
                        <div class="detail">
                            <p>
                                ${item.title}
                            </p>
                        </div>
                    </div>
                `);
            });
        } else {
            console.warn("Response is empty or not an array");
        }
    }
});
appointmentorder.html("");

$.ajax({
    url : "/appointment-order",
    method : "GET",
    success : function(response){
        console.log( response); // Shows the array
        if (Array.isArray(response) && response.length > 0) {
            response.forEach(function(item) {
                $('#appointment-order').append(`
                  <div class="reports view-pdf" data-src="${item.filePath || ''}">
                        <div class="img">
                            <img src="images/icon/pdf.png" alt="PDF Icon">
                        </div>
                        <div class="detail">
                            <p>
                                ${item.title}
                            </p>
                        </div>
                    </div>
                `);
            });
        } else {
            console.warn("Response is empty or not an array");
        }
    }
});
});

/*showing pdf*/

$(document).on('click', '.view-pdf', async function () {
    const pdfUrl = $(this).data('src');
    $('#pdfContainer').html('<p>Loading PDF...</p>');
    $('#pdfModal1').modal('show');

    try {
        const loadingTask = pdfjsLib.getDocument(pdfUrl);
        const pdf = await loadingTask.promise;
        $('#pdfContainer').html('');

        for (let pageNum = 1; pageNum <= pdf.numPages; pageNum++) {
            const page = await pdf.getPage(pageNum);
            const viewport = page.getViewport({ scale: 1.2 });
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('2d');
            canvas.height = viewport.height;
            canvas.width = viewport.width;
            canvas.style.marginBottom = '10px';
            $('#pdfContainer').append(canvas);

            await page.render({ canvasContext: context, viewport: viewport }).promise;
        }
    } catch (error) {
        console.error("Error loading PDF:", error);
        $('#pdfContainer').html('<p>Failed to load PDF.</p>');
    }
});

// Clear on close
$('#pdfModal1').on('hidden.bs.modal', function () {
    $('#pdfContainer').html('');
});