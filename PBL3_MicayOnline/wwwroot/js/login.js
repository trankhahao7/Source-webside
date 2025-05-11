function validate() {
    if (!username) {
        alert('Please enter username.');
        return false;
    }
    if (!password) {
        alert('Please enter password.');
        return false;
    }
    if (password.length < 6) {
        alert('Password must be at least 6 characters.');
        return false;
    }
}
async function login(returnURL) {
    if (!returnURL) {
        returnURL = '/';
    }
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');

    const username = usernameInput.value;
    const password = passwordInput.value;

    const loginData = {
        username: username,
        passwordHash: password
    };

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginData),
        });

        if (!response.ok) {
            const errorText = await response.text();
            alert(`Login failed: ${errorText || response.statusText}`);
            console.error('Login failed:', response.status, errorText);
            return;
        }

        const result = await response.json();

        // Lưu trữ token và các thông tin người dùng
        localStorage.setItem('token', result.token);
        localStorage.setItem('userId', result.userId);
        localStorage.setItem('username', result.username);
        localStorage.setItem('role', result.role);
        localStorage.setItem('email', result.email);

        window.location.href = returnURL;

    } catch (error) {
        alert('An error occurred during login. Please try again.');
        console.error('Network error or exception:', error);
    }
}