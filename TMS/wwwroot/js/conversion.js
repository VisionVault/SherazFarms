var rIndex = -1;
var vType = null;

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
                        + '<td>' + item.rawPro + '</td>'
                        + '<td class="text-center">' + item.rawQty + '</td>'
                        + '<td>' + item.readyPro + '</td>'
                        + '<td class="text-center">' + item.readyQty + '</td>'
                        + '<td class="text-center">'
                        + '<a href="/' + c + '/Edit?Id=' + item.docId + '"><i class="fas fa-edit"></i></a> | '
                        + '<a href="#" class="btnRemove"><i class="fas fa-trash"></i></a>'
                        + '</td>'
                        + '</tr>';
                    tBody.append(row);
                })
            }
            $('#myTable').DataTable({
                columns: [
                    { "width": "4.15%" },
                    { "width": "16.66%" },
                    { "width": "4.15%" },
                    { "width": "25%" },
                    { "width": "8.33%" },
                    { "width": "25%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
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
        data: {id: id},
        success: function (data) {
            if (data.length > 0) {
                var sr = 0;
                $('#DocId').val(data[0].docId);
                $('#TransactionDate').val(data[0].date);
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td hidden>' + item.rawProductId + '</td>'
                        + '<td>' + item.rawPro + '</td>'
                        + '<td class="text-right">' + item.rawQty + '</td>'
                        + '<td class="text-right">' + item.rawRate + '</td>'
                        + '<td class="text-right">' + item.rawAmount + '</td>'
                        + '<td hidden>' + item.readyProductId + '</td>'
                        + '<td>' + item.readyPro + '</td>'
                        + '<td class="text-right">' + item.readyQty + '</td>'
                        + '<td class="text-right">' + item.readyRate + '</td>'
                        + '<td class="text-right">' + item.readyAmount + '</td>'
                        + '<td class="text-center">'
                        + '<a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
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
    if ($('#RawProductId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Select a raw product',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#RawProductId').select2('open');
            }
        });
        v = false;
        return v;
    }
    if (isNaN($('#RawQty').val()) || parseInt($('#RawQty').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Please enter raw qty',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#RawQty').focus();
            }
        });
        v = false;
        return v;
    }
    if (isNaN($('#RawRate').val()) || parseFloat($('#RawRate').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Raw Rate cannot be zero',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#RawRate').focus();
            }
        });
    }
    if ($('#ReadyProductId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Select a ready product',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#ReadyProductId').select2('open');
            }
        });
        v = false;
        return v;
    }
    if (isNaN($('#ReadyQty').val()) || parseInt($('#ReadyQty').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Please enter ready qty',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#ReadyQty').focus();
            }
        });
        v = false;
        return v;
    }
    if (isNaN($('#ReadyRate').val()) || parseFloat($('#ReadyRate').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Ready rate cannot be zero',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#ReadyRate').focus();
            }
        });
        v = false;
        return v;
    }
    return v;
}
function AR(tBody) {
    var rawSelect = $('#RawProductId').select2('data');
    var readySelect = $('#ReadyProductId').select2('data');
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#RawProductId').val());
        row.find('td:eq(2)').text((rawSelect[0].name || rawSelect[0].text));
        row.find('td:eq(3)').text($('#RawQty').val());
        row.find('td:eq(4)').text($('#RawRate').val());
        row.find('td:eq(5)').text($('#RawAmount').val());
        row.find('td:eq(6)').text($('#ReadyProductId').val());
        row.find('td:eq(7)').text((readySelect[0].name || readySelect[0].text));
        row.find('td:eq(8)').text($('#ReadyQty').val());
        row.find('td:eq(9)').text($('#ReadyRate').val());
        row.find('td:eq(10)').text($('#ReadyAmount').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#RawProductId').val() + '</td>'
            + '<td>' + (rawSelect[0].name || rawSelect[0].text) + '</td>'
            + '<td class="text-right">' + $('#RawQty').val() + '</td>'
            + '<td class="text-right">' + $('#RawRate').val() + '</td>'
            + '<td class="text-right">' + $('#RawAmount').val() + '</td>'
            + '<td hidden>' + $('#ReadyProductId').val() + '</td>'
            + '<td>' + (readySelect[0].name || readySelect[0].text) + '</td>'
            + '<td class="text-right">' + $('#ReadyQty').val() + '</td>'
            + '<td class="text-right">' + $('#ReadyRate').val() + '</td>'
            + '<td class="text-right">' + $('#ReadyAmount').val() + '</td>'
            + '<td class="text-center">'
            + '<a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
            + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
            + '</td>'
            + '</tr>';
        tBody.append(row);
    }
    //CalcTotal();
    Reset();
    var d = $('.tableFixed');
    d.scrollTop(d.prop("scrollHeight"));
}
function UR(row) {
    newOption($('#RawProductId'), row.find('td:eq(2)').text(), parseInt(row.find('td:eq(1)').text()));
    $('#RawQty').val(parseFloat(row.find('td:eq(3)').text()));
    $('#RawRate').val(parseFloat(row.find('td:eq(4)').text()));
    $('#RawAmount').val(parseFloat(row.find('td:eq(5)').text()));
    newOption($('#ReadyProductId'), row.find('td:eq(7)').text(), parseInt(row.find('td:eq(6)').text()));
    $('#ReadyQty').val(parseFloat(row.find('td:eq(8)').text()));
    $('#ReadyRate').val(parseFloat(row.find('td:eq(9)').text()));
    $('#ReadyAmount').val(parseFloat(row.find('td:eq(10)').text()));
    rIndex = row.index();
}
function RR(row) {
    row.remove();
    CalcTotal();
}
//Rows

//Reset
function CalcRawAmount() {
    var qty = parseFloat($('#RawQty').val()), rate = parseFloat($('#RawRate').val()), amount = (qty * rate).toFixed(0);
    $('#RawAmount').val(amount);
}
function CalcReadyAmount() {
    var qty = parseFloat($('#ReadyQty').val()), rate = parseFloat($('#ReadyRate').val()), amount = (qty * rate).toFixed(0);
    $('#ReadyAmount').val(amount);
}
function Reset() {
    $('#RawProductId').val(null).trigger('change');
    $('#RawStock').val(0);
    $('#RawQty').val(0);
    $('#RawRate').val(0);
    $('#RawAmount').val(0);
    $('#ReadyProductId').val(null).trigger('change');
    $('#ReadyStock').val(0);
    $('#ReadyQty').val(0);
    $('#ReadyRate').val(0);
    $('#ReadyAmount').val(0);
    $('#RawProductId').select2('open');
}
//Reset

//Add
function S(c, a, print) {
    $('.loader').show();
    var data = [];
    $.each($('#tBody tr'), function () {
        var row = $(this);
        var d = {};
        d = {
            DocId: parseInt($('#DocId').val()),
            TransactionDate: $('#TransactionDate').val(),
            RawProductId: parseInt(row.find('td:eq(1)').text()),
            RawQty: parseFloat(row.find('td:eq(3)').text()),
            RawRate: parseFloat(row.find('td:eq(4)').text()),
            RawAmount: parseFloat(row.find('td:eq(5)').text()),
            ReadyProductId: parseInt(row.find('td:eq(6)').text()),
            ReadyQty: parseFloat(row.find('td:eq(8)').text()),
            ReadyRate: parseFloat(row.find('td:eq(9)').text()),
            ReadyAmount: parseFloat(row.find('td:eq(10)').text()),
        }
        data.push(d);
    })
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: {vm: data},
        success: function (data) {
            $('.loader').hide();
            if (print) {
                if (data != null) {
                    var w = window.open('/Reports/Invoice?d=' + data.d + '&t=' + data.t);
                }
            }
            if (a == 'Edit') {
                window.location.href = '/' + c + '/Index';
            }
            else {
                window.location.reload();
            }
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}
//Add

//Print
function Print(d, t) {
    var w = window.open('/Reports/Invoice?d=' + d + '&t=' + t);
}
//Print

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
$('#RawQty').on('blur', function () {
    CalcRawAmount();
})
$('#RawRate').on('blur', function () {
    CalcRawAmount();
})
$('#ReadyQty').on('blur', function () {
    CalcReadyAmount();
})
$('#ReadyRate').on('blur', function () {
    CalcReadyAmount();
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
$('#RawQty').on('focus', function () {
    $(this).select();
})
$('#RawRate').on('focus', function () {
    $(this).select();
})
$('#ReadyQty').on('focus', function () {
    $(this).select();
})
$('#ReadyRate').on('focus', function () {
    $(this).select();
})
//focus eve

//input eve
$('#RawQty').on('input', function () {
    CN($(this));
})
$('#RawRate').on('input', function () {
    CN($(this));
})
$('#ReadyQty').on('input', function () {
    CN($(this));
})
$('#ReadyRate').on('input', function () {
    CN($(this));
})
//input eve

//keydown eve
$('body').on('keydown', function (e) {
    if (e.which == 119) {
        e.preventDefault();
        $('#ProductId').select2('close');
        $('#TrackingNumber').focus();
    }
})
$('#RawQty').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#RawRate').focus();
    }    
})
$('#RawRate').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#ReadyProductId').select2('open');
    }
})
$('#ReadyQty').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#ReadyRate').focus();
    }
})
$('#ReadyRate').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})
//keydown eve

//select2 Eve
$('#RawProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#RawQty').focus();
    }
})
$('#ReadyProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#ReadyQty').focus();
    }
})
//select2 Eve