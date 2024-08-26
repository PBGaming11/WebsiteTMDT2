$(document).ready(function () {
    // Function to update the cart item count
    function updateCartItemCount() {
        $.getJSON('/ShoppingCart/GetCartItemCount', function (data) {
            $('#checkout_items').text(data);
        });
    }

    // Function to update the cart total price
    function updateCartTotal(newTotal) {
        $('#cartTotal').text(newTotal.toLocaleString('vi-VN') + ' Đ');
    }

    // Function to fetch the cart total from the server and update the UI
    function fetchCartTotal() {
        $.getJSON('/ShoppingCart/GetCartTotal', function (data) {
            if (data.total) {
                updateCartTotal(data.total);
            }
        });
    }

    // Call these functions when the page loads to ensure the count and total are up-to-date
    updateCartItemCount();
    fetchCartTotal();

    // Handle click event for adding items to the cart
    $(document).on('click', '#addToCartButton', function (event) {
        event.preventDefault(); // Prevent default action

        var productId = $(this).data('id'); // Get product ID from data-id
        var quantity = 1; // Default quantity or get from input if available

        var item = {
            productId: productId,
            quantity: quantity
        };

        $.ajax({
            url: '/ShoppingCart/AddToCart',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(item),
            success: function (response) {
                if (response.success) {
                    // Update cart item count and total price
                    updateCartItemCount();
                    updateCartTotal(response.newTotal);

                    alert('Sản phẩm đã được thêm vào giỏ hàng.');
                } else {
                    alert('Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.');
                }
            },
            error: function () {
                alert('Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.');
            }
        });
    });

    // Handle click event for removing items from the cart
    $(document).on('click', '.remove-from-cart', function (e) {
        e.preventDefault();
        var productId = $(this).data('id');

        $.ajax({
            type: 'POST',
            url: '/ShoppingCart/RemoveFromCart',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    updateCartItemCount();
                    fetchCartTotal(); // Fetch the updated total price
                } else {
                    alert('Failed to remove item from cart.');
                }
            },
            error: function () {
                alert('An error occurred while removing the item from the cart.');
            }
        });
    });

    // Handle change event for cart item quantity
    $(document).on('change', '.pro-qty-2 input.cart-item-quantity', function () {
        var $input = $(this);
        var quantity = parseInt($input.val(), 10);
        var productId = $input.data('id');

        if (quantity > 0) {
            // Send update request to the server
            $.ajax({
                url: '/ShoppingCart/UpdateCartItem',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ productId: productId, quantity: quantity }),
                success: function (response) {
                    if (response.success) {
                        // Update cart item count and total price
                        updateCartItemCount();
                        fetchCartTotal(); // Fetch the updated total price
                    } else {
                        alert('Error updating cart item.');
                    }
                },
                error: function () {
                    alert('Error updating cart item.');
                }
            });
        } else {
            alert('Quantity must be greater than 0.');
        }
    });
});
