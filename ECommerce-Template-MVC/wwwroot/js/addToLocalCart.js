$(document).ready(function () {
    $(".addToCart").on("click", function (event) {
        event.preventDefault();
        var productId = $(this).data("productid");
        $.post('/Home/AddToCart', { productId: productId }, function (response) {
            if (response.addToLocalStorage) {
                var cart = JSON.parse(localStorage.getItem("cart") || "[]");

                var existingProduct = cart.find(p => p.ProductId === response.product.ProductId);
                if (existingProduct) {
                    existingProduct.Count += response.product.Count;
                } else {
                    // Aquí adaptamos la estructura del objeto producto
                    var newProduct = {
                        ProductId: response.product.productId,
                        Count: response.product.count,
                        Price: response.product.price,
                        ProductName: response.product.productName  // Usamos el ProductName aquí
                    };
                    cart.push(newProduct);
                }

                localStorage.setItem("cart", JSON.stringify(cart));
                alert("Producto añadido al carritods!");              
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.error("Error al agregar al carrito: ", textStatus, errorThrown);
        });
    });
});