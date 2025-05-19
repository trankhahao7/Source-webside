document.getElementById("productForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const token = localStorage.getItem("token");
    const fileInput = document.getElementById("imageFile");

    if (fileInput.files.length === 0) {
        alert("Vui lòng chọn ảnh sản phẩm.");
        return;
    }

    const file = fileInput.files[0];

    const formData = new FormData();
    formData.append("Name", document.getElementById("productName").value);
    formData.append("Description", document.getElementById("description").value);
    formData.append("Price", document.getElementById("price").value);
    formData.append("CategoryId", document.getElementById("categoryId").value);
    formData.append("IsPopular", "false");
    formData.append("IsActive", "false");
    formData.append("ImageFile", file); // Gửi ảnh thật

    try {
        const response = await fetch("/api/ProductApi", {
            method: "POST",
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        });

        if (response.ok) {
            alert("Thêm sản phẩm thành công!");
            location.reload();
        } else {
            const err = await response.text();
            alert("Thêm sản phẩm thất bại!\n" + err);
        }
    } catch (err) {
        alert("Lỗi khi gửi dữ liệu: " + err.message);
    }
});
