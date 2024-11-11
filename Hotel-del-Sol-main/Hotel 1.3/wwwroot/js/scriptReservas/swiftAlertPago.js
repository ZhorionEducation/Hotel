$(document).ready(function () {
    $('#formpagos').submit(function (event) {
        event.preventDefault(); // Prevenir el envío por defecto del formulario

        var form = $(this);
        var formData = new FormData(form[0]);

        console.log('Enviando formulario...');

        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                console.log('Respuesta AJAX:', response); // Debug

                if (response.success) {
                    Swal.fire({
                        title: 'Comprobante enviado',
                        text: 'Un administrador revisará tu comprobante y te notificará si la reserva fue aceptada.',
                        icon: 'info',
                        confirmButtonText: 'Entendido',
                        allowOutsideClick: false,
                        allowEscapeKey: false
                    }).then((result) => {
                        if (result.isConfirmed) {
                            if (response.redirectUrl) {
                                console.log('Redirigiendo a:', response.redirectUrl); // Debug
                                window.location.href = response.redirectUrl;
                            } else {
                                Swal.fire('Error', "URL de redirección inválida.", 'error');
                            }
                        }
                    });
                } else {
                    Swal.fire('Error', response.message || "Error al procesar el pago.", 'error');
                }
            },
            error: function (xhr, status, error) {
                console.error('Error AJAX:', status, error, xhr.responseText); // Debug
                Swal.fire('Error', "Error al procesar la solicitud.", 'error');
            }
        });
    });
});