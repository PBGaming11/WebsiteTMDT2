$(document).ready(function () {
    // Xử lý sự kiện click cho các nút thêm vào giỏ hàng
    $(document).on('click', '#addToCartButton', function (event) {
        event.preventDefault(); // Ngăn chặn hành động mặc định của liên kết

        var productId = $(this).data('id'); // Lấy ID sản phẩm từ thuộc tính data-id
        var quantity = 1; // Hoặc lấy từ một trường input nếu có

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
                    // Tăng số lượng sản phẩm trong giỏ hàng
                    var currentCount = parseInt($('#checkout_items').text());
                    $('#checkout_items').text(currentCount + 1);

                    // Cập nhật tổng tiền
                    updateCartTotal(response.newTotal);

                    alert('Sản phẩm đã được thêm vào giỏ hàng.');
                } else {
                    alert('Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.');
                }
            },
            error: function (xhr, status, error) {
                alert('Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.');
            }
        });
    });

    function updateCartTotal(newTotal) {
        $('#cartTotal').text(newTotal.toLocaleString('vi-VN') + ' Đ');
        $('#checkoutTotal').text(newTotal.toLocaleString('vi-VN') + ' Đ');
    }

    // Cập nhật giá trị tổng tiền khi trang được tải
    var initialTotal = parseFloat($('#cartTotal').text().replace(/[^0-9.-]+/g, ""));
    updateCartTotal(initialTotal);
});
