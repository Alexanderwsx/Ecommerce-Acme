document.addEventListener("DOMContentLoaded", function () {

    let cart = JSON.parse(localStorage.getItem('cart') || '[]');
    renderCart(cart);

    // Después de renderizar, añade los eventos
    addEventsToButtons();
});

function renderCart(cart) {
    let container = document.getElementById("cartItemsContainer");
    let total = 0;
    let htmlContent = '<table class="table"><thead><tr><th>Productos</th> <th>Cantidad</th> <th>Precio</th><th>Total</th><th></th></tr></thead>'; // Header del table

    cart.forEach((item, index) => {
        total += item.Count * item.Price;
        htmlContent += `
            <tr>
                <td>${item.ProductName}</td>
                <td>${item.Count}</td>
                <td>${item.Price}</td>
                <td>${(item.Count * item.Price).toFixed(2)}</td>
                <td><div>
                <button data-id="${index}" class="add">+</button>
                <button data-id="${index}" class="subtract">-</button>
                <button data-id="${index}" class="delete">🗑️</button>
                </div></td>
            </tr>`;
    });

    htmlContent += '</table>';
    htmlContent += `<h3>Total General: ${total.toFixed(2)}</h3>`;

    container.innerHTML = htmlContent;
}

function addEventsToButtons() {
    let addButton = document.querySelectorAll('.add');
    let subtractButton = document.querySelectorAll('.subtract');
    let deleteButton = document.querySelectorAll('.delete');

    deleteButton.forEach(button => {
        button.addEventListener('click', function (e) {
            let id = e.target.getAttribute('data-id');
            deleteProduct(id);
        });
    });

    addButton.forEach(button => {
        button.addEventListener('click', function (e) {
            let id = e.target.getAttribute('data-id');
            incrementProduct(id);
        });
    });

    subtractButton.forEach(button => {
        button.addEventListener('click', function (e) {
            let id = e.target.getAttribute('data-id');
            decrementProduct(id);
        });
    });
}

function incrementProduct(id) {
    let cart = JSON.parse(localStorage.getItem('cart'));
    if (cart[id].Count < cart[id].Stock) {
        cart[id].Count += 1;
        localStorage.setItem('cart', JSON.stringify(cart));
        renderCart(cart);
        addEventsToButtons();
    } else {
        alert("No puedes añadir más de este producto. Stock limitado.");
    }
}

function deleteProduct(id) {
    let cart = JSON.parse(localStorage.getItem('cart'));
    cart.splice(id, 1); // This will remove the item at the given index
    localStorage.setItem('cart', JSON.stringify(cart));
    renderCart(cart);
    addEventsToButtons();
}

function decrementProduct(id) {
    let cart = JSON.parse(localStorage.getItem('cart'));
    if (cart[id].Count > 1) { // Suponiendo un mínimo de 1 por producto
        cart[id].Count -= 1;
        localStorage.setItem('cart', JSON.stringify(cart));
        renderCart(cart);
        addEventsToButtons();
    } else {
        // Lógica si quieres eliminar el producto cuando llega a 0
         cart.splice(id, 1);
         localStorage.setItem('cart', JSON.stringify(cart));
         renderCart(cart);
         addEventsToButtons();
    }
}
