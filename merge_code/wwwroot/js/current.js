function logout() {
    
     localStorage.removeItem('token');
     localStorage.removeItem('userId');
     localStorage.removeItem('username');
     localStorage.removeItem('role');
     localStorage.removeItem('email');
     window.location.href = '/logout';
 }
document.addEventListener('DOMContentLoaded', function () {
    const guestHeader = document.getElementById('guestheader');
    const adminHeader = document.getElementById('adminheader');
    const customerHeader = document.getElementById('customerheader');

    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');

    if (guestHeader) guestHeader.style.display = 'none';
    if (adminHeader) adminHeader.style.display = 'none';
    if (customerHeader) customerHeader.style.display = 'none';

    if (token) {
        if (role === 'Admin') {
            if (adminHeader) {
                adminHeader.style.display = 'block';
                setupDropdown(adminHeader);
            }
        } else if (role === 'Customer') {
            if (customerHeader) {
                customerHeader.style.display = 'block';
                setupDropdown(customerHeader);
            }
        } 
    } else {
        if (guestHeader) guestHeader.style.display = 'block';
    }

    function setupDropdown(headerElement) {
        if (!headerElement) {
            console.error("Header element is null or undefined.");
            return;
        }
        const userMenu = headerElement.querySelector('.user-menu');
        const userDropdown = headerElement.querySelector('.user-dropdown');
        const logoutLink = headerElement.querySelector('.logout-link');

        // Kiểm tra nếu tìm thấy user-menu và user-dropdown
        if (userMenu && userDropdown) {
            function closeDropdown() {
                userDropdown.style.display = 'none';
            }
            userMenu.addEventListener('click', function (event) {
                event.stopPropagation();
                userDropdown.style.display = (userDropdown.style.display === 'block') ? 'none' : 'block';
            });

            document.addEventListener('click', function (event) {
                if (!userMenu.contains(event.target) && !userDropdown.contains(event.target)) {
                    closeDropdown();
                }
            });

            userDropdown.addEventListener('click', function (event) {
                event.stopPropagation();
            });
        }
        
        if (logoutLink) {
            logoutLink.addEventListener('click', function (event) {
                event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ a (chuyển hướng)
                logout(); // Gọi hàm logout đã định nghĩa ở trên
            });
        }
    }
    if (token && !adminHeader.style.display && !customerHeader.style.display) {
        console.warn("Token có nhưng role không khớp. Hiển thị lại guestHeader để tránh mất giao diện.");
        guestHeader.style.display = 'block';
    }

});
