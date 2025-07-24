function confirmDelete(customerId) {
    if (confirm("Bạn có chắc chắn muốn xóa khách hàng này không?")) {
        fetch('/Customer/Delete/' + customerId, {
            method: 'DELETE'
        })
            .then(response => {
                if (response.ok) {
                    location.reload();
                } else {
                    alert("Có lỗi xảy ra trong quá trình xóa khách hàng.");
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert("Có lỗi xảy ra trong quá trình xóa khách hàng.");
            });
    }
}