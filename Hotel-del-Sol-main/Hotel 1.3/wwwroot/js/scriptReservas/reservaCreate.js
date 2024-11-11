$(document).ready(function () {
    // Animaci�n de entrada para el formulario
    $('.container').hide().fadeIn(1000);
    // Efecto hover para los campos del formulario
    $('.form-control').hover(
        function () { $(this).addClass('shadow-sm'); },
        function () { $(this).removeClass('shadow-sm'); }
    );
    // Validaci�n de fechas
    $('#FechaInicio, #FechaFin').change(function () {
        var fechaInicio = new Date($('#FechaInicio').val());
        var fechaFin = new Date($('#FechaFin').val());
        if (fechaFin <= fechaInicio) {
            alert('La fecha de fin debe ser posterior a la fecha de inicio');
            $('#FechaFin').val('');
        }
    });
    // Calculo automatico de dias y precio total
    $('#FechaInicio, #FechaFin, #PrecioTotal').change(function () {
        var fechaInicio = new Date($('#FechaInicio').val());
        var fechaFin = new Date($('#FechaFin').val());
        var precioTotal = parseFloat($('#PrecioTotal').val());
        if (!isNaN(fechaInicio) && !isNaN(fechaFin) && !isNaN(precioTotal)) {
            var dias = (fechaFin - fechaInicio) / (1000 * 60 * 60 * 24);
            var precioPorDia = precioTotal / dias;
            if (dias > 0) {
                alert('Estancia de ' + dias + ' dias. Precio por dia: $' + precioPorDia.toFixed(2));
            }
        }
    });
});


document.addEventListener('DOMContentLoaded', function() {
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    const selectedServicios = document.getElementById('selectedServicios');
    const selectedComodidades = document.getElementById('selectedComodidades');

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function() {
            const container = this.name === 'Servicios' ? selectedServicios : selectedComodidades;
            if (this.checked) {
                const span = document.createElement('span');
                span.classList.add('selected-item');
                span.textContent = this.nextElementSibling.textContent;
                span.dataset.id = this.id;
                container.appendChild(span);
            } else {
                const span = container.querySelector(`[data-id="${this.id}"]`);
                if (span) span.remove();
            }
        });
    });
});

//Precio de reserva:

$(document).ready(function() {
    const IVA = 0.19; // Porcentaje de IVA (19%)

    function actualizarPrecioTotal() {
        var precioTotal = 0;

        // Obtener el precio de la habitación seleccionada
        var habitacionId = $('#HabitacionId').val();
        if (habitacionId) {
            $.ajax({
                url: '/Reservas/GetPrecioHabitacion',
                type: 'GET',
                data: { id: habitacionId },
                success: function(precioHabitacion) {
                    precioTotal += parseFloat(precioHabitacion);

                    // Sumar los precios de los servicios seleccionados
                    var serviciosSeleccionados = $('input[name="Servicios"]:checked');
                    var totalServicios = serviciosSeleccionados.length;
                    var serviciosProcesados = 0;

                    serviciosSeleccionados.each(function() {
                        var servicioId = $(this).val();
                        $.ajax({
                            url: '/Reservas/GetPrecioServicio',
                            type: 'GET',
                            data: { id: servicioId },
                            success: function(precioServicio) {
                                precioTotal += parseFloat(precioServicio);
                                serviciosProcesados++;

                                // Actualizar el campo de precio total solo cuando todos los servicios hayan sido procesados
                                if (serviciosProcesados === totalServicios) {
                                    // Calcular el precio de las comodidades
                                    actualizarPrecioComodidades(precioTotal);
                                }
                            }
                        });
                    });

                    // Si no hay servicios seleccionados, actualizar el campo de precio total
                    if (totalServicios === 0) {
                        // Calcular el precio de las comodidades
                        actualizarPrecioComodidades(precioTotal);
                    }
                }
            });
        } else {
            // Si no hay habitación seleccionada, actualizar el campo de precio total a 0
            $('#precioTotalSinIVA').val(precioTotal.toFixed(2));
            $('#precioTotalConIVA').val(precioTotal.toFixed(2));
        }
    }

    function actualizarPrecioComodidades(precioTotal) {
        var comodidadesSeleccionadas = $('input[name="Comodidades"]:checked');
        var totalComodidades = comodidadesSeleccionadas.length;
        var comodidadesProcesadas = 0;

        comodidadesSeleccionadas.each(function() {
            var comodidadId = $(this).val();
            $.ajax({
                url: '/Reservas/GetPrecioComodidad',
                type: 'GET',
                data: { id: comodidadId },
                success: function(precioComodidad) {
                    precioTotal += parseFloat(precioComodidad);
                    comodidadesProcesadas++;

                    // Actualizar los campos de precio total solo cuando todas las comodidades hayan sido procesadas
                    if (comodidadesProcesadas === totalComodidades) {
                        // Calcular el IVA
                        var precioConIVA = precioTotal + (precioTotal * IVA);
                        $('#precioTotalSinIVA').val(precioTotal.toFixed(2));
                        $('#precioTotalConIVA').val(precioConIVA.toFixed(2));
                    }
                }
            });
        });

        // Si no hay comodidades seleccionadas, actualizar los campos de precio total
        if (totalComodidades === 0) {
            // Calcular el IVA
            var precioConIVA = precioTotal + (precioTotal * IVA);
            $('#precioTotalSinIVA').val(precioTotal.toFixed(2));
            $('#precioTotalConIVA').val(precioConIVA.toFixed(2));
        }
    }

    // Actualizar el precio total cuando se cambia la habitación
    $('#HabitacionId').change(function() {
        actualizarPrecioTotal();
    });

    // Actualizar el precio total cuando se seleccionan/deseleccionan servicios
    $('input[name="Servicios"]').change(function() {
        actualizarPrecioTotal();
    });

    // Actualizar el precio total cuando se seleccionan/deseleccionan comodidades
    $('input[name="Comodidades"]').change(function() {
        actualizarPrecioTotal();
    });

    // Inicializar el precio total
    actualizarPrecioTotal();
});





