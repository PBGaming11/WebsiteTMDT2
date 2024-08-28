$(document).ready(function () {

    function updateCartTotalAmount() {
        $.ajax({
            url: '/Cart/GetCartTotalAmount',
            type: 'GET',
            success: function (response) {
                $('#totalAmount').text(response.totalAmount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' Đ');
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    }

    // Gọi hàm này sau khi bạn cập nhật giỏ hàng (ví dụ: sau khi thêm sản phẩm)
    updateCartTotalAmount();


    // Sự kiện click cho nút thêm vào giỏ hàng
    $(document).on('click', '#addToCartButton', function (e) {
        e.preventDefault(); // Ngăn chặn hành động mặc định của liên kết hoặc nút

        var productId = $(this).data('id'); // Lấy productId từ thuộc tính data-id của nút

        $.ajax({
            url: '/Cart/AddToCart', // Đường dẫn tới action AddToCart trong CartController
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    alert('Đã thêm sản phẩm vào giỏ hàng!');
                    updateCartTotalAmount();
                    // Cập nhật số lượng sản phẩm trong giỏ hàng (nếu cần)
                    $('#cartItemCount').text(response.cartItemCount);
                } else {
                    alert('Có lỗi xảy ra. Vui lòng thử lại!');
                }
            },
            error: function () {
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    });
    $(document).on('change', '.cart-item-quantity', function () {
        var productId = $(this).data('id');
        var quantity = $(this).val();

        $.ajax({
            url: '/Cart/UpdateCart',
            type: 'POST',
            data: {
                productId: productId,
                quantity: quantity // Sử dụng giá trị mới
            },
            success: function (response) {
                if (response.success) {
                    updateCartTotalAmount();
                    $('#totalAmount').text(response.totalAmount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' Đ');
                    $('#totalAmounts').text(response.totalAmount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' Đ');
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    });
    $('.remove-from-cart').on('click', function () {
        var productId = $(this).data('id');

        $.ajax({
            url: '/Cart/RemoveFromCart',
            type: 'POST',
            data: {
                productId: productId
            },
            success: function (response) {
                if (response.success) {
                    // Xóa sản phẩm khỏi HTML
                    $(`a[data-id="${productId}"]`).closest('tr').remove();

                    // Cập nhật tổng tiền và số lượng sản phẩm
                    $('#totalAmounts').text(response.totalAmount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' Đ');
                    $('#totalAmount').text(response.totalAmount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ' Đ');
                    $('#cartItemCount').text(response.cartItemCount);
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    });
});
