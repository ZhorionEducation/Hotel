﻿document.getElementById('searchInput').addEventListener('input', function (e) {
    const searchTerm = e.target.value.toLowerCase();
    const rows = document.querySelectorAll('tbody tr');

    rows.forEach(row => {
        const cells = row.querySelectorAll('td');
        let found = false;

        cells.forEach(cell => {
            if (cell.textContent.toLowerCase().includes(searchTerm)) {
                found = true;
            }
        });

        row.style.display = found ? '' : 'none';
    });
});


function loadEditForm(id) {

    $.ajax({
        url: '/Reservas/Edit/' + id,
        type: 'GET',
        success: function (result) {
            $('#editFormContainer').html(result);
            $('#editModal').show();
            $('#detailsModal').hide(); // Ocultar el contenedor de detalles
        }
    });
}

function closeEditModal() {
    $('#editModal').hide();
    $('#detailsModal').show(); // Mostrar el contenedor de detalles
}

$(document).ready(function () {
    $('.close').click(function () {
        closeEditModal();
    });

    $(document).on('submit', '#editForm', function (e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    closeEditModal();
                    // Update the details view with the new data
                    loadDetailsModal(result.id);
                    // Optionally, update the main page
                    location.reload();
                } else {
                    $('#editFormContainer').html(result);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                alert('A ocurrido un error guardando los cambios. Intenta de nuevo');
            }
        });
    });
});

function loadDetailsModal(id) {
    $.ajax({
        url: '/Reservas/Details/' + id,
        type: 'GET',
        success: function (result) {
            $('#detailsContainer').html(result);
            $('#detailsModal').show();
        }
    });
}

$(document).ready(function () {
    $('.close').click(function () {
        closeDetailsModal();
    });
});
function closeDetailsModal() {
    $('#detailsModal').hide();

}