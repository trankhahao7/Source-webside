let appliedPromoCodeId = null;
let appliedDiscountPercent = 0;
let originalTotal = 0;



// Thêm sản phẩm vào giỏ hàng
async function addToCart(productId, quantity = 1) {
    const res = await fetch('/Orders/AddToCartAjax', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({ productId, quantity }),
        credentials: 'same-origin'
    });
    const data = await res.json();
    alert(data.message || (data.success ? "Đã thêm vào giỏ hàng!" : "Thêm vào giỏ hàng thất bại!"));
}


// Đặt hàng và chuyển trang thanh toán, gửi kèm JWT token
// Đặt hàng và chuyển trang thanh toán, gửi kèm JWT token
async function submitOrder() {
    const cartItems = window.cartItems || [];
    if (!cartItems.length) {
        alert('Giỏ hàng trống!');
        return;
    }

    // ✅ Lấy userId (giả sử bạn đã inject từ Razor hoặc lấy từ localStorage sau đăng nhập)
    const userId = parseInt(localStorage.getItem('userId')); // Hoặc gán bằng cách khác tuỳ dự án

    if (!userId) {
        alert('Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại.');
        return;
    }

    // ✅ Tạo payload đúng định dạng OrderCreateDto
    const items = cartItems.map(item => ({
        productId: item.ProductId,
        quantity: item.Quantity
    }));

    const payload = {
        userId: userId,
        promoCodeId: null, // nếu có mã giảm giá có thể thay đổi chỗ này
        items: items
    };

    const token = localStorage.getItem('token');
    if (!token) {
        alert('Bạn cần đăng nhập để đặt hàng!');
        return;
    }

    const response = await fetch('/Orders/PlaceOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify(payload)
    });

    if (response.ok) {
        const data = await response.json();
        if (data.success) {
            alert('Đặt hàng thành công!');
            window.location.href = '/payment';
        } else {
            alert(data.message || 'Có lỗi xảy ra khi đặt hàng.');
        }
    } else {
        alert('Có lỗi xảy ra khi đặt hàng.');
    }
}



document.getElementById('btnCheckout')?.addEventListener('click', function () {
    if (confirm('Bạn có chắc chắn muốn đặt hàng không?')) {
        submitOrder();
    }
});

//document.querySelectorAll('.quantity-btn').forEach(btn => {
//    btn.addEventListener('click', function () {
//        const productId = this.getAttribute('data-product-id');
//        const input = document.querySelector('.quantity-input[data-product-id="' + productId + '"]');
//        let currentQuantity = parseInt(input.value);
//        let newQuantity = currentQuantity;
//        if (this.classList.contains('increase')) {
//            newQuantity = currentQuantity + 1;
//        } else if (this.classList.contains('decrease')) {
//            newQuantity = currentQuantity - 1;
//        }
//        if (newQuantity < 1) return;
//        updateQuantity(productId, newQuantity).then(() => window.location.reload());
//    });
//});


// Hàm cập nhật số lượng trên server
async function updateQuantity(productId, quantity) {
    const token = localStorage.getItem('token'); // Lấy token từ localStorage nếu có
    await fetch('/Orders/UpdateQuantityAjax', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value,
            ...(token ? { 'Authorization': 'Bearer ' + token } : {})
        },
        body: JSON.stringify({ productId, quantity }),
        credentials: 'same-origin'
    });
}


// Hàm xóa sản phẩm khỏi giỏ hàng trên server
async function removeFromCart(productId) {
    const token = localStorage.getItem('token');
    const res = await fetch('/Orders/RemoveFromCartAjax', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
        },
        body: JSON.stringify({ productId }),
        credentials: 'same-origin'
    });
    const data = await res.json();
    if (data.success) {
        // Xóa khỏi DOM
        const itemDiv = document.querySelector('.cart-item[data-product-id="' + productId + '"]');
        if (itemDiv) itemDiv.remove();
        // Cập nhật lại window.cartItems
        if (window.cartItems) {
            window.cartItems = window.cartItems.filter(item => item.ProductId !== parseInt(productId));
        }
        updateTotalPrice();
        // Nếu giỏ hàng trống, hiển thị thông báo
        if (document.querySelectorAll('.cart-item').length === 0) {
            const container = document.querySelector('.cart-container');
            if (container) {
                container.innerHTML = '<div>Giỏ hàng trống.</div>';
            }
        }
    } else {
        alert(data.message || 'Xóa sản phẩm thất bại!');
    }
}

// Hàm cập nhật tổng tiền trên giao diện và đồng bộ window.cartItems
function updateTotalPrice() {
    let total = 0;
    document.querySelectorAll('.cart-item').forEach(item => {
        const priceText = item.querySelector('.product-price').innerText.replace(/\D/g, '');
        const price = parseInt(priceText) || 0;
        const qty = parseInt(item.querySelector('.quantity-input').value) || 0;
        total += price * qty;
    });

    originalTotal = total;

    const totalDiv = document.querySelector('.total-price');
    if (totalDiv) {
        let displayedTotal = total;
        if (appliedDiscountPercent > 0) {
            displayedTotal = Math.round(total * (100 - appliedDiscountPercent) / 100);
        }
        totalDiv.innerText = 'Tổng cộng: ' + displayedTotal.toLocaleString('vi-VN') + ' VNĐ';
    }
}

// Gắn sự kiện cho nút tăng/giảm và nút xóa sau khi DOM đã load
document.addEventListener('DOMContentLoaded', function () {
    // Sự kiện tăng/giảm số lượng
    document.querySelectorAll('.quantity-btn').forEach(btn => {
        btn.addEventListener('click', async function () {
            const productId = this.getAttribute('data-product-id');
            const input = document.querySelector('.quantity-input[data-product-id="' + productId + '"]');
            let currentQuantity = parseInt(input.value);
            let newQuantity = currentQuantity;
            if (this.classList.contains('increase')) {
                newQuantity = currentQuantity + 1;
            } else if (this.classList.contains('decrease')) {
                newQuantity = currentQuantity - 1;
            }
            if (newQuantity < 1) return;

            await updateQuantity(productId, newQuantity);
            input.value = newQuantity;
            updateTotalPrice();
        });
    });

    // Sự kiện xóa sản phẩm
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const productId = this.getAttribute('data-product-id');
            removeFromCart(productId);
        });
    });
});


// test mã giảm giá


document.addEventListener('DOMContentLoaded', function () {
    // Lưu tổng tiền gốc ban đầu để tính giảm giá
    const totalText = document.querySelector('.total-price')?.innerText || '';
    originalTotal = parseInt(totalText.replace(/\D/g, '')) || 0;

    document.getElementById('checkPromo')?.addEventListener('click', function () {
        const promoCode = document.getElementById('promoCode').value;

        fetch(`/Orders/ValidatePromo?code=${encodeURIComponent(promoCode)}`)
            .then(response => response.json())
            .then(data => {
                const promoMessage = document.getElementById('promoMessage');
                if (data.valid) {
                    appliedPromoCodeId = data.promoCodeId;
                    appliedDiscountPercent = data.discount;
                    promoMessage.innerText = `Mã hợp lệ. Giảm ${data.discount}%`;
                    promoMessage.style.color = 'green';

                    const discountedTotal = Math.round(originalTotal * (100 - data.discount) / 100);
                    const totalDiv = document.querySelector('.total-price');
                    if (totalDiv) {
                        totalDiv.innerText = 'Tổng cộng: ' + discountedTotal.toLocaleString('vi-VN') + ' VNĐ';
                    }
                } else {
                    appliedPromoCodeId = null;
                    appliedDiscountPercent = 0;
                    promoMessage.innerText = 'Mã không hợp lệ hoặc không đủ điều kiện.';
                    promoMessage.style.color = 'red';

                    const totalDiv = document.querySelector('.total-price');
                    if (totalDiv) {
                        totalDiv.innerText = 'Tổng cộng: ' + originalTotal.toLocaleString('vi-VN') + ' VNĐ';
                    }
                }
            })
            .catch(err => {
                console.error("Lỗi khi kiểm tra mã:", err);
            });
    });
});