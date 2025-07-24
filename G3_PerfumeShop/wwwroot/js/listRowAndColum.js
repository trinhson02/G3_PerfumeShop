document.addEventListener('DOMContentLoaded', function () {

    const fromDateInput = document.getElementById('filter-date-from'); // Lấy phần tử input "fromDate"
    const toDateInput = document.getElementById('filter-date-to'); // Lấy phần tử input "toDate"
    const dateFilterForm = document.getElementById('dateFilterForm'); // Lấy form lọc theo ngày

    // Hàm kiểm tra nếu cả hai trường fromDate và toDate đều có giá trị thì submit form
    function checkAndSubmitForm() {
        if (fromDateInput.value && toDateInput.value) { // Kiểm tra nếu cả fromDate và toDate đều có giá trị
            dateFilterForm.submit(); // Submit form lọc theo ngày
        }
    }

    // Gọi hàm kiểm tra mỗi khi có thay đổi trên fromDate hoặc toDate
    fromDateInput.addEventListener('change', checkAndSubmitForm); // Lắng nghe sự kiện thay đổi trên fromDate
    toDateInput.addEventListener('change', checkAndSubmitForm); // Lắng nghe sự kiện thay đổi trên toDate

    const toggleButtons = document.querySelectorAll('.toggle-column'); // Lấy tất cả các nút toggle cho cột
    const restoreButton = document.getElementById('restoreColumns'); // Lấy nút để khôi phục cột
    const hiddenColumns = new Set(); // Sử dụng Set để theo dõi các cột bị ẩn

    toggleButtons.forEach(button => {
        button.addEventListener('click', function () {
            //5
            const columnIndex = this.dataset.column; // Lấy chỉ số cột từ thuộc tính data-column của nút

            // Thay đổi trạng thái hiển thị của cột
            const table = document.getElementById('multi-filter-select'); // Lấy bảng chứa các cột
            const isHidden = hiddenColumns.has(columnIndex); // Kiểm tra xem cột có đang ẩn không

            for (let row of table.rows) {
                const cell = row.cells[columnIndex];
                cell.style.display = isHidden ? '' : 'none'; // Ẩn hoặc hiện cột tùy thuộc vào trạng thái
            }

            // Cập nhật trạng thái cột
            if (isHidden) {
                hiddenColumns.delete(columnIndex); // Nếu cột đang ẩn thì xóa khỏi Set
                this.textContent = '-'; // Đổi lại thành nút '-'
            } else {
                hiddenColumns.add(columnIndex); // Nếu cột hiện thì thêm vào Set
                this.textContent = '+'; // Đổi lại thành nút '+'
            }
        });
    });

    restoreButton.addEventListener('click', function () {
        if (hiddenColumns.size > 0) { // Kiểm tra nếu có cột nào bị ẩn
            const columnToRestore = Array.from(hiddenColumns)[0]; // Lấy cột đầu tiên trong danh sách cột ẩn

            // Khôi phục hiển thị cột
            const table = document.getElementById('multi-filter-select'); // Lấy bảng chứa các cột
            for (let row of table.rows) {
                const cell = row.cells[columnToRestore];
                cell.style.display = ''; // Hiển thị lại cột
            }

            // Khôi phục trạng thái của nút
            const button = toggleButtons[columnToRestore];
            button.textContent = '-'; // Đổi lại thành '-'

            hiddenColumns.delete(columnToRestore); // Xóa cột khỏi danh sách ẩn

        } else {
            alert('Tất cả các cột đã được hiển thị.'); // Thông báo nếu không có cột nào bị ẩn
        }
    });
});
