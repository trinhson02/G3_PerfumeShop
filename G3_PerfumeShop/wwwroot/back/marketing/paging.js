document.addEventListener('DOMContentLoaded', function () {
    document.getElementById("demoPaging").addEventListener("keyup", function (event) {
        if (event.key === "Enter") {
            GetPaging($(this).val());
        }
    });
})

function GetPaging(pageNum) {
    $.ajax({
        url: "/Marketing/GetFilteredData",
        type: 'GET',
        data: {
            n: parseInt(pageNum, 10)
        },
        dataType: 'json',
        success: function (data) {
            // Lặp qua danh sách dữ liệu và hiển thị trên trang
            $.each(data, function (index, item) {
                
                console.log(item.pagingNum);
                var holder = document.getElementById('holder');
                holder.innerHTML = '';

                data.listProduct.forEach(product => {
                    holder.innerHTML += '<p>' + product.name + '</p>';
                    console.log(product.name); // In ra name của từng sản phẩm
                });
            });
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
            alert(xhr.responseText);
        }
    });
}