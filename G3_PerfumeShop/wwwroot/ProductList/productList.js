// Khởi tạo biến lưu số trang hiện tại, mặc định là trang đầu tiên (1)
var page = 1;

// Đối tượng filter để lưu thông tin các bộ lọc khi tìm kiếm hoặc lọc sản phẩm
const filter = {
    // Từ khóa tìm kiếm (nếu có)
    keyword: undefined,
    // Thương hiệu sản phẩm (brand) được chọn làm bộ lọc
    brand: undefined,
    // Giá sản phẩm trong khoảng (from: giá thấp nhất, to: giá cao nhất)
    price: {
        from: undefined, // Giá thấp nhất
        to: undefined    // Giá cao nhất
    },
    // Dung tích (capacity) của sản phẩm trong khoảng
    cap: {
        from: undefined, // Dung tích nhỏ nhất
        to: undefined    // Dung tích lớn nhất
    },
    // Xuất xứ của sản phẩm (ví dụ: Mỹ, Pháp, Việt Nam,...)
    origin: undefined
};

// Đối tượng pagination để quản lý phân trang
const pagination = {
    page: 1,                // Trang hiện tại (mặc định là trang đầu tiên)
    size: 8,                // Số lượng sản phẩm hiển thị trên mỗi trang
    pagingNum: undefined,   // Tổng số trang (sẽ được tính dựa trên kết quả trả về)
    result: 0,              // Tổng số kết quả tìm kiếm
    pagingHolder: undefined,// Nơi chứa các nút phân trang (holder cho UI)
    txtResultNum: undefined,// Thẻ UI hiển thị số lượng kết quả (ví dụ: "20 kết quả")
    inputPagingNum: undefined // Input để nhập số trang muốn chuyển đến
};



// Lấy các tham số từ URL.
const urlParams = new URLSearchParams(window.location.search);

// Đợi DOM được load hoàn chỉnh.
document.addEventListener('DOMContentLoaded', function () {
    HandlePaging(); // Thiết lập thông tin phân trang.
    HandleFilter(); // Gắn các phần tử giao diện bộ lọc vào đối tượng `filter`.
    GetFilterDetail(); // Lấy thông tin chi tiết của các bộ lọc.
    GetProductList(); // Lấy danh sách sản phẩm mặc định.

    // Xử lý khi nhấn nút "Lọc".
    document.getElementById('btn-filter').addEventListener('click', function (event) {
        pagination.page = 1; // Đặt lại trang hiện tại là 1.
        GetProductList(size = pagination.size, page = pagination.page); // Gửi yêu cầu lấy danh sách sản phẩm.
    });

    // Xử lý khi nhấn nút "Reset" để làm mới bộ lọc.
    document.getElementById('btn-reset').addEventListener('click', function (event) {
        pagination.page = 1; // Đặt lại trang hiện tại là 1.
        filter.keyword.value = null; // Xóa từ khóa.
        filter.brand.value = 'All'; // Đặt lại thương hiệu là "All".
        filter.origin.value = 'All'; // Đặt lại xuất xứ là "All".
        filter.price.from.value = null; // Xóa giá từ.
        filter.price.to.value = null; // Xóa giá đến.
        filter.cap.from.value = null; // Xóa dung tích từ.
        filter.cap.to.value = null; // Xóa dung tích đến.
        GetProductList(size = pagination.size, page = pagination.page); // Gửi yêu cầu lấy danh sách sản phẩm.
    });

    // Xử lý khi nhấn nút "Phân trang".
    document.getElementById('btn-paging').addEventListener('click', function (event) {
        const input = Number(pagination.inputPagingNum.value); // Lấy số trang từ input.

        // Kiểm tra số trang hợp lệ và cập nhật trang hiện tại.
        if (input > 0 && input <= pagination.pagingNum) {
            pagination.page = input; // Cập nhật trang hiện tại.
            GetProductList(size = pagination.size, page = pagination.page); // Gửi yêu cầu lấy danh sách sản phẩm.
        }
    });
});

// Hàm xử lý và gán các phần tử bộ lọc.
function HandleFilter() {
    filter.keyword = document.getElementById('input-filter-keyword'); // Từ khóa.
    filter.brand = document.getElementById('brands'); // Thương hiệu.
    filter.origin = document.getElementById('origins'); // Xuất xứ.
    filter.price.from = document.getElementById('input-filter-from-price'); // Giá từ.
    filter.price.to = document.getElementById('input-filter-to-price'); // Giá đến.
    filter.cap.from = document.getElementById('input-filter-from-volume'); // Dung tích từ.
    filter.cap.to = document.getElementById('input-filter-to-volume'); // Dung tích đến.
}

// Hàm thiết lập thông tin phân trang.
function HandlePaging() {
    pagination.page = 1; // Trang hiện tại mặc định là 1.
    pagination.size = 9; // Số sản phẩm trên mỗi trang mặc định là 8.
    pagination.pagingHolder = document.getElementById('pagination-ul'); // Holder cho danh sách phân trang.
    pagination.txtResultNum = document.getElementById('txt-result-num'); // Hiển thị tổng số kết quả.
    pagination.inputPagingNum = document.getElementById('input-paging'); // Input nhập số trang để phân trang.
}
function CreatePagingComponent(size = 7, page = 1) {
    // Hiển thị số kết quả và số trang.
    pagination.txtResultNum.textContent = `Tìm được ${pagination.result} giá trị. Bao gồm ${pagination.pagingNum} trang.`;

    // Kiểm tra nếu có nhiều hơn 1 trang thì mới tạo giao diện phân trang.
    if (pagination.pagingNum > 1) {
        console.log(pagination.pagingNum); // Log số lượng trang để kiểm tra.
        const pagingFragment = document.createDocumentFragment(); // Tạo một fragment để chứa các phần tử phân trang.

        // Tính toán số trang cố định và thay đổi.
        const noChangeNum = Math.floor(size / 2); // Số lượng trang không thay đổi (xung quanh trang hiện tại).
        const topChangeNum = noChangeNum + 1; // Trang trên cùng khi trang hiện tại nhỏ.
        const botChangeNum = Number(pagination.pagingNum) - noChangeNum; // Trang dưới cùng khi trang hiện tại lớn.
        var end;

        // Nếu tổng số trang lớn hơn số lượng trang hiển thị (size).
        if (pagination.pagingNum >= size) {
            var start = page; // Bắt đầu từ trang hiện tại.
            if (start <= topChangeNum) {
                start = 1; // Nếu trang hiện tại gần đầu, bắt đầu từ trang 1.
            } else if (start >= botChangeNum) {
                start = Number(pagination.pagingNum) - size + 1; // Nếu gần cuối, bắt đầu từ trang cuối.
            } else {
                start = page - noChangeNum; // Nếu ở giữa, tính toán từ trang hiện tại.
                end = page - noChangeNum;
            }

            end = start + size - 1; // Tính trang kết thúc.
        } else {
            start = 1; // Nếu tổng số trang nhỏ hơn size, bắt đầu từ trang 1.
            end = pagination.pagingNum; // Kết thúc là tổng số trang.
        }

        // Tạo các phần tử phân trang từ trang `start` đến `end`.
        for (let i = start; i <= end; i++) {
            const li = document.createElement('li'); // Tạo phần tử <li>.
            if (i == page) {
                li.setAttribute('class', 'page-item active'); // Đánh dấu trang hiện tại.
            } else {
                li.setAttribute('class', 'page-item'); // Trang không phải hiện tại.
            }
            const a = document.createElement('a'); // Tạo phần tử <a>.
            a.dataset.page = i; // Gán số trang vào data attribute.
            a.setAttribute('class', 'page-link'); // Gán class cho <a>.
            a.text = i; // Gán text là số trang.

            // Xử lý sự kiện khi nhấn vào số trang.
            a.addEventListener('click', function (event) {
                pagination.page = Number(this.dataset.page); // Cập nhật trang hiện tại.
                GetProductList(size = pagination.size, page = pagination.page); // Lấy danh sách sản phẩm mới.
            });
            li.append(a); // Gắn <a> vào <li>.

            pagingFragment.append(li); // Gắn <li> vào fragment.
        }

        // Thêm các nút điều hướng "<<" và ">>" nếu số trang lớn hơn `size`.
        if (pagination.pagingNum > size) {
            // Nút "<<" để quay về trang đầu tiên.
            const liStart = document.createElement('li');
            liStart.setAttribute('class', 'page-item');
            const aStart = document.createElement('a');
            aStart.dataset.page = 1;
            aStart.setAttribute('class', 'page-link');
            aStart.text = '<<';
            aStart.addEventListener('click', function (event) {
                pagination.page = Number(this.dataset.page); // Chuyển về trang đầu tiên.
                this.parentElement.setAttribute('class', 'page-item disabled'); // Vô hiệu hóa nút.
                GetProductList(size = pagination.size, page = pagination.page); // Lấy danh sách sản phẩm.
            });

            // Nút ">>" để chuyển đến trang cuối cùng.
            const liEnd = document.createElement('li');
            liEnd.setAttribute('class', 'page-item');
            const aEnd = document.createElement('a');
            aEnd.dataset.page = pagination.pagingNum;
            aEnd.setAttribute('class', 'page-link');
            aEnd.text = '>>';
            aEnd.addEventListener('click', function (event) {
                pagination.page = Number(this.dataset.page); // Chuyển đến trang cuối.
                this.parentElement.setAttribute('class', 'page-item disabled'); // Vô hiệu hóa nút.
                GetProductList(size = pagination.size, page = pagination.page); // Lấy danh sách sản phẩm.
            });

            liStart.append(aStart); // Gắn <a> vào <li> (nút "<<").
            liEnd.append(aEnd); // Gắn <a> vào <li> (nút ">>").

            pagingFragment.append(liStart); // Gắn nút đầu tiên vào fragment.
            pagingFragment.append(liEnd); // Gắn nút cuối cùng vào fragment.
        }

        // Xóa nội dung cũ và gắn fragment mới vào holder.
        pagination.pagingHolder.innerHTML = ``;
        pagination.pagingHolder.append(pagingFragment);
    } else {
        pagination.pagingHolder.innerHTML = ``; // Nếu chỉ có 1 trang, không hiển thị phân trang.
    }
}

function UpdatePaging() {

}
function GetFilterDetail() {
    // Gửi yêu cầu AJAX để lấy thông tin chi tiết bộ lọc
    $.ajax({
        url: '/Product/GetFilterDetail',
        type: 'GET',
        datatype: 'json',
        success: function (data) {
            // Lấy danh sách quốc gia và thương hiệu từ dữ liệu trả về
            const origins = data.origins;
            const brands = data.brands;

            // Xử lý danh sách quốc gia (origins)
            const originFragment = document.createDocumentFragment();
            var originsHolder = document.getElementById('origins');

            // Thêm tùy chọn "Tất cả" mặc định
            var originFirstElem = document.createElement("option");
            originFirstElem.value = "All";
            originFirstElem.text = "Tất cả";
            originFirstElem.selected = true;
            originFragment.append(originFirstElem);

            // Tạo các tùy chọn quốc gia từ dữ liệu
            origins.forEach(i => {
                const opt = document.createElement('option');
                opt.value = i.CountryCode;
                opt.text = i.Name;
                originFragment.appendChild(opt);
            });
            originsHolder.append(originFragment);

            // Xử lý danh sách thương hiệu (brands)
            const brandFragment = document.createDocumentFragment();
            var brandsHolder = document.getElementById('brands');

            // Thêm tùy chọn "Tất cả" mặc định
            var brandFirstElem = document.createElement("option");
            brandFirstElem.value = "All";
            brandFirstElem.text = "Tất cả";
            brandFirstElem.selected = true;
            brandFragment.append(brandFirstElem);

            // Tạo các tùy chọn thương hiệu từ dữ liệu
            brands.forEach(i => {
                const opt = document.createElement('option');
                opt.value = i.Name;
                opt.text = i.Name;
                brandFragment.appendChild(opt);
            });
            brandsHolder.append(brandFragment);
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi nếu yêu cầu không thành công
            console.error('Lỗi khi lấy dữ liệu:', error);
            alert(xhr.responseText);
        }
    });
}

function GetProductList(size = 9, page = 1) {
    // Lấy container để hiển thị danh sách sản phẩm
    var productsHolder = document.getElementById('product-list-container');

    // Gửi yêu cầu AJAX để lấy danh sách sản phẩm
    $.ajax({
        url: '/Product/GetProductList',
        type: 'GET',
        data: {
            // Các tham số bộ lọc và phân trang
            size: size,
            page: page,
            keyword: filter.keyword.value || null,
            brand: filter.brand.value === 'All' ? null : filter.brand.value,
            countryCode: filter.origin.value === 'All' ? null : filter.origin.value,
            priceFrom: filter.price.from.value || null,
            priceTo: filter.price.to.value || null,
            capFrom: filter.cap.from.value || null,
            capTo: filter.cap.to.value || null
        },
        datatype: 'json',
        success: function (data) {
            // Cập nhật thông tin phân trang
            pagination.page = Number(page);
            pagination.size = Number(size);
            pagination.pagingNum = Number(data.pagingNum);
            pagination.result = Number(data.resultNum);

            // Thiết lập giá trị tối đa cho input phân trang
            pagination.inputPagingNum.setAttribute('max', pagination.pagingNum);

            // Tạo component phân trang
            CreatePagingComponent(size = 7, page = page);

            // Lấy danh sách sản phẩm từ dữ liệu trả về
            const products = data.products;

            // Tạo fragment để lưu trữ HTML các sản phẩm
            const productFragment = document.createDocumentFragment();

            products.forEach(i => {
                // Tạo container cho từng sản phẩm
                let productContainer = document.createElement('div');
                productContainer.setAttribute('class', 'product-container');

                // Kiểm tra sản phẩm có phải sản phẩm mới không
                if (new Date(i.ReleaseYear).getFullYear() >= new Date().getFullYear()) {
                    productContainer.innerHTML = `<div class="product-label">MỚI</div>`;
                }

                // Lấy giá thấp nhất và cao nhất của các kích cỡ sản phẩm
                let minPrice = Math.min(...i.ProductSizePricings.map(function (p) { return p.Price }));
                let maxPrice = Math.max(...i.ProductSizePricings.map(function (p) { return p.Price }));

                // Tạo nội dung cho kích thước sản phẩm
                let sizeContent = '';
                let productSizePricing = i.ProductSizePricings.sort((a, b) => a.Size - b.Size);
                productSizePricing.forEach(eachVar => {
                    let productSizeItem = document.createElement('div');
                    productSizeItem.setAttribute('class', "product-size-item");
                    productSizeItem.textContent = eachVar.Size;
                    sizeContent += productSizeItem.outerHTML;
                });

                // Hiển thị giá theo kích thước
                let displayPriceBySize;
                if (minPrice == maxPrice) {
                    displayPriceBySize = `<div class="product-price">${minPrice}</div>`;
                } else {
                    displayPriceBySize = `<div class="product-price">${minPrice} - ${maxPrice}</div>`;
                }

                // Tạo nội dung HTML cho sản phẩm
                productContainer.innerHTML += `<div class="product-image-container">
                        <img class="product-image" src="${i.ImageUrl}">
                    </div>
                    <div class="product-detail-container">
                        <div class="product-brand-name">${i.Brand.Name}</div>
                        <div class="product-name">${i.Name}</div>
                        <div class="product-price-label label">Giá</div>`+
                    displayPriceBySize
                    + `<div class="product-origin-container">
                            <div class="product-origin-label label">Xuất xứ:</div>
                            <div class="product-origin-detail-container">
                                <img class="product-origin-flag" src="https://flagsapi.com/${i.Origin.CountryCode}/shiny/64.png">
                                <div class="product-origin detail-font">${i.Origin.Name} </div>
                            </div>
                        </div>

                        <div class="product-size-container">
                            <div class="product-size-label label">Kích thước (ml)</div>
                            <div class="product-size-item-container">
                                ${sizeContent}
                            </div>
                        </div>
                    </div>
                    <a type="submit" class="product-detail-btn" href="/Product/ProductDetail/${i.Id}">CHI TIẾT</a>
                </div>`;

                // Thêm sản phẩm vào fragment
                productFragment.append(productContainer);
            });

            // Xóa nội dung cũ và thêm danh sách sản phẩm mới
            productsHolder.innerHTML = '';
            productsHolder.append(productFragment);
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi nếu yêu cầu không thành công
            console.error('Lỗi khi lấy dữ liệu:', error);
            alert(xhr.responseText);
        }
    });
}

function CreatePagination() {
    // Hàm tạo phân trang (chưa có nội dung)
}
