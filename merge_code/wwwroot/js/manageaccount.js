document.addEventListener('DOMContentLoaded', function () {
    const batchDeleteBtn = document.getElementById('batchDeleteBtn');

    async function deleteSelectedCustomers() {
        const checkboxes = document.querySelectorAll('.select-customer:checked');
        const ids = Array.from(checkboxes).map(cb => cb.dataset.id);

        if (ids.length === 0) {
            alert("Vui lòng chọn ít nhất một khách hàng để xóa.");
            return;
        }

        if (!confirm(`Bạn có chắc muốn xóa ${ids.length} khách hàng không?`)) return;

        const token = localStorage.getItem('token');
        let successCount = 0;

        for (let id of ids) {
            try {
                const res = await fetch(`/api/UserApi/${id}`, {
                    method: 'DELETE',
                    headers: { 'Authorization': `Bearer ${token}` }
                });

                if (res.ok) {
                    successCount++;
                } else {
                    console.error(`Không xóa được khách hàng ID ${id}`);
                }
            } catch (err) {
                console.error(`Lỗi xóa khách hàng ID ${id}`, err);
            }
        }

        alert(`Đã xóa ${successCount}/${ids.length} khách hàng.`);
        location.reload(); // Reload để cập nhật bảng
    }

    if (batchDeleteBtn) {
        batchDeleteBtn.addEventListener('click', deleteSelectedCustomers);
    }
});
