// permisoRol.js

function openModal(modalId, itemId) {
    var modal = document.getElementById(modalId);
    if (!modal) {
        console.error(`Modal con ID: ${modalId} no encontrado.`);
        return;
    }

    modal.style.display = "block";
    setTimeout(() => {
        modal.style.opacity = "1";
        modal.style.visibility = "visible";
        modal.querySelector('.modal-content').style.transform = "translateY(0)";
    }, 10);

    if (modalId === 'editPermisoModal') {
        document.getElementById('editPermisoId').value = itemId;
    } else if (modalId === 'editRolModal') {
        document.getElementById('editRolId').value = itemId;
    } else if (modalId === 'deletePermisoModal') {
        document.getElementById('deletePermisoId').value = itemId;
    } else if (modalId === 'deleteRolModal') {
        document.getElementById('deleteRolId').value = itemId;
    } else if (modalId === 'detailsPermisoModal') {
        loadDetails(itemId, true);
    } else if (modalId === 'detailsRolModal') {
        loadDetails(itemId, false);
    }
}

function closeModal(modalId) {
    var modal = document.getElementById(modalId);
    var modalContent = modal.querySelector('.modal-content');

    modalContent.style.transform = "translateY(-20px)";
    modal.style.opacity = "0";

    setTimeout(() => {
        modal.style.display = "none";
        modal.style.visibility = "hidden";
        modalContent.style.transform = "translateY(0)";
    }, 200);
}

function loadDetails(id, isPermiso) {
    var url = '/PermisoRol/Details';
    url += "?id=" + id + "&isPermiso=" + isPermiso;

    fetch(url)
        .then(response => response.text())
        .then(data => {
            if (isPermiso) {
                document.getElementById('detailsPermisoContent').innerHTML = data;
            } else {
                document.getElementById('detailsRolContent').innerHTML = data;
            }
        })
        .catch(error => console.error('Error loading details:', error));
}

let currentRolId;
let currentRolEstado;

function changeEstadoRol(id, estadoActual) {
    console.log(`Cambiando estado del rol con ID: ${id}, estado actual: ${estadoActual}`);

    fetch(`/PermisoRol/ChangeEstadoRol/${id}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ estado: !estadoActual }) // Inviertes el estado aquí
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            console.log(`Estado cambiado exitosamente para el rol con ID: ${id}`);
            const checkbox = document.querySelector(`input[onclick*="changeEstadoRol('${id}')"]`);
            if (checkbox) {
                checkbox.checked = !estadoActual; // Cambia el estado en la UI
            } else {
                console.error(`Checkbox no encontrado para el rol con ID: ${id}`);
            }
        } else {
            console.error('Error al cambiar el estado:', data.message);
            // Revertir el cambio en la UI si hubo un error
            const checkbox = document.querySelector(`input[onclick*="changeEstadoRol('${id}')"]`);
            if (checkbox) {
                checkbox.checked = estadoActual;
            } else {
                console.error(`Checkbox no encontrado para el rol con ID: ${id}`);
            }
        }
    })
    .catch(error => {
        console.error('Error:', error);
        // Revertir el cambio en la UI si hubo un error
        const checkbox = document.querySelector(`input[onclick*="changeEstadoRol('${id}')"]`);
        if (checkbox) {
            checkbox.checked = estadoActual;
        } else {
            console.error(`Checkbox no encontrado para el rol con ID: ${id}`);
        }
    });
}

// Event listeners para cerrar modales
document.addEventListener('DOMContentLoaded', function () {
    var modals = document.getElementsByClassName('modal');
    for (var i = 0; i < modals.length; i++) {
        modals[i].addEventListener('click', function (event) {
            if (event.target === this) {
                closeModal(this.id);
            }
        });
    }

    // Animaci�n para los botones
    var buttons = document.getElementsByTagName('button');
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].addEventListener('mouseover', function () {
            this.style.transform = 'scale(1.05)';
            this.style.transition = 'transform 0.1s ease';
        });
        buttons[i].addEventListener('mouseout', function () {
            this.style.transform = 'scale(1)';
            this.style.transition = 'transform 0.1s ease';
        });
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var createPermisoForm = document.getElementById('createPermisoForm');
    if (createPermisoForm) {
        createPermisoForm.addEventListener('submit', createPermiso);
    }

    var createRolForm = document.getElementById('createRolForm');
    if (createRolForm) {
        createRolForm.addEventListener('submit', createRol);
    }
});

document.addEventListener('DOMContentLoaded', function () {


    // Evento para el bot�n de confirmaci�n en el modal de cambio de estado
    document.getElementById('confirmChangeEstado').addEventListener('click', confirmChangeEstadoRol);

    // Animaci�n para los botones de cambio de estado
    var changeEstadoButtons = document.querySelectorAll('.btn-warning, .btn-success');
    for (var i = 0; i < changeEstadoButtons.length; i++) {
        changeEstadoButtons[i].addEventListener('mouseover', function () {
            this.style.transform = 'scale(1.05)';
            this.style.transition = 'transform 0.1s ease';
        });
        changeEstadoButtons[i].addEventListener('mouseout', function () {
            this.style.transform = 'scale(1)';
            this.style.transition = 'transform 0.1s ease';
        });
    }
});

// Funci�n para manejar la creaci�n de permisos
function createPermiso(event) {
    event.preventDefault();
    var form = event.target;
    var formData = new FormData(form);

    fetch(form.action, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                closeModal('addPermisoModal');
                location.reload();
            } else {
                // Manejar errores
                console.error('Error al crear permiso:', data.message);
            }
        })
        .catch(error => console.error('Error:', error));
}

// Funci�n para manejar la creaci�n de roles
function createRol(event) {
    event.preventDefault();
    var form = event.target;
    var formData = new FormData(form);

    fetch(form.action, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                closeModal('addRolModal');
                location.reload();
            } else {
                // Manejar errores
                console.error('Error al crear rol:', data.message);
            }
        })
        .catch(error => console.error('Error:', error));
}

// Asignar los manejadores de eventos a los formularios cuando el DOM est� cargado
document.addEventListener('DOMContentLoaded', function () {
    var createPermisoForm = document.getElementById('createPermisoForm');
    if (createPermisoForm) {
        createPermisoForm.addEventListener('submit', createPermiso);
    }

    var createRolForm = document.getElementById('createRolForm');
    if (createRolForm) {
        createRolForm.addEventListener('submit', createRol);
    }
});