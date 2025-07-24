// Hàm mở modal chỉnh sửa ảnh
function openEditImageModal(orderId, productSizePricingId, title) {
    // Gán giá trị orderId vào input ẩn 'orderIdInput' trong modal
    document.getElementById('orderIdInput').value = orderId;

    // Gán giá trị productSizePricingId vào input ẩn 'productSizePricingIdInput' trong modal
    document.getElementById('productSizePricingIdInput').value = productSizePricingId;

    // Gán tiêu đề (title) hiện tại vào input 'titleInput', hoặc để trống nếu title là null hoặc undefined
    document.getElementById('titleInput').value = title || '';

    // Tạo instance của modal Bootstrap và hiển thị modal
    var editImageModal = new bootstrap.Modal(document.getElementById('editImageModal'));
    editImageModal.show();
}

// Hàm gửi form chỉnh sửa ảnh qua AJAX
function submitImageForm() {
    // Tạo đối tượng FormData từ form 'imageUploadForm' để gửi dữ liệu dưới dạng multipart/form-data
    var formData = new FormData(document.getElementById('imageUploadForm'));

    // Gửi AJAX request
    $.ajax({
        url: '/OrderDetail/UploadToS3',  // URL endpoint để xử lý upload trên server
        type: 'POST',                    // Phương thức gửi POST
        data: formData,                  // Dữ liệu form bao gồm ảnh và tiêu đề
        processData: false,              // Ngăn jQuery xử lý dữ liệu vì FormData tự xử lý
        contentType: false,              // Ngăn jQuery đặt Content-Type vì FormData tự đặt

        // Xử lý thành công
        success: function (response) {
            // Lấy URL ảnh mới từ phản hồi của server
            var imageUrl = response.imageUrl;

            // Lấy productSizePricingId hiện tại từ input ẩn trong form
            var productSizePricingId = document.getElementById('productSizePricingIdInput').value;

            // Lấy tiêu đề mới từ input 'titleInput' trong form
            var newTitle = document.getElementById('titleInput').value;

            // Cập nhật URL ảnh mới trong bảng tóm tắt (thay thế src của ảnh bằng URL mới)
            document.querySelector(`#productImage_${productSizePricingId}`).src = imageUrl;

            // Cập nhật title mới cho tooltip của ảnh (hiển thị khi người dùng di chuột qua ảnh)
            document.querySelector(`#productImage_${productSizePricingId}`).title = newTitle;

            // Cập nhật nội dung caption (chú thích) bên dưới ảnh bằng tiêu đề mới
            document.querySelector(`#caption_${productSizePricingId}`).innerText = newTitle;


            // Nếu ảnh tải lên thành công, đóng modal
            var editImageModal = bootstrap.Modal.getInstance(document.getElementById('editImageModal'));
            editImageModal.hide(); // Đóng modal

        },

        // Xử lý lỗi
        error: function (xhr, status, error) {
            // Hiển thị lỗi trên console nếu việc upload thất bại
            console.error("Upload failed: ", error);
        }
    });
}
