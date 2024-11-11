$(document).ready(function () {
    // Eliminar la extensión global de mensajes
    // $.extend($.validator.messages, {
    //     required: "Este campo es obligatorio.",
    //     pattern: "El formato es inválido."
    // });

    var numeroAcompanantesInput = $('#numeroAcompanantes');

    numeroAcompanantesInput.on('input', function () {
        var numAcompanantes = $(this).val();
        var huespedesContainer = $('#huespedesContainer');

        huespedesContainer.empty();

        if (numAcompanantes > 0 && numAcompanantes <= 5) {
            for (var i = 0; i < numAcompanantes; i++) {
                var nombreCampo = `Nombre del Huésped ${i + 1}`;
                var documentoCampo = `Documento de Identidad del Huésped ${i + 1}`;

                var huespedForm = `
                    <div class="form-group">
                        <label for="Huespedes_${i}__Nombre" class="control-label">${nombreCampo}</label>
                        <input type="text" 
                               id="Huespedes_${i}__Nombre"
                               name="Huespedes[${i}].Nombre" 
                               class="form-control" 
                               data-val="true" 
                               data-val-required="El nombre del huésped ${i + 1} es obligatorio." 
                               data-val-regex="El nombre del huésped ${i + 1} debe contener solo letras y espacios (máximo 20 caracteres)."
                               data-val-regex-pattern="^[a-zA-Z\\s]{1,20}$">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Huespedes[${i}].Nombre" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group">
                        <label for="Huespedes_${i}__DocumentoIdentidad" class="control-label">${documentoCampo}</label>
                        <input type="text" 
                               id="Huespedes_${i}__DocumentoIdentidad"
                               name="Huespedes[${i}].DocumentoIdentidad" 
                               class="form-control" 
                               data-val="true" 
                               data-val-required="El documento de identidad del huésped ${i + 1} es obligatorio." 
                               data-val-regex="El documento de identidad del huésped ${i + 1} debe contener exactamente 11 dígitos numéricos."
                               data-val-regex-pattern="^\\d{11}$">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Huespedes[${i}].DocumentoIdentidad" data-valmsg-replace="true"></span>
                    </div>
                `;
                huespedesContainer.append(huespedForm);
            }

            // Destruir y reiniciar la validación
            var form = huespedesContainer.closest('form');
            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);
        }
    });
});