const conexion = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

conexion.on("SonidoPalabrota", function () {
    sonidoPalabrota();
});


conexion.on("GetMessage", (message) => {
    console.log(message);
    const li = document.createElement("li");

    if (message.avatar && message.avatar !== null) {
        const avatar = document.createElement("img");
        avatar.src = message.avatar;
        avatar.alt = "Avatar";
        avatar.style.width = "30px";
        li.appendChild(avatar);
    }

    const messageText = document.createElement("span");
    messageText.textContent = `${message.user} - ${message.text}`;
    li.appendChild(messageText);

    document.getElementById("lstMensajes").appendChild(li);
});

conexion.on("GetUsers", (users) => {
    const lstUsuarios = document.getElementById("lstUsuarios");
    const lstUsuariosTotales = document.getElementById("lstUsuariosTotales");
    const sala = document.getElementById("sala").value;

    lstUsuarios.innerHTML = "";
    lstUsuariosTotales.innerHTML = "";

    users.forEach(x => {
        const li = document.createElement("li");
        const li2 = document.createElement("li");

        if (x.avatar && x.avatar !== null) {
            const avatar = document.createElement("img");
            avatar.src = x.avatar;
            avatar.alt = "Avatar";
            avatar.style.width = "50px";
            li.appendChild(avatar);
        }

        li2.textContent = x.user;

        const userNameText = document.createElement("span");
        userNameText.textContent = x.user;
        li.appendChild(userNameText);

        lstUsuarios.appendChild(li);

        if (sala == x.room) {
            lstUsuariosTotales.appendChild(li2);
        }
    });
});

document.getElementById("txtUsuario").addEventListener("input", (event) => {
    document.getElementById("btnConectar").disabled = event.target.value === "";
});

document.getElementById("txtMensaje").addEventListener("input", (event) => {
    document.getElementById("btnEnviar").disabled = event.target.value === "";
});

document.getElementById("btnConectar").addEventListener("click", async (event) => {
    const usuario = document.getElementById("txtUsuario").value;
    const avatarUrl = document.getElementById("txtAvatar").value;
    const sala = document.getElementById("sala").value;

    console.log("Avatar URL:", avatarUrl);
    if (sala === "Elige sala") {
        alert("Elige una sala para conectarte");
        return;
    }

    if (conexion.state === signalR.HubConnectionState.Disconnected) {
        try {
            await conexion.start();

            document.getElementById("btnConectar").textContent = "Desconectar";
            document.getElementById("txtUsuario").disabled = true;
            document.getElementById("txtAvatar").disabled = true;
            document.getElementById("txtMensaje").disabled = false;
            document.getElementById("btnEnviar").disabled = false;
            document.getElementById("sala").disabled = true;

            const message = {
                user: usuario,
                room: sala,
                text: "",
                avatar: avatarUrl
            }
            sonidoAleatorioConectar();
            await conexion.invoke("SendMessage", message);
        } catch (error) {
            console.error(error);
        }
    } else if (conexion.state === signalR.HubConnectionState.Connected) {
        await conexion.stop();

        document.getElementById("btnConectar").textContent = "Conectar";
        document.getElementById("txtUsuario").disabled = false;
        document.getElementById("txtAvatar").disabled = false;
        document.getElementById("txtMensaje").disabled = true;
        document.getElementById("btnEnviar").disabled = true;
        document.getElementById("sala").disabled = false;

        sonidoAleatorioDesconectar();
    }
});

document.getElementById("btnEnviar").addEventListener("click", (event) => {
    const usuario = document.getElementById("txtUsuario").value;
    const texto = document.getElementById("txtMensaje").value;
    const avatarUrl = document.getElementById("txtAvatar").value;
    const sala = document.getElementById("sala").value;

    const data = {
        user: usuario,
        room: sala,
        text: texto,
        avatar: avatarUrl
    };

    conexion.invoke("SendMessage", data).catch((error) => {
        console.error(error);
    });

    document.getElementById("txtMensaje").value = "";

    event.preventDefault();
})

//+ ................FUNCIONES CON SONIDOS ALEATORIOS.........................

// Se define un array 'sonidos' que contiene las rutas de archivos de audio.
// A partir de este array, se seleccionará aleatoriamente un sonido.

// Se genera un número aleatorio 'indice' para seleccionar un sonido aleatorio del array.
// Math.random() genera un número decimal entre 0 (inclusive) y 1 (exclusivo),
// al multiplicarlo por la longitud de 'sonidos', que los sacamos con el .lenght, obtenemos un índice válido en el rango de los índices del array.
// Math.floor() redondea este número hacia abajo para obtener un número entero.

// Se crea un objeto 'audio' usando la ruta del sonido seleccionado según el índice aleatorio.
// Se reproduce el sonido seleccionado.

//+Documentación  para mostrar algo aleatorio
// https://ed.team/comunidad/funcion-para-la-reproduccion-aleatoria-con-puro-javascript-no-relaciona-la-cancion-resultante-con-su-titulo
// https://developer.mozilla.org/es/docs/Web/JavaScript/Reference/Global_Objects/Math/random

function sonidoAleatorioConectar() {
    const sonidos =
        [
            '/Sonidos/006150542_prev.mp3',
            //'/Sonidos/siete_caballos_vienen_de_bonanza_chiquito_de_la_calzada.mp3',
            //'/Sonidos/chiquito_de_la_calzada_te_habla_un_hombre_malo_de_la_pradera.mp3',
        ];

    const indice = Math.floor(Math.random() * sonidos.length);
    const audio = new Audio(sonidos[indice]);
    audio.play();
}

function sonidoAleatorioDesconectar() {
    const sonidos =
        [
            '/Sonidos/child-says-good-bye-3-113114.mp3',
            //'/Sonidos/chiquito_de_la_calzada_cobarde.mp3',
            //'/Sonidos/chiquito_de_la_calzada_grito.mp3',
        ];

    const indice = Math.floor(Math.random() * sonidos.length);
    const audio = new Audio(sonidos[indice]);
    audio.play();
    console.log("reproduciendo sonido al desconectar");
}
function sonidoPalabrota() {
    const sonidos =
        [
            '/Sonidos/bleepme.io - bleep sound 2.mp3',
            //'/Sonidos/chiquito_de_la_calzada_cobarde.mp3',
            //'/Sonidos/chiquito_de_la_calzada_grito.mp3',
        ];

    const indice = Math.floor(Math.random() * sonidos.length);
    const audio = new Audio(sonidos[indice]);
    audio.play();

}