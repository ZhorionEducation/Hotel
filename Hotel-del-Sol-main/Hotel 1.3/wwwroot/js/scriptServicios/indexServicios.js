document.getElementById('searchInput').addEventListener('input', function (e) {
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
        url: '/ServiciosAdicionales/Edit/' + id,
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
        url: '/ServiciosAdicionales/Details/' + id,
        type: 'GET',
        success: function (result) {
            $('#detailsContainer').html(result);
            $('#detailsModal').show();
        }
    });
}

function loadDeleteForm(id) {
    $.ajax({
        url: '/ServiciosAdicionales/Delete/' + id,
        type: 'GET',
        success: function (result) {
            $('#deleteFormContainer').html(result);
            $('#deleteModal').show();
        }
    });
}

function closeDeleteModal() {
    $('#deleteModal').hide();
}

$(document).ready(function () {
    $('.close').click(function () {
        closeDeleteModal();
    });

    $(document).on('submit', '#deleteForm', function (e) {
        e.preventDefault();
        $.ajax({
            url: $(this).attr('action'),
            type: 'POST',
            data: $(this).serialize(),
            success: function (result) {
                if (result.success) {
                    closeDeleteModal();
                    removeServiceRow(result.id);
                } else {
                    $('#deleteFormContainer').html(result);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
                alert('A ocurrido un error eliminando el servicio. Intenta de nuevo');
            }
        });
    });
});

$(document).ready(function () {
    $('.close').click(function () {
        closeDetailsModal();
    });
});

function closeDetailsModal() {
    $('#detailsModal').hide();
}

function updateServiceRow(service) {
    const row = document.querySelector(`tr[data-id="${service.id}"]`);
    if (row) {
        row.querySelector('.service-name').textContent = service.nombre;
        row.querySelector('.service-description').textContent = service.descripcion;
        row.querySelector('.service-image').src = service.imagenUrl;
    }
}

function removeServiceRow(id) {
    const row = document.querySelector(`tr[data-id="${id}"]`);
    if (row) {
        row.remove();
    }
}