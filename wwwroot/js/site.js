document.addEventListener('DOMContentLoaded', function () {
    const bookingModalEl = document.getElementById('bookingModal');
    if (!bookingModalEl) return;

    const bookingModal = new bootstrap.Modal(bookingModalEl);
    
    // Step Elements
    const stepDateTime = document.getElementById('step-datetime');
    const stepDetails = document.getElementById('step-details');
    const stepSuccess = document.getElementById('step-success');
    
    // Form Elements
    const bookingDateInput = document.getElementById('bookingDate');
    const timeSlotsContainer = document.getElementById('timeSlotsContainer');
    const timeSlotsGrid = document.getElementById('timeSlotsGrid');
    const step1Actions = document.getElementById('step1Actions');
    const serviceSelect = document.getElementById('serviceSelect');
    
    // Buttons
    const btnNextToDetails = document.getElementById('btnNextToDetails');
    const btnBackToDateTime = document.getElementById('btnBackToDateTime');
    const bookingForm = document.getElementById('bookingForm');

    // State
    let selectedDateVal = null;
    let selectedTimeVal = null;

    // Load Services on Modal Open (Or on first load)
    let servicesLoaded = false;
    bookingModalEl.addEventListener('show.bs.modal', function () {
        // Reset states
        resetModal();
        if (!servicesLoaded) {
            fetch('/Booking/GetServices')
                .then(r => r.json())
                .then(data => {
                    data.forEach(service => {
                        const opt = document.createElement('option');
                        opt.value = service.id;
                        opt.textContent = `${service.name} - $${service.price.toFixed(2)}`;
                        serviceSelect.appendChild(opt);
                    });
                    servicesLoaded = true;
                });
        }
    });

    function resetModal() {
        stepDateTime.classList.remove('d-none');
        stepDetails.classList.add('d-none');
        stepSuccess.classList.add('d-none');
        bookingDateInput.value = '';
        timeSlotsContainer.classList.add('d-none');
        step1Actions.classList.add('d-none');
        selectedDateVal = null;
        selectedTimeVal = null;
        bookingForm.reset();
    }

    // Step 1: Date Selected
    bookingDateInput.addEventListener('change', function () {
        selectedDateVal = this.value;
        if (!selectedDateVal) {
            timeSlotsContainer.classList.add('d-none');
            step1Actions.classList.add('d-none');
            return;
        }

        // Fetch slots
        fetch(`/Booking/GetAvailableSlots?date=${selectedDateVal}`)
            .then(r => r.json())
            .then(slots => {
                timeSlotsGrid.innerHTML = '';
                if (slots.length === 0) {
                    timeSlotsGrid.innerHTML = '<div class="col-12"><div class="alert alert-warning">No slots available for this date. Please select another date.</div></div>';
                    step1Actions.classList.add('d-none');
                } else {
                    slots.forEach(slot => {
                        const div = document.createElement('div');
                        div.className = 'col-4 col-sm-3';
                        div.innerHTML = `<div class="p-2 border rounded-3 text-center time-slot" data-time="${slot}">${slot}</div>`;
                        timeSlotsGrid.appendChild(div);
                    });
                    timeSlotsContainer.classList.remove('d-none');
                    step1Actions.classList.add('d-none'); // Hide next until slot selected
                    selectedTimeVal = null;
                    
                    // Add click listeners to slots
                    document.querySelectorAll('.time-slot').forEach(el => {
                        el.addEventListener('click', function () {
                            document.querySelectorAll('.time-slot').forEach(t => t.classList.remove('selected'));
                            this.classList.add('selected');
                            selectedTimeVal = this.dataset.time;
                            step1Actions.classList.remove('d-none');
                        });
                    });
                }
            });
    });

    // Step 1 -> Step 2
    btnNextToDetails.addEventListener('click', function () {
        if (!selectedTimeVal) return;
        document.getElementById('selectedDate').value = selectedDateVal;
        document.getElementById('selectedTime').value = selectedTimeVal;
        
        stepDateTime.classList.add('d-none');
        stepDetails.classList.remove('d-none');
    });

    // Step 2 -> Step 1
    btnBackToDateTime.addEventListener('click', function () {
        stepDetails.classList.add('d-none');
        stepDateTime.classList.remove('d-none');
    });

    // Step 2: Form Submit
    bookingForm.addEventListener('submit', function (e) {
        e.preventDefault();
        
        const payload = {
            CustomerName: document.getElementById('customerName').value,
            Email: document.getElementById('customerEmail').value,
            PhoneNumber: document.getElementById('customerPhone').value,
            CarPlateNumber: document.getElementById('carPlate').value,
            ServiceId: parseInt(document.getElementById('serviceSelect').value),
            BookingDate: document.getElementById('selectedDate').value,
            TimeSlot: document.getElementById('selectedTime').value + ":00", // TimeSpan format "hh:mm:ss"
            CompanyName: document.getElementById('companyName').value,
            Notes: document.getElementById('bookingNotes').value
        };

        const btnSubmit = document.getElementById('btnSubmitBooking');
        btnSubmit.disabled = true;
        btnSubmit.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';

        fetch('/Booking/CreateBooking', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
        .then(response => {
            if (response.ok) {
                stepDetails.classList.add('d-none');
                stepSuccess.classList.remove('d-none');
            } else {
                response.text().then(text => alert(text || 'An error occurred. Please try again.'));
            }
        })
        .catch(error => {
            console.error(error);
            alert('An error occurred during booking.');
        })
        .finally(() => {
            btnSubmit.disabled = false;
            btnSubmit.textContent = 'Confirm Booking';
        });
    });
});
