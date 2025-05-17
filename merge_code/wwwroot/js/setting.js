document.addEventListener("DOMContentLoaded", function () {
    const API_BASE_URL = '/api/my-profile';
    const messageContainer = document.getElementById('messageContainer');
    const logoutButton = document.getElementById('logoutButton');

    function getToken() {
        return localStorage.getItem('token');
    }

    function displayMessage(message, type = 'success') {
        if (!messageContainer) return;
        messageContainer.innerHTML = `<div class="message ${type}">${message}</div>`;
        setTimeout(() => messageContainer.innerHTML = '', 5000);
    }

    // === Hàm validate đơn giản ===
    function validateForm({ email, phone, newPassword, confirmPassword }) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!email || !emailRegex.test(email)) {
            alert("Email không hợp lệ!");
            return false;
        }

        const phoneRegex = /^\+?\d{7,15}$/;
        if (phone && !phoneRegex.test(phone)) {
            alert("Số điện thoại không hợp lệ!");
            return false;
        }

        if (newPassword !== undefined && confirmPassword !== undefined) {
            if (!newPassword || newPassword.length < 6) {
                alert("Mật khẩu mới phải ít nhất 6 ký tự!");
                return false;
            }
            if (newPassword !== confirmPassword) {
                alert("Mật khẩu mới và xác nhận mật khẩu không khớp!");
                return false;
            }
        }

        return true;
    }
    // ===================== PROFILE INFO ========================
    const fullNameInput = document.getElementById('fullName');
    const emailInput = document.getElementById('email');
    const phoneInput = document.getElementById('phone');
    const saveChangesButton = document.getElementById('saveChangesButton');

    if (fullNameInput && emailInput && phoneInput && saveChangesButton) {
        async function fetchUserProfile() {
            const token = getToken();
            if (!token) {
                displayMessage('Bạn chưa đăng nhập hoặc phiên đã hết hạn.', 'error');
                return;
            }

            try {
                const response = await fetch(API_BASE_URL, {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    const userData = await response.json();
                    fullNameInput.value = userData.fullName || '';
                    emailInput.value = userData.email || '';
                    phoneInput.value = userData.phone || '';
                    alert(JSON.stringify(userData))
                } else if (response.status === 401) {
                    displayMessage('Xác thực thất bại. Vui lòng đăng nhập lại.', 'error');
                    localStorage.removeItem('authToken');
                } else {
                    const errorData = await response.json();
                    displayMessage(`Lỗi tải thông tin: ${errorData.message || response.statusText}`, 'error');
                }
            } catch (error) {
                console.error('Error fetching profile:', error);
                displayMessage('Không thể kết nối đến máy chủ để tải thông tin.', 'error');
            }
        }

        document.querySelectorAll(".edit-button").forEach((button) => {
            button.addEventListener("click", function () {
                const targetInputId = this.getAttribute('data-target');
                const inputElement = document.getElementById(targetInputId);
                if (inputElement) {
                    const isCurrentlyDisabled = inputElement.disabled;
                    inputElement.disabled = !isCurrentlyDisabled;
                    this.textContent = inputElement.disabled ? "Edit" : "Cancel";
                }
            });
        });

        saveChangesButton.addEventListener('click', async function () {
            const token = getToken();
            if (!token) {
                displayMessage('Bạn chưa đăng nhập hoặc phiên đã hết hạn.', 'error');
                return;
            }

            const updatedData = {
                fullName: fullNameInput.value,
                email: emailInput.value,
                phone: phoneInput.value
            };

            // Validate email và phone trước khi gửi
            if (!validateForm({ email: updatedData.email, phone: updatedData.phone })) {
                return; // Không gửi nếu không hợp lệ
            }

            try {
                const response = await fetch(API_BASE_URL, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(updatedData)
                });

                if (response.ok) {
                    displayMessage('Thông tin cá nhân đã được cập nhật thành công!', 'success');
                    [fullNameInput, emailInput, phoneInput].forEach(input => input.disabled = true);
                    document.querySelectorAll(".edit-button").forEach(btn => btn.textContent = "Edit");
                } else if (response.status === 401) {
                    displayMessage('Xác thực thất bại. Vui lòng đăng nhập lại.', 'error');
                } else {
                    const errorResult = await response.json();
                    displayMessage(`Lỗi cập nhật: ${errorResult.message || errorResult.title || 'Không rõ lỗi'}`, 'error');
                }
            } catch (error) {
                console.error('Error updating profile:', error);
                displayMessage('Không thể kết nối đến máy chủ để cập nhật.', 'error');
            }
        });

        fetchUserProfile();
    }

    // ===================== PASSWORD SECTION ========================
    const currentPasswordInput = document.getElementById('currentPassword');
    const newPasswordInput = document.getElementById('newPassword');
    const confirmPasswordInput = document.getElementById('confirmPassword');
    const changePasswordBtn = document.getElementById('changePasswordBtn');
    const savePasswordBtn = document.getElementById('savePasswordBtn');

    function togglePasswordFields(enable) {
        [currentPasswordInput, newPasswordInput, confirmPasswordInput].forEach(input => {
            input.disabled = !enable;
            if (!enable) input.value = '';
        });
    }

    if (changePasswordBtn && savePasswordBtn) {
        togglePasswordFields(false); // disable all at start

        changePasswordBtn.addEventListener('click', function () {
            togglePasswordFields(true);
            changePasswordBtn.style.display = 'none';
            savePasswordBtn.style.display = 'inline-block';
        });

        savePasswordBtn.addEventListener('click', async function () {
            const current = currentPasswordInput.value.trim();
            const newPass = newPasswordInput.value.trim();
            const confirm = confirmPasswordInput.value.trim();
            const token = getToken();

            if (!current || !newPass || !confirm) {
                return displayMessage("Vui lòng nhập đầy đủ thông tin.", 'error');
            }

            // Validate mật khẩu trước khi gửi
            if (!validateForm({ email: emailInput.value, phone: phoneInput.value, newPassword: newPass, confirmPassword: confirm })) {
                return;
            }

            if (newPass !== confirm) {
                return displayMessage("Mật khẩu mới và xác nhận không khớp.", 'error');
            }

            try {
                const response = await fetch(`${API_BASE_URL}/change-password`, {
                    method: 'PUT',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        currentPassword: current,
                        newPassword: newPass
                    })
                });

                if (response.ok) {
                    displayMessage("Đổi mật khẩu thành công!");
                    togglePasswordFields(false);
                    changePasswordBtn.style.display = 'inline-block';
                    savePasswordBtn.style.display = 'none';
                } else {
                    const error = await response.text();
                    displayMessage(error || "Đổi mật khẩu thất bại.", 'error');
                }
            } catch (err) {
                console.error(err);
                displayMessage("Lỗi kết nối đến máy chủ.", 'error');
            }
        });
    }

    // ===================== LOGOUT ========================
    if (logoutButton) {
        logoutButton.addEventListener('click', function (e) {
            e.preventDefault();
            localStorage.removeItem('token');
            localStorage.removeItem('userId');
            localStorage.removeItem('username');
            localStorage.removeItem('role');
            localStorage.removeItem('email');
            window.location.href = '/';
        });
    }
});
