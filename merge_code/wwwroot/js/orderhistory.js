// Hàm format ngày an toàn
function formatDate(dateString) {
    if (!dateString) return 'Không xác định';

    const normalized = dateString.toString().replace(' ', 'T');
    const date = new Date(normalized);

    if (isNaN(date.getTime())) return 'Không xác định';

    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

// Map promoCodeId => code
let promoCodeMap = {};

// Gọi API để lấy danh sách mã giảm giá
async function loadPromoCodes() {
    const token = localStorage.getItem('token');
    if (!token) return;

    try {
        const response = await fetch('https://localhost:7171/api/PromoApi', {
            headers: {
                'Authorization': 'Bearer ' + token,
                'Accept': 'application/json'
            }
        });

        if (!response.ok) {
            console.warn('Không thể tải mã giảm giá.');
            return;
        }

        const data = await response.json();
        promoCodeMap = Object.fromEntries(data.map(p => [p.promoCodeId, p.code]));
    } catch (error) {
        console.error('Lỗi khi tải mã giảm giá:', error);
    }
}

// Hiển thị danh sách đơn hàng trong bảng
function renderOrders(orders) {
    const tbody = document.getElementById('tableBody');
    if (!tbody) return;

    if (!orders.length) {
        tbody.innerHTML = '<tr><td colspan="6">Không có đơn hàng nào.</td></tr>';
        return;
    }

    tbody.innerHTML = orders.map(order => {
        const promoCode = promoCodeMap[order.promoCodeId] || 'Không áp dụng';

        return `
            <tr>
                <td>${order.orderId}</td>
                <td>${formatDate(order.orderDate)}</td>
                <td>${(order.totalAmount || 0).toLocaleString('vi-VN')} VNĐ</td>
                <td>${order.status || 'Chưa xác định'}</td>
                <td>${promoCode}</td>
                <td><button class="view-btn" data-id="${order.orderId}">Xem</button></td>
            </tr>
        `;
    }).join('');
}

// Gọi API lấy danh sách đơn hàng
async function loadOrders() {
    const token = localStorage.getItem('token');
    if (!token) {
        alert('Vui lòng đăng nhập để xem lịch sử đơn hàng.');
        return;
    }

    try {
        const response = await fetch('https://localhost:7171/api/OrderApi', {
            method: 'GET',
            headers: {
                'accept': 'text/plain',
                'Authorization': 'Bearer ' + token
            }
        });

        if (!response.ok) {
            alert('Lỗi khi tải đơn hàng. Mã lỗi: ' + response.status);
            return;
        }

        const orders = await response.json();
        window.allOrders = orders;
        renderOrders(orders);
    } catch (error) {
        console.error('Lỗi API:', error);
        alert('Đã xảy ra lỗi khi lấy dữ liệu đơn hàng.');
    }
}

// Tìm kiếm đơn hàng
function searchOrders() {
    const keyword = document.getElementById('search').value.toLowerCase();
    const filtered = window.allOrders.filter(order =>
        order.orderId.toString().includes(keyword) ||
        (order.status && order.status.toLowerCase().includes(keyword)) ||
        (promoCodeMap[order.promoCodeId] && promoCodeMap[order.promoCodeId].toLowerCase().includes(keyword))
    );
    renderOrders(filtered);
}

// Gọi API để lấy chi tiết đơn hàng
async function loadOrderDetail(orderId) {
    const token = localStorage.getItem('token');
    if (!token) {
        alert('Vui lòng đăng nhập!');
        return;
    }

    try {
        const response = await fetch(`https://localhost:7171/api/OrderApi/${orderId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Accept': 'application/json'
            }
        });

        if (!response.ok) {
            alert('Không thể tải chi tiết đơn hàng.');
            return;
        }

        const detail = await response.json();
        renderOrderDetailModal(detail);
        showModal();
    } catch (err) {
        console.error(err);
        alert('Lỗi khi tải chi tiết đơn hàng.');
    }
}

// Hiển thị nội dung modal chi tiết đơn hàng
function renderOrderDetailModal(order) {
    const modalBody = document.getElementById('modalBody');
    if (!order.items || order.items.length === 0) {
        modalBody.innerHTML = '<p>Không có sản phẩm trong đơn hàng này.</p>';
        return;
    }

    let totalQuantity = 0;
    let totalPrice = 0;

    const rows = order.items.map(item => {
        const quantity = item.quantity;
        const unitPrice = item.unitPrice;
        const total = quantity * unitPrice;
        totalQuantity += quantity;
        totalPrice += total;

        return `
            <tr>
                <td>${item.productName}</td>
                <td>${quantity}</td>
                <td>${unitPrice.toLocaleString('vi-VN')} VNĐ</td>
                <td>${total.toLocaleString('vi-VN')} VNĐ</td>
            </tr>
        `;
    }).join('');

    modalBody.innerHTML = `
        <table>
            <thead>
                <tr>
                    <th>Sản phẩm</th>
                    <th>Số lượng</th>
                    <th>Đơn giá</th>
                    <th>Thành tiền</th>
                </tr>
            </thead>
            <tbody>${rows}</tbody>
        </table>
        <hr>
        <p><strong>Tổng số lượng:</strong> ${totalQuantity}</p>
        <p><strong>Thành tiền:</strong> ${totalPrice.toLocaleString('vi-VN')} VNĐ</p>
    `;
}

// Hiển thị modal
function showModal() {
    document.getElementById('orderDetailModal')?.classList.remove('hidden');
}

// Đóng modal
function closeModal() {
    document.getElementById('orderDetailModal')?.classList.add('hidden');
}

// Sự kiện DOM load
document.addEventListener('DOMContentLoaded', async () => {
    await loadPromoCodes(); // Lấy mã giảm giá trước
    await loadOrders();     // Rồi mới hiển thị đơn hàng

    document.getElementById('searchBtn')?.addEventListener('click', searchOrders);
    document.getElementById('search')?.addEventListener('keypress', e => {
        if (e.key === 'Enter') searchOrders();
    });

    document.addEventListener('click', e => {
        if (e.target.classList.contains('view-btn')) {
            const id = e.target.getAttribute('data-id');
            loadOrderDetail(id);
        }

        if (e.target.classList.contains('close-btn') || e.target.id === 'orderDetailModal') {
            closeModal();
        }
    });
});
