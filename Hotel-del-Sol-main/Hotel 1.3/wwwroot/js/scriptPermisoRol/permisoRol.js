// permisoRol.js

function openModal(modalId, itemId) {
    var modal = document.getElementById(modalId);
    if (!modal) return;

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
        
        // Cargar datos del rol y sus permisos
        fetch(`/PermisoRol/GetRolDetails/${itemId}`)
            .then(response => response.json())
            .then(data => {
                document.getElementById('editRolNombre').value = data.nombre;
                
                // Desmarcar todos los checkboxes primero
                document.querySelectorAll('input[name="PermisosIds"]').forEach(checkbox => {
                    checkbox.checked = false;
                });

                // Marcar los checkboxes de los permisos existentes
                data.permisos.forEach(permisoId => {
                    const checkbox = document.querySelector(`input[name="PermisosIds"][value="${permisoId}"]`);
                    if (checkbox) {
                        checkbox.checked = true;
                    }
                });
            });
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
    fetch(`/PermisoRol/ChangeEstadoRol/${id}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ estado: !estadoActual })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const checkbox = document.querySelector(`input[onclick*="changeEstadoRol('${id}')"]`);
            if (checkbox) {
                checkbox.checked = !estadoActual;
                Swal.fire({
                    icon: 'success',
                    title: '¡Estado actualizado!',
                    text: 'El estado del rol ha sido modificado correctamente',
                    timer: 1500,
                    showConfirmButton: false
                });
            }
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'No se pudo cambiar el estado del rol'
            });
            const checkbox = document.querySelector(`input[onclick*="changeEstadoRol('${id}')"]`);
            if (checkbox) {
                checkbox.checked = estadoActual;
            }
        }
    })
    .catch(error => {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Hubo un error al procesar la solicitud'
        });
        const checkbox = document.querySelector(`input[onclick*="changeEstadoRol('${id}')"]`);
        if (checkbox) {
            checkbox.checked = estadoActual;
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
                Swal.fire({
                    icon: 'success',
                    title: '¡Éxito!',
                    text: 'El permiso ha sido creado correctamente',
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => {
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo crear el permiso: ' + data.message
                });
            }
        })
        .catch(error => {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Hubo un error al procesar la solicitud'
            });
        });
}
// Función para manejar la creación de roles
document.addEventListener('DOMContentLoaded', function() {
    const addRolForm = document.querySelector('#addRolForm');
    if (addRolForm) {
        addRolForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            // Validación del nombre
            const nombreInput = document.getElementById('addRolNombreInput');
            if (!nombreInput.value.trim()) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'El nombre del rol es requerido'
                });
                return;
            }

            // Validación de permisos
            const permisosSeleccionados = Array.from(
                document.querySelectorAll('#addRolModal input[name="PermisosIds"]:checked')
            );
            
            if (permisosSeleccionados.length === 0) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Debe seleccionar al menos un permiso'
                });
                return;
            }

            try {
                const formData = new FormData();
                formData.append('Nombre', nombreInput.value.trim());
                formData.append('PermisosIds', JSON.stringify(permisosSeleccionados.map(cb => cb.value)));

                const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                if (tokenElement) {
                    formData.append('__RequestVerificationToken', tokenElement.value);
                }

                const response = await fetch('/PermisoRol/CreateRol', {
                    method: 'POST',
                    body: formData
                });

                if (!response.ok) {
                    throw new Error('Error en la respuesta del servidor');
                }

                const data = await response.json();
                
                if (data.success) {
                    closeModal('addRolModal');
                    await Swal.fire({
                        icon: 'success',
                        title: '¡Éxito!',
                        text: data.message,
                        timer: 2000,
                        showConfirmButton: false
                    });
                    location.reload();
                } else {
                    throw new Error(data.message || 'Error al crear el rol');
                }
            } catch (error) {
                console.error('Error:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.message || 'Error al crear el rol'
                });
            }
        });
    }
});
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

// Función para manejar el envío del formulario de edición
document.addEventListener('DOMContentLoaded', function() {
    const editRolForm = document.querySelector('#editRolModal form');
    if (editRolForm) {
        editRolForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            try {
                const formData = new FormData();
                formData.append('Id', document.getElementById('editRolId').value);
                formData.append('Nombre', document.getElementById('editRolNombre').value);
                
                const permisosSeleccionados = Array.from(
                    document.querySelectorAll('input[name="PermisosIds"]:checked')
                ).map(cb => cb.value);
                
                formData.append('PermisosIds', JSON.stringify(permisosSeleccionados));

                // Si usas tokens antiforgery:
                const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                if (tokenElement) {
                    formData.append('__RequestVerificationToken', tokenElement.value);
                }

                const response = await fetch('/PermisoRol/EditRol', {
                    method: 'POST',
                    body: formData
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
                }

                const data = await response.json();
                
                if (data.success) {
                    closeModal('editRolModal');
                    await Swal.fire({
                        icon: 'success',
                        title: '¡Éxito!',
                        text: data.message,
                        timer: 2000,
                        showConfirmButton: false
                    });
                    location.reload();
                } else {
                    throw new Error(data.message || 'Error al actualizar el rol');
                }
            } catch (error) {
                console.error('Error:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.message || 'Error al actualizar el rol'
                });
            }
        });
    }
});