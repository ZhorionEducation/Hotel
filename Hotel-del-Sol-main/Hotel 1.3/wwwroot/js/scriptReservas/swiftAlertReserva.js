$(document).ready(function () {
    $('#reservaForm').submit(function (event) {
        event.preventDefault();
        var form = $(this);
        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        title: 'Reserva creada',
                        text: 'La reserva se ha creado exitosamente.',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = redirectUrl;
                        }
                    });
                } else {
                    // Manejar errores si es necesario
                }
            }
        });
    });
});