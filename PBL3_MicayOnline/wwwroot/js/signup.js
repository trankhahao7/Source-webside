// Hàm xử lý validation frontend
function validateSignupForm(username, email, fullname, password, confirmPassword, phone) {
    if (!username) {
        alert('Username is required.');
        return false;
    }
    if (email && !/\S+@\S+\.\S+/.test(email)) {
        alert('Invalid email format.');
        return false;
    }
    if (phone && !/^\d{10,11}$/.test(phone)) {
        alert('Invalid phone number format.');
        return false;
    }
    if (!password) {
        alert('Password is required.');
        return false;
    } else if (password.length < 6) {
        alert('Password must be at least 6 characters.');
        return false;
    }
    if (!confirmPassword) {
        alert('Please confirm password.');
        return false;
    }
    if (password && confirmPassword && password !== confirmPassword) {
        alert('Passwords do not match.');
        return false;
    }

    // Nếu tất cả kiểm tra đều vượt qua
    return true;
}

async function signup() {
    const usernameInput = document.getElementById('username');
    const emailInput = document.getElementById('email');
    const fullnameInput = document.getElementById('fullname');
    const phoneInput = document.getElementById('phone');
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirm-password');

    const username = usernameInput.value.trim();
    const email = emailInput.value.trim();
    const fullname = fullnameInput.value.trim();
    const phone = phoneInput.value.trim();
    const password = passwordInput.value.trim();
    const confirmPassword = confirmPasswordInput.value.trim();

    if (!validateSignupForm(username, email, fullname, password, confirmPassword, phone)) {
        return;
    }
    //alert('Đang xử lý yêu cầu đăng ký...');
    const registrationData = {
        username: username,
        passwordHash: password,
        email: email,
        fullName: fullname,
        phone: phone,
    };
    try {
        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(registrationData),
        });
        //alert('Đang xử lý yêu cầu đăng ký...');
        if (response.ok) {
            alert("Đăng ký thành công! Vui lòng đăng nhập.");
            window.location.href = '/';

        } else {
            const errorText = await response.text();
            console.error('Registration failed:', response.status, errorText);

            // Hiển thị lỗi từ API bằng alert
            alert(`Registration failed: ${errorText || response.statusText}`);
        }

    } catch (error) {
        alert('An error occurred during sign up. Please try again.');
        console.error('Network error or exception:', error);
    }
}
