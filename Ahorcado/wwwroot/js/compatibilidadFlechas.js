function formatearPalabra(estado) {
    return estado.split('').join(' ');
}

function actualizarPantalla(data) {
    $('#palabra-oculta').text(formatearPalabra(data.estado));
    $('#img-ahorcado').attr('src', data.imagen);
    $('#txt-fallos').text(data.fallos);

    if (data.gano) {
        $('#mensaje').text('¡Ganaste! 😄');
        $('.btn-letra').prop('disabled', true);
    } else if (data.perdio) {
        $('#mensaje').text('Perdiste... 😢');
        $('.btn-letra').prop('disabled', true);
    } else {
        $('#mensaje').text('');
    }
}

function cargarNuevoJuego() {
    $.get('/Ahorcado/NuevoJuegoApi', function (data) {
        $('.btn-letra').prop('disabled', false);
        actualizarPantalla(data);
    });
}

function intentar(letra) {
    $.post('/Ahorcado/Intentar', { letra: letra }, function (data) {
        actualizarPantalla(data);
    });
}

$(document).ready(function () {
    const letras = 'ABCDEFGHIJKLMNÑOPQRSTUVWXYZ';
    let html = '';
    for (let c of letras) {
        html += `<button type="button"
                          class="btn btn-outline-secondary m-1 btn-letra"
                          data-letra="${c}"
                          aria-label="Letra ${c}">
                    ${c}
                 </button>`;
    }
    $('#teclado').html(html);

    $('#teclado').on('click', '.btn-letra', function () {
        const letra = $(this).data('letra');
        $(this).prop('disabled', true);
        intentar(letra);
    });


    $('#teclado').on('keydown', '.btn-letra', function (e) {
        const $botones = $('.btn-letra');
        const total = $botones.length;
        const index = $botones.index(this);
        if (index === -1) return;

        const columnas = 7; 
        const fila = Math.floor(index / columnas);
        const col = index % columnas;

        let nuevoIndex = index;
        let step = 0;

        if (e.key === 'ArrowRight') {
            nuevoIndex = index + 1;
            step = 1;
        } else if (e.key === 'ArrowLeft') {
            nuevoIndex = index - 1;
            step = -1;
        } else if (e.key === 'ArrowDown') {
     
            const nuevaFila = fila + 1;
            if (nuevaFila * columnas >= total) return; 
            nuevoIndex = nuevaFila * columnas + col;
            step = columnas;
        } else if (e.key === 'ArrowUp') {
            const nuevaFila = fila - 1;
            if (nuevaFila < 0) return;
            nuevoIndex = nuevaFila * columnas + col;
            step = -columnas;
        } else {
            return; 
        }


        if (nuevoIndex < 0 || nuevoIndex >= total) {
            return;
        }


        function buscarHabilitado(idx, paso) {
            let i = idx;
            while (i >= 0 && i < total) {
                const $btn = $botones.eq(i);
                if (!$btn.prop('disabled')) {
                    return i;
                }
                i += paso;
            }
            return index;
        }

        nuevoIndex = buscarHabilitado(nuevoIndex, step);

        if (nuevoIndex !== index) {
            e.preventDefault();
            $botones.eq(nuevoIndex).focus();
        }
    });


    $('#btn-nuevo').click(function () {
        cargarNuevoJuego();
    });

    $('#btn-accesibilidad').on('click', function () {
        const abierto = $(this).attr('aria-expanded') === 'true';
        $(this).attr('aria-expanded', (!abierto).toString());
        $('#panel-accesibilidad').toggle(!abierto);

        if (!abierto) {
            $('#chk-alto-contraste').focus();
        }
    });

    $('#chk-alto-contraste').on('change', function () {
        $('body').toggleClass('modo-alto-contraste', this.checked);
    });

    $('#chk-alto-contraste').on('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            $(this).click();
        }
    });

    const selectorFocoAccesibilidad = '#chk-alto-contraste, #btn-font-minus, #btn-font-plus';

    $('#panel-accesibilidad').on('keydown', function (e) {
        if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
            const $focusables = $(selectorFocoAccesibilidad);
            let index = $focusables.index(document.activeElement);

            if (index === -1) {
                $focusables.first().focus();
            } else {
                if (e.key === 'ArrowDown') {
                    index = (index + 1) % $focusables.length;
                } else {
                    index = (index - 1 + $focusables.length) % $focusables.length;
                }
                $focusables.eq(index).focus();
            }
            e.preventDefault();
        }
    });

    let escalaFuente = 1.0;
    const escalaMin = 0.8;
    const escalaMax = 1.8;
    const pasoEscala = 0.1;

    function aplicarTamanoFuente() {
        $('body').css('font-size', escalaFuente.toFixed(2) + 'em');
    }

    $('#btn-font-plus').on('click', function () {
        if (escalaFuente < escalaMax) {
            escalaFuente += pasoEscala;
            aplicarTamanoFuente();
        }
    });

    $('#btn-font-minus').on('click', function () {
        if (escalaFuente > escalaMin) {
            escalaFuente -= pasoEscala;
            aplicarTamanoFuente();
        }
    });

    const secciones = ['#seccion-header', '#seccion-main', '#teclado', '#btn-accesibilidad', '#seccion-footer'];

    function moverEntreSecciones(direccion) {
        const $secciones = $(secciones.join(','));
        const active = document.activeElement;
        let indexActual = -1;

        $secciones.each(function (i) {
            if (this === active || this.contains(active)) {
                indexActual = i;
            }
        });

        let nuevoIndex;
        if (indexActual === -1) {
            nuevoIndex = direccion > 0 ? 0 : $secciones.length - 1;
        } else {
            nuevoIndex = (indexActual + direccion + $secciones.length) % $secciones.length;
        }

        const elemento = $secciones.get(nuevoIndex);
        if (elemento) {
            elemento.focus();
        }
    }

    $(document).on('keydown', function (e) {
        const active = document.activeElement;
        const $active = $(active);

        if ($active.is('input, textarea, select')) return;
        if ($active.closest('#panel-accesibilidad').length) return;
        if ($active.closest('#teclado').length) return;

        if (e.key === 'ArrowDown') {
            e.preventDefault();
            moverEntreSecciones(1);
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            moverEntreSecciones(-1);
        }
    });

    cargarNuevoJuego();
});
