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
                        + '<td class="text-center">' + item.date + '</td>'
                        + '<td class="text-center">' + item.docId + '</td>'
                        + '<td>' + item.account + '</td>'
                        + '<td>' + item.dyeingAc + '</td>'
                        + '<td class="text-center">' + item.lotNumber + '</td>'
                        + '<td class="text-center"><a href="/' + c + '/Edit?Id=' + item.docId + '" class="btn btn-sm btn-warning shadow"><i class="fas fa-edit"></i> Edit</a> | '
                        + '<button class="btnRemove btn btn-sm btn-danger shadow"><i class="fas fa-trash"></i> Remove</button> </td>'
                        + '</tr>';
                    tBody.append(row);
                })
            }
            $('#myTable').DataTable({
                columns: [
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "16.66%" },
                    { "width": "16.66%" },
                    { "width": "8.33%" },
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
                $('#DocId').val(data[0].docId);
                $('#TransactionDate').val(data[0].date);
                newOption($('#AccountId'), data[0].account, data[0].accountId);
                newOption($('#DyeingAcId'), data[0].dyeingAc, data[0].dyeingAcId);
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td hidden>' + item.productDetailId + '</td>'
                        + '<td>' + item.product + '</td>'
                        + '<td>' + item.lotNumber + '</td>'
                        + '<td class="text-right">' + item.qty + '</td>'
                        + '<td class="text-right">' + item.rate + '</td>'
                        + '<td class="text-right">' + item.amount + '</td>'
                        + '<td class="text-center"><a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
                        + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
                        + '</td>'
                        + '<td hidden>' + item.initLotNumber + '</td>'
                        + '</tr>';
                    tBody.append(row);
                })
                CalcTotal();
                $('#Payment').val(data[0].payment);
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
    if ($('#ProductId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Select a product',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#Barcode').focus();
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
    if (isNaN($('#Amount').val()) || parseFloat($('#Amount').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Amount cannot be zero',
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
    var pData = $('#ProductId').select2('data')[0];
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#ProductId').val());
        row.find('td:eq(2)').text((pData.name || pData.text));
        row.find('td:eq(3)').text($('#LotNumber').val());
        row.find('td:eq(4)').text($('#Qty').val());
        row.find('td:eq(5)').text($('#Rate').val());
        row.find('td:eq(6)').text($('#Amount').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#ProductId').val() + '</td>'
            + '<td>' + (pData.name || pData.text) + '</td>'
            + '<td>' + $('#LotNumber').val() + '</td>'
            + '<td class="text-right">' + $('#Qty').val() + '</td>'
            + '<td class="text-right">' + $('#Rate').val() + '</td>'
            + '<td class="text-right">' + $('#Amount').val() + '</td>'
            + '<td class="text-center"><a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
            + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
            + '</td>'
            + '</tr>';
        tBody.append(row);
    }
    CalcTotal();
    Reset();
    var d = $('.tableFixed');
    d.scrollTop(d.prop("scrollHeight"));
}
function UR(row) {
    rIndex = row.index();
    newOption($('#ProductId'), row.find('td:eq(2)').text(), parseInt(row.find('td:eq(1)').text()));
    $('#LotNumber').val(row.find('td:eq(3)').text());
    $('#Qty').val(parseFloat(row.find('td:eq(4)').text()));
    $('#Rate').val(parseFloat(row.find('td:eq(5)').text()));
    $('#Amount').val(parseFloat(row.find('td:eq(6)').text()));
}
function RR(row) {
    row.remove();
    CalcTotal();
}
//Rows

//Reset
function Calc() {
    var amount = parseFloat($('#Amount').val()), advance = parseFloat($('#Advance').val()), balance = 0;
    balance = amount - advance;
    $('#Balance').val(balance);
}
function Reset() {
    $('#ProductId').val(null).trigger('change');
    $('#LotNumber').val('');
    $('#Qty').val(0);
    $('#Rate').val(0);
    $('#Amount').val(0);
    $('#ProductId').select2('open');
}
//Reset

//Add
function UI(fd) {
    return $.ajax({
        type: 'Post',
        url: '/Purchase/UploadImage',
        data: fd,
        contentType: false,
        processData: false,
        success: function (data) {
        },
        error: function (resp) {
            $('.loader').hide();
            inProcess = false;
            DisplayError(resp.responseText);
        }
    })
}
function S(c, a, print) {
    $('.loader').show();
    var data = {
        DocId: parseInt($('#DocId').val()),
        TransactionDate: $('#TransactionDate').val(),
        Name: $('#SupplierName').val(),
        Contact: $('#Contact').val(),
        CNIC: $('#CNIC').val(),
        PropertyName: $('#PropertyName').val(),
        Description: $('#Description').val(),
        Amount: parseFloat($('#Amount').val()),
        Advance: parseFloat($('#Advance').val()),
        Balance: parseFloat($('#Balance').val()),
        DueDate: $('#DueDate').val(),
        Print: print
    }
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: { vm: data },
        success: function () {
            var fd = new FormData();
            fd.append('PropertyName', $('#PropertyName').val());
            $.each($('#tBody tr'), function () {
                var row = $(this);
                var files = row.find('.custom-file-input')[0].files;
                var file = files[0];
                fd.append('ImageFile', file);
            });
            $.when(UI(fd)).done(function () {
                if (a == 'Edit') {
                    window.location.href = '/' + c + '/Index';
                }
                else {
                    window.location.reload();
                }
            })
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

//blur eve
$('#Amount').on('blur', function () {
    Calc();
})
$('#Advance').on('blur', function () {
    Calc();
})
$('#tBody').on('blur', '.txtQty', function (e) {
    e.preventDefault();
    var row = $(this).closest('tr');
    CalcRow(row);
})
//blur eve

//Click Eve
$('#btnAdd').click(function (e) {
    e.preventDefault();
    if (RV()) {
        AR($('#tBody'));
    }
})
$('#btnReset').click(function (e) {
    e.preventDefault();
    Reset();
})
$('#btnAddMore').click(function (e) {
    e.preventDefault();
    var row = '<tr>'
        + '<td>'
        + '<div class="custom-file">'
        + '<input type="file" class="custom-file-input" name="filename" />'
        + '<label class="custom-file-label" for="customFile">Choose file</label>'
        + '</div>'
        + '</td>'
        + '<td class="text-center"></td>'
        + '</tr>';
    $('#tBody').append(row);
})
$('#tBody').on('click', '.btnUR', function (e) {
    e.preventDefault();
    var row = $(this).closest('tr');
    UR(row);
})
$('#tBody').on('click', '.btnRR', function (e) {
    e.preventDefault();
    var row = $(this).closest('tr');
    RR(row);
})
//Click Eve

//focus eve
$('#Amount').on('focus', function () {
    $(this).select();
})
$('#Advance').on('focus', function () {
    $(this).select();
})
//focus eve

//keydown eve
$('#Contact').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        var param = $(this).val();
        if (param != null || param != '') {
            $.when(GetAccount(param, 'Contact')).done(function () {
                if (acc != null) {
                    $('#CNIC').val(acc.cnic);
                    $('#SupplierName').val(acc.name);
                    $('#PropertyName').focus();
                }
                else {
                    $('#CNIC').focus();
                }
            })
        }
    }
})
$('#CNIC').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        var param = $(this).val();
        if (param != null || param != '') {
            $.when(GetAccount(param, 'CNIC')).done(function () {
                if (acc != null) {
                    $('#Contact').val(acc.contact);
                    $('#Name').val(acc.name);
                    $('#PropertyName').focus();
                }
                else {
                    $('#SupplierName').focus();
                }
            })
        }
    }
})
$('#SupplierName').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#PropertyName').focus();
    }
})
$('#PropertyName').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        var param = $(this).val();
        if (param != null || param != '') {
            $.when(GetProperty(param)).done(function () {
                if (property != null) {
                    $('#Description').val(property.description);
                    if (property.images.length > 0) {
                        $('#tBody').html('');
                        $.each(property.images, function (i) {
                            var item = property.images[i];
                            var row = '<tr>'
                                + '<td>'
                                + '<div class="custom-file">'
                                + '<input type="file" class="custom-file-input" name="filename" />'
                                + '<label class="custom-file-label" for="customFile">Choose file</label>'
                                + '</div>'
                                + '</td>'
                                + '<td class="text-center"><a target="_blank" href="https://property.lyallpurtextiles.pk/images/' + property.name + '/' + item + '"><i class="fa fa-eye"></i></a></td>'
                                + '</tr>';
                            $('#tBody').append(row);
                        })
                    }
                    $('#Amount').focus();
                }
                else {
                    $('#Description').focus();
                }
            })
        }
    }
})
$('#Description').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Amount').focus();
    }
})
$('#Amount').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Advance').focus();
    }
})
$('#Advance').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#DueDate').focus();
    }
})
$('#DueDate').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnSave').focus();
    }
})
//keydown eve

//file input
$('#tBody').on('change', '.custom-file-input', function () {
    var row = $(this).closest('tr');
    var fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

//select2 Eve
$('#AccountId').on('select2:close', function () {
    $('#DyeingAcId').select2('open');
})
$('#ProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#LotNumber').focus();
    }
})
//select2 Eve