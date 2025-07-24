var page = 1;

const filter = {
    keyword: undefined,
    brand: undefined,
    price: {
        from: undefined,
        to: undefined
    },
    cap: {
        from: undefined,
        to: undefined
    },
    origin: undefined
}


const pagination = {
    page: 1,
    size: 8,
    pagingNum: undefined,
    result: 0,
    pagingHolder: undefined,
    txtResultNum: undefined,
    inputPagingNum: undefined
}
const urlParams = new URLSearchParams(window.location.search);
document.addEventListener('DOMContentLoaded', function () {
    HandlePaging();
    HandleFilter();
    GetFilterDetail();
    GetProductList();



    document.getElementById('btn-filter').addEventListener('click', function (event) {
        pagination.page = 1;
        GetProductList(size = pagination.size, page = pagination.page);
    });

    document.getElementById('btn-reset').addEventListener('click', function (event) {
        pagination.page = 1;
        filter.keyword.value = null;
        filter.brand.value = 'All';
        filter.origin.value = 'All';
        filter.price.from.value = null;
        filter.price.to.value = null;
        filter.cap.from.value = null;
        filter.cap.to.value = null;
        GetProductList(size = pagination.size, page = pagination.page);
    });

    document.getElementById('btn-paging').addEventListener('click', function (event) {
        const input = Number(pagination.inputPagingNum.value);

        if (input > 0 && input <= pagination.pagingNum) {
            pagination.page = input;
            GetProductList(size = pagination.size, page = pagination.page);
        }
    });
});


function HandleFilter() {
    filter.keyword = document.getElementById('input-filter-keyword');
    filter.brand = document.getElementById('brands');
    filter.origin = document.getElementById('origins');
    filter.price.from = document.getElementById('input-filter-from-price');
    filter.price.to = document.getElementById('input-filter-to-price');
    filter.cap.from = document.getElementById('input-filter-from-volume');
    filter.cap.to = document.getElementById('input-filter-to-volume');
}

function HandlePaging() {
    pagination.page = 1,
        pagination.size = 8
    pagination.pagingHolder = document.getElementById('pagination-ul');
    pagination.txtResultNum = document.getElementById('txt-result-num');
    pagination.inputPagingNum = document.getElementById('input-paging');
}

function CreatePagingComponent(size = 7, page = 1) {

    pagination.txtResultNum.textContent = `Tìm được ${pagination.result} giá trị. Bao gồm ${pagination.pagingNum} trang.`;
  
    if (pagination.pagingNum > 1) {
        console.log(pagination.pagingNum);
        const pagingFragment = document.createDocumentFragment();

        const noChangeNum = Math.floor(size / 2);
        const topChangeNum = noChangeNum + 1;
        const botChangeNum = Number(pagination.pagingNum) - noChangeNum;
        var end;
        if (pagination.pagingNum >= size) {
            var start = page;
            if (start <= topChangeNum) {
                start = 1;
            } else if (start >= botChangeNum) {
                start = Number(pagination.pagingNum) - size + 1;
            } else {
                start = page - noChangeNum;
                end = page - noChangeNum;
            }

            end = start + size - 1;
        } else {
            start = 1;
            end = pagination.pagingNum;
        }

        for (let i = start; i <= end; i++) {
            const li = document.createElement('li');
            if (i == page) {
                li.setAttribute('class', 'page-item active');
            } else {
                li.setAttribute('class', 'page-item');
            }
            const a = document.createElement('a');
            a.dataset.page = i;
            a.setAttribute('class', 'page-link');
            a.text = i;

            a.addEventListener('click', function (event) {

                pagination.page = Number(this.dataset.page);
                GetProductList(size = pagination.size, page = pagination.page);
            });
            li.append(a);

            pagingFragment.append(li);
        }

        if (pagination.pagingNum > size) {
            const liStart = document.createElement('li');
            liStart.setAttribute('class', 'page-item');
            const aStart = document.createElement('a');
            aStart.dataset.page = 1;
            aStart.setAttribute('class', 'page-link');
            aStart.text = '<<';
            aStart.addEventListener('click', function (event) {

                pagination.page = Number(this.dataset.page);
                this.parentElement.setAttribute('class', 'page-item disabled');
                GetProductList(size = pagination.size, page = pagination.page);
            });
            

            const liEnd = document.createElement('li');
            liEnd.setAttribute('class', 'page-item');
            const aEnd = document.createElement('a');
            aEnd.dataset.page = pagination.pagingNum;
            aEnd.setAttribute('class', 'page-link');
            aEnd.text = '>>';
            aEnd.addEventListener('click', function (event) {

                pagination.page = Number(this.dataset.page);
                this.parentElement.setAttribute('class', 'page-item disabled');
                GetProductList(size = pagination.size, page = pagination.page);
            });

            liStart.append(aStart);
            liEnd.append(aEnd);

            pagingFragment.append(liStart);
            pagingFragment.append(liEnd);
        }


        pagination.pagingHolder.innerHTML = ``;
        pagination.pagingHolder.append(pagingFragment);
    } else {
        pagination.pagingHolder.innerHTML = ``;
    }
}

function UpdatePaging() {

}

function GetFilterDetail() {
    $.ajax({
        url: '/Product/GetFilterDetail',
        type: 'GET',
        datatype: 'json',
        success: function (data) {

            const origins = data.origins
            const brands = data.brands


            //origin
            const originFragment = document.createDocumentFragment();
            var originsHolder = document.getElementById('origins');
            var originFirstElem = document.createElement("option");
            originFirstElem.value = "All";
            originFirstElem.text = "Tất cả";
            originFirstElem.selected = true;
            originFragment.append(originFirstElem);

            origins.forEach(i => {
                const opt = document.createElement('option');
                opt.value = i.CountryCode;
                opt.text = i.Name;
                originFragment.appendChild(opt);
            });
            originsHolder.append(originFragment);

            //brand
            const brandFragment = document.createDocumentFragment();
            var brandsHolder = document.getElementById('brands');
            var brandFirstElem = document.createElement("option");
            brandFirstElem.value = "All";
            brandFirstElem.text = "Tất cả";
            brandFirstElem.selected = true;
            brandFragment.append(brandFirstElem);

            brands.forEach(i => {
                const opt = document.createElement('option');
                opt.value = i.Name;
                opt.text = i.Name;
                brandFragment.appendChild(opt);
            });
            brandsHolder.append(brandFragment);

        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
            alert(xhr.responseText);
        }
    });
}
function GetProductList(size = 8, page = 1) {
    var productsHolder = document.getElementById('product-list-container');


    $.ajax({
        url: '/Product/GetProductList',
        type: 'GET',
        data: {
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

            //Lưu trữ những thông tin cần thiết lại
            pagination.page = Number(page);
            pagination.size = Number(size);
            pagination.pagingNum = Number(data.pagingNum);
            pagination.result = Number(data.resultNum);

            pagination.inputPagingNum.setAttribute('max', pagination.pagingNum);

            CreatePagingComponent(size = 7, page = page);
            const products = data.products



            const productFragment = document.createDocumentFragment();

            products.forEach(i => {
                let productContainer = document.createElement('div');
                productContainer.setAttribute('class', 'product-container');
                if (new Date(i.ReleaseYear).getFullYear() >= new Date().getFullYear()) {
                    productContainer.innerHTML = `<div class="product-label">MỚI</div>`;
                }

                let minPrice = Math.min(...i.ProductSizePricings.map(function (p) { return p.Price }));
                let maxPrice = Math.max(...i.ProductSizePricings.map(function (p) { return p.Price }));

                let sizeContent = '';
                let productSizePricing = i.ProductSizePricings.sort((a, b) => a.Size - b.Size);
                productSizePricing.forEach(eachVar => {
                    let productSizeItem = document.createElement('div');


                    productSizeItem.setAttribute('class', "product-size-item");
                    productSizeItem.textContent = eachVar.Size;
                    sizeContent += productSizeItem.outerHTML;
                });

                let displayPriceBySize;
                if (minPrice == maxPrice) {
                    displayPriceBySize = `<div class="product-price">${minPrice}</div>`;
                } else {
                    displayPriceBySize = `<div class="product-price">${minPrice} - ${maxPrice}</div>`;
                }

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


                productFragment.append(productContainer);
            });

            productsHolder.innerHTML = '';
            productsHolder.append(productFragment);

        },
        error: function (xhr, status, error) {
            console.error('Lỗi khi lấy dữ liệu:', error);
            alert(xhr.responseText);
        }
    });
}

function CreatePagination() {

}