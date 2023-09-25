$(document).ready(function () {
    $(".addToCart").on("click", function (event) {
        event.preventDefault();
        var productId = $(this).data("productid");
        console.log("button clickerds!");
        $.post('/Home/AddToCart', { productId: productId }, function (response) {
            if (response.addToLocalStorage) {
                var cart = JSON.parse(localStorage.getItem("cart") || "[]");

                var existingProduct = cart.find(p => p.ProductId === response.product.ProductId);
                if (existingProduct) {
                    existingProduct.Count += response.product.Count;
                } else {
                    // Aquí adaptamos la estructura del objeto producto
                    var newProduct = {
                        ProductId: response.product.ProductId,
                        Count: response.product.Count,
                        Price: response.product.Price,
                        Product: { Name: response.product.ProductName }  // Usamos el ProductName aquí
                    };
                    cart.push(newProduct);
                }

                localStorage.setItem("cart", JSON.stringify(cart));
                alert("Producto añadido al carritods!");
            }
            console.log(response);
        });

    });
});