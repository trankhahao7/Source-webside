document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('addCustomerForm');

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const username = document.getElementById('username').value.trim();
        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value.trim();
        const phone = document.getElementById('phone').value.trim();
        const fullname = document.getElementById('fullname').value.trim();

        // Kiểm tra cơ bản
        if (!username || !email || !password || !phone || !fullname) {
            alert("Vui lòng điền đầy đủ thông tin.");
            return;
        }

        const token = localStorage.getItem('token');
        if (!token) {
            alert("Bạn chưa đăng nhập.");
            return;
        }

        const userDto = {
            username: username,
            passwordHash: password,
            email: email,
            fullName: fullname,
            phone: phone,
        };

        try {
            const response = await fetch('/api/UserApi', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(userDto)
            });

            if (response.ok) {
                alert("Thêm khách hàng thành công!");
                form.reset();
            } else {
                const error = await response.text();
                alert("Thêm thất bại: " + error);
            }
        } catch (err) {
            console.error("Lỗi gửi yêu cầu:", err);
            alert("Đã xảy ra lỗi khi thêm khách hàng.");
        }
    });
});
