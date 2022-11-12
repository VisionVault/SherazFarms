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
                        + '<td class="text-center">' + item.outQty + '</td>'
                        + '<td class="text-center">' + item.inQty + '</td>'
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
                    { "width": "8.33%" },
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
                        + '<td hidden>' + item.outProductId + '</td>'
                        + '<td>' + item.outProduct + '</td>'
                        + '<td class="text-right">' + item.outQty + '</td>'
                        + '<td hidden>' + item.inProductId + '</td>'
                        + '<td>' + item.inProduct + '</td>'
                        + '<td class="text-right">' + item.inQty + '</td>'
                        + '<td class="text-center">'
                        + '<a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
                        + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
                        + '</td>'
                        + '</tr>';
                    tBody.append(row);
                })
                CalcTotal();
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
    return v;
}
function AR(tBody) {
    var outSelect = $('#OutProductId').select2('data');
    var inSelect = $('#InProductId').select2('data');
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#OutProductId').val());
        row.find('td:eq(2)').text((outSelect[0].name || outSelect[0].text));
        row.find('td:eq(3)').text($('#OutQty').val());
        row.find('td:eq(4)').text($('#InProductId').val());
        row.find('td:eq(5)').text((inSelect[0].name || inSelect[0].text));
        row.find('td:eq(6)').text($('#InQty').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#OutProductId').val() + '</td>'
            + '<td>' + (outSelect[0].name || outSelect[0].text) + '</td>'
            + '<td class="text-right">' + $('#OutQty').val() + '</td>'
            + '<td hidden>' + $('#InProductId').val() + '</td>'
            + '<td>' + (inSelect[0].name || inSelect[0].text) + '</td>'
            + '<td class="text-right">' + $('#InQty').val() + '</td>'
            + '<td class="text-center">'
            + '<a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
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
    newOption($('#OutProductId'), row.find('td:eq(2)').text(), parseInt(row.find('td:eq(1)').text()));
    $('#OutQty').val(parseFloat(row.find('td:eq(3)').text()));
    newOption($('#InProductId'), row.find('td:eq(5)').text(), parseInt(row.find('td:eq(4)').text()));
    $('#InQty').val(parseFloat(row.find('td:eq(6)').text()));
    rIndex = row.index();
}
function RR(row) {
    row.remove();
    CalcTotal();
}
//Rows

//Reset
function CalcAmount() {
    var qty = parseFloat($('#Qty').val()), rate = parseFloat($('#Rate').val()), amount = (qty * rate).toFixed(0);
    $('#Amount').val(amount);
}
function CalcTotal() {
    //$('#tFoot tr:first').find('th:eq(1)').text('');
    //$('#tFoot tr:first').find('th:eq(3)').text('');
    //var qty = 0, amount = 0;
    //$.each($('#tBody tr'), function () {
    //    var r = $(this);
    //    qty += parseFloat(r.find('td:eq(3)').text());
    //    amount += parseFloat(r.find('td:eq(5)').text());
    //})
    //$('#tFoot tr:first').find('th:eq(1)').text(qty);
    //$('#tFoot tr:first').find('th:eq(3)').text(amount);
}
function Reset() {
    $('#OutProductId').val(null).trigger('change');
    $('#OutQty').val(0);
    $('#InProductId').val(null).trigger('change');
    $('#InQty').val(0);
    $('#OutProductId').select2('open');
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
            OutProductId: parseInt(row.find('td:eq(1)').text()),
            OutQty: parseFloat(row.find('td:eq(3)').text()),
            InProductId: parseInt(row.find('td:eq(4)').text()),
            InQty: parseFloat(row.find('td:eq(6)').text()),
            Print: print
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
$('#OutQty').on('focus', function () {
    $(this).select();
})
$('#InQty').on('focus', function () {
    $(this).select();
})
//focus eve

//input eve
$('#OutQty').on('input', function () {
    CN($(this));
})
$('#InQty').on('input', function () {
    CN($(this));
})
//input eve

//keydown eve
$('body').on('keydown', function (e) {
    if (e.which == 119) {
        e.preventDefault();
        $('#ProductId').select2('close');
        $('#Discount').focus();
    }
})
$('#OutQty').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#InProductId').select2('open');
    }    
})
$('#InQty').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})
//keydown eve

//select2 Eve
$('#OutProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#OutQty').focus();
    }
})
$('#InProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#InQty').focus();
    }
})
//select2 Eve