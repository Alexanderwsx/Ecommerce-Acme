document.getElementById("loginForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Evitar el envío del formulario de forma predeterminada

    var localCart = localStorage.getItem('cart');

    var formData = new FormData(this);

    // Solo agregar localCart a formData si no es null
    if (localCart && localCart !== "null") {
        formData.append("localCart", localCart);
    }

    fetch(this.action, {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                localStorage.removeItem('cart'); // Eliminar el carrito del localStorage
                window.location.href = data.redirectUrl; // Redirigir al usuario
            } else {
                alert("Error al iniciar sesión");
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
});
