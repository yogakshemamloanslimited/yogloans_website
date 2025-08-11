$(document).ready(function() {
    $('#spinners').show();
    navigator.geolocation.getCurrentPosition(function(position) {
        const userLat = position.coords.latitude;
        const userLng = position.coords.longitude;

        $.ajax({
            url: "http://pfl.yogloans.com:8580/api/braches",
            method: "GET",
            success: function(response) {
                 $('#dummylist-list').hide();
                $('#branch-list').show();
                console.log("API response:", response); // Debug: log the response
                let branches = [];
                if (Array.isArray(response)) {
                    branches = response;
                } else if (Array.isArray(response.data)) {
                    branches = response.data;
                } else if (Array.isArray(response.Data)) {
                    branches = response.Data;
                } else {
                    console.warn("Unexpected response format");
                }
                let foundNearby = false;
                $('#branch-list').empty(); // Clear previous results
                $.each(branches, function(index, branch) {
                    const branchLat = parseFloat(branch.latitude); 
                    const branchLng = parseFloat(branch.longitude);

                    const distance = getDistanceInKm(userLat, userLng, branchLat, branchLng);

                    if (distance <= 10) {
                        foundNearby = true;
                        const mapsUrl = `https://www.google.com/maps/dir/?api=1&origin=${userLat},${userLng}&destination=${branchLat},${branchLng}`;
                        $('#branch-list').append(
                          `<div style='margin-bottom:10px;' class="card">
                          <div style='margin-bottom:10px; display:flex; width:100%;  justify-content: space-between;
align-items:center;'>
                          <img src="/images/icon/location.png">
                                <img src="/images/icon/show.png" data-bs-toggle="modal" data-bs-target="#model_${branch.branch_id}">
                                </div>
                             <span style='font-weight:bold; '>${branch.branch_name}</span> <span style="color:green;">(${distance.toFixed(2)} km away)</span>
                             <a href="${mapsUrl}" target="_blank" style='margin-left:10px; '>Get Directions</a>
                           </div>
                           

     <div class="modal fade" id="model_${branch.branch_id}" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">${branch.branch_name} - Details</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div>
                        <strong>Address:</strong><br>
                        ${branch.address1 ? branch.address1 + '<br>' : ''}
                        ${branch.address2 ? branch.address2 + '<br>' : ''}
                        ${branch.address3 ? branch.address3 + '<br>' : ''}
                        ${branch.address4 ? branch.address4 + '<br>' : ''}
                        ${branch.address5 ? branch.address5 + '<br>' : ''}
                    </div>
                    <div style="margin: 10px 0;">
                        <strong>Contact:</strong><br>
                        ${branch.mobile ? branch.mobile : 'N/A'}
                    </div>
                    <div style="margin: 10px 0;">
                        <strong>Map:</strong><br>
                        <iframe
                            width="100%"
                            height="200"
                            frameborder="0"
                            style="border:0"
                            src="https://www.google.com/maps?q=${branch.latitude},${branch.longitude}&hl=es;z=14&output=embed"
                            allowfullscreen>
                        </iframe>
                    </div>
                    <a href="https://www.google.com/maps/dir/?api=1&destination=${branch.latitude},${branch.longitude}" target="_blank" class="btn btn-primary w-100">
                        Get Directions
                    </a>
                </div>
            </div>
        </div>
    </div>
                           
                           
                           `
                        );
                    }
                });
                if (!foundNearby) {
                    $('#branch-list').empty();
                    $('#branch-list').html(`
                        <div style="grid-column: 1 / -1; text-align: justify; padding: 40px; background: white; border-radius: 15px; box-shadow: 0 5px 20px rgba(0, 0, 0, 0.08);">
                            <i class="fas fa-search" style="font-size: 3rem; color: #6c757d; margin-bottom: 20px;"></i>
                            <h3 style="color: #2c3e50; margin-bottom: 10px;">No Branches Nearby</h3>
                            <p style="color: #6c757d;">No branches found within 10km of your location. Please use the branch locator below to find branches in your area.</p>
                        </div>
                    `);
                }

                console.log("Branches loaded:", branches);
                setTimeout(function() {
                    let defaultBranch = branches.find(branch => String(branch.branch_id) === "0");
                    if (defaultBranch) {
                        $('#branch').val(defaultBranch.branch_id).trigger('change');
                    }
                }, 0);
            },
            error: function(xhr, status, error) {
                $('#spinners').hide();
                $('#branch-list').empty();
                console.error("AJAX error:", error);
            }
        });

    }, function(error) {
        $('#spinners').hide();
        $('#branch-list').empty();
        // SweetAlert for location denied or error
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'warning',
                title: 'Location Required',
                text: 'Please allow location access to find nearby branches.',
                confirmButtonText: 'OK'
            });
        } else {
            alert('Please allow location access to find nearby branches.');
        }
        console.error("Geolocation error:", error);
    });

    // Haversine formula to calculate distance between two lat/lng points
    function getDistanceInKm(lat1, lon1, lat2, lon2) {
        const R = 6371; // Radius of the earth in km
        const dLat = (lat2 - lat1) * Math.PI / 180;
        const dLon = (lon2 - lon1) * Math.PI / 180;
        const a =
            Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) *
            Math.sin(dLon / 2) * Math.sin(dLon / 2);
        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        return R * c;
    }

   $.ajax({
       url: "http://pfl.yogloans.com:8580/api/states",
            method: "GET",
            success: function(response) {
                // Try to get the array of states from the response
                let states = [];
                if (Array.isArray(response)) {
                    states = response;
                } else if (Array.isArray(response.data)) {
                    states = response.data;
                } else if (Array.isArray(response.Data)) {
                    states = response.Data;
                } else {
                    console.warn("Unexpected response format", response);
                }

                $('#states').empty(); // Use the correct selector

                function capitalizeWords(str) {
                    return str.replace(/\b\w/g, c => c.toUpperCase());
                }

                $.each(states, function(index, state) {
                    let displayName = capitalizeWords(String(state.state_name).trim());
                    console.log("Display name:", displayName);
                    $('#states').append(
                        `<option value="${state.state_id}">${displayName}</option>`
                    );
                });
   // When a state is selected, fetch branches for that state
                $('#states').on('change', function() {
                    var stateId = $(this).val();
                    if (!stateId) {
                        $('#branch').empty();
                        return;
                    }
                    $.ajax({
                        url: "http://pfl.yogloans.com:8580/api/selected?state_id=" + stateId,
                        method: "GET",
                        success: function(response) {
                            let branches = [];
                            if (Array.isArray(response)) {
                                branches = response;
                            } else if (Array.isArray(response.data)) {
                                branches = response.data;
                            } else if (Array.isArray(response.Data)) {
                                branches = response.Data;
                            } else {
                                console.warn("Unexpected response format", response);
                            }
                            currentBranches = branches;
                            $('#branch').empty();
                            $.each(branches, function(index, branch) {
                                $('#branch').append(
                                    `<option value="${branch.branch_id}">${branch.branch_name}</option>`
                                );
                            });

                            // Select and show default branch (branch_id == 0)
                            let defaultBranch = branches.find(branch => String(branch.branch_id) === "0");
                            if (defaultBranch) {
                                $('#branch').val(defaultBranch.branch_id).trigger('change');
                            }
                        },
                        error: function(xhr, status, error) {
             
                            console.error("AJAX error:", error);
                            $('#branch').empty();
                        }
                    });
                });
            },
            error: function(xhr, status, error) {
                console.error("AJAX error:", error);
                // Optionally show a message to the user
            }
        });
    });

    let map; // For the Leaflet map
    let branchMarker; // For the marker
    let currentBranches = []; // This will store the branches for the selected state
    let selectedBranchForMap = null; // Store the selected branch

    // Initialize default map
    function initializeDefaultMap() {
        // Default coordinates from the Google Maps URL (Thrissur area)
        const defaultLat = 10.53532;
        const defaultLng = 76.21403;
        
        // Remove previous map instance if it exists
        if (map) {
            map.remove();
            map = null;
        }
        
        $('#branch-map').html(""); // Clear the map container
        
        // Initialize the map with default location
        map = L.map('branch-map').setView([defaultLat, defaultLng], 15);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
        
        // Add default marker
        branchMarker = L.marker([defaultLat, defaultLng])
            .addTo(map)
            .bindPopup('Yogakshemam Apts.<br>Kallingal Lane<br>Thrissur - 01')
            .openPopup();
    }

    // Initialize default map when page loads
    $(document).ready(function() {
        // Initialize default map after a short delay to ensure DOM is ready
        setTimeout(function() {
            initializeDefaultMap();
        }, 500);
    });

    $('#branch').on('change', function() {
        console.log("Branch changed!", $(this).val());
        var branchId = $(this).val();
        
        // Show loader immediately when branch selection starts
        $('#branch-loader').show();
        $('#branch-info').removeClass('active');
        
        // Add a small delay to make loader visible
        setTimeout(function() {
            if (!branchId) {
                $('#open-maps-btn').hide();
                $('#branch-loader').hide();
                selectedBranchForMap = null;
                
                // Show default data when no branch is selected
                $('#default-address').show();
                $('#default-contact').show();
                $('#dynamic-address').hide();
                $('#dynamic-contact').hide();
                
                // Show default map
                initializeDefaultMap();
                return;
            }

            var selectedBranch = currentBranches.find(branch => String(branch.branch_id) === String(branchId));
            
            // If HO branch is selected (branch_id == "0") or no valid branch data, show default map
            if (branchId === "0" || !selectedBranch || !selectedBranch.latitude || !selectedBranch.longitude) {
                $('#open-maps-btn').hide();
                $('#branch-loader').hide();
                selectedBranchForMap = null;
                
                // Show default data when no valid branch data
                $('#default-address').show();
                $('#default-contact').show();
                $('#dynamic-address').hide();
                $('#dynamic-contact').hide();
                
                // Show default map
                initializeDefaultMap();
                return;
            }

            // Hide loader and show branch info section
            $('#branch-loader').hide();
            $('#branch-info').addClass('active');

            // Hide default data and show dynamic data
            $('#default-address').hide();
            $('#default-contact').hide();
            $('#dynamic-address').show();
            $('#dynamic-contact').show();

            // Show address
            let addressHtml = '';
            if (selectedBranch.address1) addressHtml += selectedBranch.address1 + '<br>';
            if (selectedBranch.address2) addressHtml += selectedBranch.address2 + '<br>';
            if (selectedBranch.address3) addressHtml += selectedBranch.address3 + '<br>';
            if (selectedBranch.address4) addressHtml += selectedBranch.address4 + '<br>';
            if (selectedBranch.address5) addressHtml += selectedBranch.address5 + '<br>';
            $('#dynamic-address').html(addressHtml || '<span style="color: #6c757d;">Address not available</span>');
            
            let contactHtml = '';
            if (selectedBranch.mobile) contactHtml += '<span style="color: #495057;">Mobile: ' + selectedBranch.mobile + '</span>';
            $('#dynamic-contact').html(contactHtml || '<span style="color: #6c757d;">Contact not available</span>');
            
            selectedBranchForMap = selectedBranch;
            $('#open-maps-btn').show();

            // Show the map
            // Remove previous map instance if it exists
            if (map) {
                map.remove();
                map = null;
            }
            $('#branch-map').html(""); // Clear the map container

            map = L.map('branch-map').setView([selectedBranch.latitude, selectedBranch.longitude], 15);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '© OpenStreetMap contributors'
            }).addTo(map);

            // Add marker for the branch
            branchMarker = L.marker([selectedBranch.latitude, selectedBranch.longitude])
                .addTo(map)
                .bindPopup(selectedBranch.branch_name)
                .openPopup();
        }, 300); // 300ms delay to show loader
    });

    // Button click handler
    $('#open-maps-btn').on('click', function() {
        if (selectedBranchForMap) {
            // Use selected branch coordinates
            var mapsUrl = `https://www.google.com/maps/search/?api=1&query=${selectedBranchForMap.latitude},${selectedBranchForMap.longitude}`;
            window.open(mapsUrl, '_blank');
        } else {
            // Use default coordinates for HO branch
            var defaultLat = 10.53532;
            var defaultLng = 76.21403;
            var mapsUrl = `https://www.google.com/maps/search/?api=1&query=${defaultLat},${defaultLng}`;
            window.open(mapsUrl, '_blank');
        }
    });

    // 1. After populating the states dropdown
    let defaultStateId = "12"; // Replace with your actual default state_id
    $('#states').val(defaultStateId).trigger('change');

    // 2. Show default data initially
    $('#branch-info').addClass('active');
    $('#default-address').show();
    $('#default-contact').show();
    $('#dynamic-address').hide();
    $('#dynamic-contact').hide();
    
    // Show Get Directions button for default location
    $('#open-maps-btn').show();

    // 3. In the branches AJAX success
    setTimeout(function() {
        let defaultBranch = branches.find(branch => String(branch.branch_id) === "0");
        if (defaultBranch) {
            $('#branch').val(defaultBranch.branch_id).trigger('change');
        }
    }, 0);

    let firstStateId = $('#states option:first').val();
    $('#states').val(firstStateId).trigger('change');