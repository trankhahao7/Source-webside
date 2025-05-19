document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('search');
    const searchBtn = document.getElementById('searchBtn');
    const deleteBtn = document.getElementById('deleteBtn');
    const editBtn = document.getElementById('editBtn');
    const tableBody = document.getElementById('tableBody');
    function logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('userId');
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        localStorage.removeItem('email');
        window.location.href = '/';
    }

    async function deleteSelectedProducts() {
        const selectedCheckboxes = document.querySelectorAll('.select-product:checked');
        const selectedIds = Array.from(selectedCheckboxes).map(cb => parseInt(cb.dataset.id));

        if (selectedIds.length === 0) {
            alert("Vui lòng chọn ít nhất một sản phẩm để xóa.");
            return;
        }

        const confirmDelete = confirm(`Bạn có chắc muốn xóa ${selectedIds.length} sản phẩm đã chọn không?`);
        if (!confirmDelete) return;

        const token = localStorage.getItem('token');
        if (!token) {
            console.error("Token không tìm thấy.");
            logout();
            return;
        }

        let successCount = 0;
        for (const id of selectedIds) {
            try {
                const response = await fetch(`/api/ProductApi/${id}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (response.ok) {
                    successCount++;
                } else {
                    const errorText = await response.text();
                    console.error(`Xóa thất bại (ID ${id}): ${errorText}`);
                    if (response.status === 401 || response.status === 403) {
                        alert("Không có quyền xóa. Đăng xuất.");
                        logout();
                        return;
                    }
                }
            } catch (err) {
                console.error(`Lỗi khi xóa sản phẩm ID ${id}:`, err);
            }
        }

        alert(`Đã xóa ${successCount}/${selectedIds.length} sản phẩm.`);
        location.reload();
    }

    function handleSearch() {
        const keyword = searchInput.value.toLowerCase();
        const rows = tableBody.querySelectorAll('tr');

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(keyword) ? '' : 'none';
        });
    }

    function handleEdit() {
        const selectedCheckboxes = document.querySelectorAll('.select-product:checked');
        if (selectedCheckboxes.length === 0) {
            alert("Vui lòng chọn một sản phẩm để chỉnh sửa.");
            return;
        }

        if (selectedCheckboxes.length > 1) {
            alert("Chỉ được chọn một sản phẩm để chỉnh sửa.");
            return;
        }

        const selectedId = selectedCheckboxes[0].dataset.id;
        window.location.href = `/Manager/UpdateProduct/${selectedId}`;
    }

    if (searchBtn) {
        searchBtn.addEventListener('click', handleSearch);
    }

    if (searchInput) {
        searchInput.addEventListener('keypress', function (event) {
            if (event.key === 'Enter') {
                event.preventDefault();
                handleSearch();
            }
        });
    }

    if (deleteBtn) {
        deleteBtn.addEventListener('click', deleteSelectedProducts);
    }

    if (editBtn) {
        editBtn.addEventListener('click', handleEdit);
    }
});