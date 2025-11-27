function date_validation() {
    const dateInputs = document.querySelectorAll('input[type="date"]');

    dateInputs.forEach(input => {
        input.addEventListener('change', function () {
            const selectedDate = this.value;

            if (!selectedDate) {
                alert("Please select a valid date.");
                return;
            }

            const year = selectedDate.split('-')[0];

            if (year.length !== 4) {
                alert("Year must have exactly 4 digits.");
                this.value = ""; // Clear the field
            }
        });
    });
}

// Call it on page load
document.addEventListener("DOMContentLoaded", date_validation);