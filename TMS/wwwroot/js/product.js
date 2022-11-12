function Rec(c, a, id) {
    $('.loader').show();
    $.ajax({
        type: 'Get',
        url: '/' + c + '/' + a,
        data: { id: id },
        success: function (data) {
            if (data != null) {
                $('#Id').val(data.id);
                $('#InitialName').val(data.name);
                newOption($('#CategoryId'), data.category, data.productCategoryId);
                newOption($('#BrandId'), data.brand, data.brandId);
                $('#Name').val(data.name);
                $('#CostPrice').val(data.costPrice);
                $('#SalePrice').val(data.salePrice);
                $('#IsActive').prop('checked', data.isActive);
            }
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}


function Calc() {
    var cost = parseFloat($('#CostPrice').val()), qty = parseFloat($('#OpeningStock').val()), amount = (cost * qty).toFixed(2);
    $('#OpeningValue').val(amount);
}

function S(c, a) {
    var d = {
        Id: $('#Id').val(),
        InitialName: $('#InitialName').val(),
        ProductCategoryId: $('#CategoryId').val(),
        BrandId: $('#BrandId').val(),
        Name: $('#Name').val(),
        CostPrice: $('#CostPrice').val(),
        SalePrice: $('#SalePrice').val(),
        OpeningStock: 0,
        OpeningValue: 0,
        IsActive: $('input[type="checkbox"]').is(':checked'),
    }
    $('.loader').show();
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: { vm: d },
        success: function () {
            $('.loader').hide();
            DisplayAddOrList('Product');
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}

//focus
$('#CostPrice').on('focus', function () {
    $(this).select();
})
$('#SalePrice').on('focus', function () {
    $(this).select();
})

//input
$('#CostPrice').on('input', function () {
    CN($(this));
})
$('#SalePrice').on('input', function () {
    CN($(this));
})

//keydown
$('#Name').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#CostPrice').focus();
    }
})
$('#CostPrice').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#SalePrice').focus();
    }
})
$('#SalePrice').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnSave').focus();
    }
})

//change
$('#CategoryId').on('select2:close', function (e) {
    $('#BrandId').select2('open');
})
$('#BrandId').on('select2:close', function (e) {
    $('#Name').focus();
})