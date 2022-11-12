var rIndex = -1;
var inProcess = false;

//get
function LD(tBody, c, a, fromDate, toDate) {
    $('.loader').show();
    $('#myTable').DataTable().clear();
    $('#myTable').DataTable().destroy();
    var fd = new FormData();
    fd.append('FromDate', fromDate);
    fd.append('ToDate', toDate);
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: fd,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.length > 0) {
                var sr = 0;
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td hidden>' + item.productId + '</td>'
                        + '<td>' + item.product + '</td>'
                        + '<td class="text-center"><a href="/' + c + '/Edit?Id=' + item.productId + '" class="btn btn-sm btn-warning shadow"><i class="fas fa-edit"></i> Edit</a> | '
                        //+ '<button class="btnRemove btn btn-sm btn-danger shadow"><i class="fas fa-trash"></i> Remove</button> </td>'
                        + '</tr>';
                    tBody.append(row);
                })
            }
            $('#myTable').DataTable({
                columns: [
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "75%" },
                    { "width": "16.66%" },
                ]
            });
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}
function Rec(tBody, c, a, id) {
    $('.loader').show();
    tBody.html = '';
    $.ajax({
        type: 'Get',
        url: '/' + c + '/' + a,
        data: { id: id },
        success: function (data) {
            if (data.length > 0) {
                var sr = 0;
                newOption($('#ProductId'), data[0].product, data[0].productId);
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td hidden>' + item.rawMaterialId + '</td>'
                        + '<td>' + item.rawMaterial + '</td>'
                        + '<td class="text-right">' + item.qty + '</td>'
                        + '<td class="text-center">'
                        + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
                        + '</td>'
                        + '</tr>';
                    tBody.append(row);
                })
            }
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}
//get

//Rows
function RV() {
    var v = true;
    if ($('#RawMaterialId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Select a raw material',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#RawMaterialId').select2('open');
            }
        });
        v = false;
        return v;
    }
    if (isNaN($('#Qty').val()) || parseInt($('#Qty').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Please enter qty',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#Qty').focus();
            }
        });
        v = false;
        return v;
    }
    return v;
}
function AR(tBody) {
    var pData = $('#RawMaterialId').select2('data')[0];
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#RawMaterialId').val());
        row.find('td:eq(2)').text((pData.name || pData.text));
        row.find('td:eq(3)').text($('#Qty').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#RawMaterialId').val() + '</td>'
            + '<td>' + (pData.name || pData.text) + '</td>'
            + '<td class="text-right">' + $('#Qty').val() + '</td>'
            + '<td class="text-center">'
            + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
            + '</td>'
            + '</tr>';
        tBody.append(row);
    }
    Reset();
    var d = $('.tableFixed');
    d.scrollTop(d.prop("scrollHeight"));
}
function UR(row) {
    rIndex = row.index();
    newOption($('#RawMaterialId'), row.find('td:eq(2)').text(), parseInt(row.find('td:eq(1)').text()));
    $('#Qty').val(parseFloat(row.find('td:eq(3)').text()));
}
function RR(row) {
    row.remove();
}
//Rows

//Reset
function Calc() {
    var amount = parseFloat($('#Amount').val()), advance = parseFloat($('#Advance').val()), balance = 0;
    balance = amount - advance;
    $('#Balance').val(balance);
}
function Reset() {
    $('#RawMaterialId').val(null).trigger('change');
    $('#Qty').val(0);
    $('#RawMaterialId').select2('open');
}
//Reset

//Add
function S(c, a, print) {
    $('.loader').show();
    var data = [];
    $.each($('#tBody tr'), function () {
        var row = $(this);
        var d = {
            ProductId: $('#ProductId').val(),
            RawMaterialId: parseInt(row.find('td:eq(1)').text()),
            Qty: parseFloat(row.find('td:eq(3)').text()),
        }
        data.push(d);
    })
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: { vm: data },
        success: function () {
            window.location.href = '/' + c + '/Index';
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}
//Add

//Rem
function Rem(c, id) {
    $.alert({
        title: 'Question',
        content: 'Do you want to remove this voucher?',
        type: 'yellow',
        typeAnimated: true,
        buttons: {
            YES: {
                text: 'YES',
                btnClass: 'btn-green',
                action: function () {
                    $('.loader').show();
                    $.ajax({
                        type: 'Get',
                        url: '/' + c + '/Remove',
                        data: { id: id },
                        success: function (data) {
                            window.location.reload();
                        },
                        error: function (resp) {
                            $('.loader').hide();
                            DisplayError(resp.responseText);
                        }
                    })
                }
            },
            NO: {
                text: 'NO',
                btnClass: 'btn-red',
                action: function () {
                    return;
                }
            }
        }
    })
}
//Rem

//Click Eve
$('#btnAdd').click(function (e) {
    e.preventDefault();
    if (RV()) {
        AR($('#tBody'));
    }
})
$('#tBody').on('click', '.btnRR', function (e) {
    e.preventDefault();
    var row = $(this).closest('tr');
    RR(row);
})
//Click Eve

//focus eve
$('#Qty').on('focus', function () {
    $(this).select();
})
//focus eve

//input eve
$('#Qty').on('input', function () {
    CN($(this));
})
//input eve

//keydown eve
$('#Qty').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})
//keydown eve

//select2 Eve
$('#ProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#RawMaterialId').select2('open');
    }
})
$('#RawMaterialId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#Qty').focus();
    }
})
//select2 Eve