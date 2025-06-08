// Hàm format ngày an toàn
function formatDate(dateString) {
    if (!dateString) return 'Không xác định';
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return 'Không xác định';

    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');

    return `
        <div class="datetime-wrapper">
            <span class="order-date">${day}/${month}/${year}</span>
            <span class="order-time">${hours}:${minutes}</span>
        </div>
    `;
}



const ORDER_STATUSES = ["Đang xử lý", "Đang giao", "Hoàn tất", "Đã hủy"];
const isAdmin = localStorage.getItem('role') === 'Admin';
let promoCodeMap = {};

// Tải mã giảm giá
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

// Hiển thị danh sách đơn hàng
function renderOrders(orders) {
    const tbody = document.getElementById('tableBody');
    if (!tbody) return;

    if (!orders.length) {
        tbody.innerHTML = '<tr><td colspan="12">Không có đơn hàng nào.</td></tr>';
        return;
    }

    tbody.innerHTML = orders.map(order => {
        const promoCode = promoCodeMap[order.promoCodeId] || 'Không áp dụng';
        const userInfo = `${order.userId} - ${order.username || 'Chưa có tên'}`;
        const payment = order.payment;

        const paymentMethod = payment ? payment.paymentMethod : 'null';
        const paymentStatus = payment ? payment.status : 'Chưa thanh toán';

        // Cột Admin xác nhận (ưu tiên payment.confirmedBy > order.confirmedBy > 'Chưa có')
        const adminConfirmedBy = payment && payment.confirmedBy
            ? payment.confirmedBy
            : (order.confirmedBy || 'Chưa có');

        const paymentCreatedAt = payment ? formatDate(payment.createdAt) : 'Chưa có';

        // Dropdown trạng thái thanh toán (admin + có payment)
        const paymentStatusDropdown = isAdmin && payment
            ? `
                <select data-payment-id="${payment.paymentId}" class="payment-status-dropdown">
                    <option value="Pending" ${payment.status === 'Pending' ? 'selected' : ''}>Pending</option>
                    <option value="Đã nhận tiền" ${payment.status === 'Đã nhận tiền' ? 'selected' : ''}>Đã nhận tiền</option>
                </select>`
            : (payment ? payment.status : 'Chưa thanh toán');

        // Dropdown trạng thái đơn hàng (theo logic yêu cầu)
        let allowedStatuses;
        if (!payment) {
            allowedStatuses = ["Đang xử lý", "Đã hủy"];
        } else if (payment.status === 'Đã nhận tiền') {
            allowedStatuses = ["Đang xử lý", "Đang giao", "Hoàn tất"];
        } else {
            allowedStatuses = ORDER_STATUSES;
        }

        const actionCell = isAdmin
            ? `
                <select data-id="${order.orderId}" class="status-dropdown">
                    ${allowedStatuses.map(status =>
                `<option value="${status}" ${order.status === status ? 'selected' : ''}>${status}</option>`).join('')}
                </select>`
            : `<button class="view-btn" data-id="${order.orderId}">Xem</button>`;

        return `
            <tr>
                <td>${order.orderId}</td>
                <td>${userInfo}</td>
                <td>${formatDate(order.orderDate)}</td>
                <td>${(order.totalAmount || 0).toLocaleString('vi-VN')} VNĐ</td>
                <td>${order.status || 'Chưa xác định'}</td>
                <td>${promoCode}</td>
                <td>${paymentMethod}</td>
                <td>${paymentStatus}</td>
                <td>${adminConfirmedBy}</td>
                <td>${paymentCreatedAt}</td>
                <td>${paymentStatusDropdown}</td>
                <td>${actionCell}</td>
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
        const response = await fetch('https://localhost:7171/api/OrderApi/WithPayments', {
            method: 'GET',
            headers: {
                'accept': 'application/json',
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

// Cập nhật trạng thái đơn hàng
async function updateOrderStatus(orderId, newStatus) {
    const token = localStorage.getItem('token');
    if (!token) return alert('Vui lòng đăng nhập!');
    try {
        const response = await fetch(`https://localhost:7171/api/OrderApi/${orderId}/status`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newStatus)
        });
        if (!response.ok) {
            alert('Cập nhật trạng thái thất bại.');
            return;
        }
        alert('Cập nhật trạng thái thành công.');
        loadOrders();
    } catch (err) {
        console.error(err);
        alert('Lỗi khi cập nhật trạng thái đơn hàng.');
    }
}

// Cập nhật trạng thái thanh toán
async function updatePaymentStatus(paymentId, newStatus) {
    const token = localStorage.getItem('token');
    if (!token) return alert('Vui lòng đăng nhập!');
    try {
        const response = await fetch(`https://localhost:7171/api/PaymentApi/${paymentId}/status`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newStatus)
        });
        if (!response.ok) {
            alert('Cập nhật trạng thái thanh toán thất bại.');
            return;
        }
        alert('Cập nhật trạng thái thanh toán thành công.');
        loadOrders();
    } catch (err) {
        console.error(err);
        alert('Lỗi khi cập nhật trạng thái thanh toán.');
    }
}

// Tìm kiếm
function searchOrders() {
    const keyword = document.getElementById('search').value.toLowerCase();
    const filtered = window.allOrders.filter(order =>
        order.orderId.toString().includes(keyword) ||
        (order.status && order.status.toLowerCase().includes(keyword)) ||
        (promoCodeMap[order.promoCodeId] && promoCodeMap[order.promoCodeId].toLowerCase().includes(keyword))
    );
    renderOrders(filtered);
}

// Modal chi tiết đơn hàng
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

// Modal chi tiết
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

// Hiển thị & đóng modal
function showModal() {
    document.getElementById('orderDetailModal')?.classList.remove('hidden');
}
function closeModal() {
    document.getElementById('orderDetailModal')?.classList.add('hidden');
}

// DOM load
document.addEventListener('DOMContentLoaded', async () => {
    await loadPromoCodes();
    await loadOrders();
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
    document.addEventListener('change', e => {
        if (e.target.classList.contains('status-dropdown')) {
            const orderId = e.target.getAttribute('data-id');
            const newStatus = e.target.value;
            updateOrderStatus(orderId, newStatus);
        }
        if (e.target.classList.contains('payment-status-dropdown')) {
            const paymentId = e.target.getAttribute('data-payment-id');
            const newStatus = e.target.value;
            updatePaymentStatus(paymentId, newStatus);
        }
    });
});
