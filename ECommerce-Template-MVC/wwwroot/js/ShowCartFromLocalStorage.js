document.addEventListener("DOMContentLoaded", function () {

    let cart = JSON.parse(localStorage.getItem('cart') || '[]');
    renderCart(cart);


});

function renderCart(cart) {
    let container = document.getElementById("cartItemsContainer");
    let total = 0;
    let htmlContent = '<table class="table">            <thead><tr><th>Productos</th> <th>Cantidad</th> <th>Precio</th><th>Total</th></tr ></thead > '; // Header del table

    cart.forEach(item => {
        total += item.Count * item.Price;
        htmlContent += `
            <tr>
                <td>${item.ProductName}</td>
                <td>${item.Count}</td>
                <td>${item.Price}</td>
                <td>${(item.Count * item.Price).toFixed(2)}</td>
            </tr>`;
    });

    htmlContent += '</table>';
    htmlContent += `<h3>Total General: ${total.toFixed(2)}</h3>`;

    container.innerHTML = htmlContent;
}
