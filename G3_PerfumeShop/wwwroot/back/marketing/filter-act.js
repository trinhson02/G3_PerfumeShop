document.addEventListener('DOMContentLoaded', function () {

    document.getElementById("btnFilter").addEventListener("click", GetFilteredData);
})




function GetFilteredData() {

    notify('top', 'right', 'glyphicon glyphicon-info-sign', 'info', 'animated fadeInDown', 'animated fadeOutUp');

    $.ajax({
        url: $(this).data('url'),
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Lặp qua danh sách dữ liệu và hiển thị trên trang
            $.each(data, function (index, item) {
                console.log(item.name + " " + item.price);
            });
        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
        }
    });
}

