document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('editProductForm');
    form.addEventListener('submit', async e => {
        e.preventDefault();

        const productId = document.getElementById('productId').value;
        const productName = document.getElementById('productName').value;
        const description = document.getElementById('description').value;
        const price = parseFloat(document.getElementById('price').value);
        const categoryId = parseInt(document.getElementById('categoryId').value);
        const imageFile = document.getElementById('imageFile').files[0];

        const token = localStorage.getItem('token');
        if (!token) return alert('Bạn cần đăng nhập');

        const formData = new FormData();
        formData.append('ProductId', productId);
        formData.append('Name', productName);  
        formData.append('Description', description);
        formData.append('Price', price);
        formData.append('CategoryId', categoryId);
        if (imageFile) formData.append('ImageFile', imageFile);


        try {
            const response = await fetch(`/api/ProductApi/${productId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': 'Bearer ' + token
                },
                body: formData
            });

            if (response.ok) {
                alert('Cập nhật sản phẩm thành công');
                window.location.href = '/Manager/Product';
            } else {
                alert('Lỗi khi cập nhật sản phẩm');
            }
        } catch (err) {
            console.error(err);
            alert('Lỗi kết nối server');
        }
    });
});