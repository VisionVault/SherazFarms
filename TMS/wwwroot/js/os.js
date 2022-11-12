var rIndex = -1;

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
                        + '<td>' + item.product + '</td>'
                        + '<td class="text-center">' + item.lotNumber + '</td>'
                        + '<td class="text-center">' + item.qty + '</td>'
                        + '<td class="text-center"><a href="/' + c + '/Edit?Id=' + item.docId + '" class="btn btn-sm btn-warning shadow"><i class="fas fa-edit"></i> Edit</a> | '
                        + '<button class="btnRemove btn btn-sm btn-danger shadow"><i class="fas fa-trash"></i> Remove</button> </td>'
                        + '</tr>';
                    tBody.append(row);
                })
                $('#myTable').DataTable({
                    columns: [
                        { "width": "8.33%" },
                        { "width": "16.66%" },
                        { "width": "8.33%" },
                        { "width": "16.66%" },
                        { "width": "16.66%" },
                        { "width": "8.33%" },
                        { "width": "8.33%" },
                        { "width": "25%" },
                    ]
                });
            }
            else {
                $('#myTable').DataTable();
            }
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
                if (data[0].account != null) {
                    newOption($('#AccountId'), data[0].account, data[0].accountId);
                }
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td hidden>' + item.productDetailId + '</td>'
                        + '<td>' + item.product + '</td>'
                        + '<td>' + item.lotNumber + '</td>'
                        + '<td hidden>' + item.designId + '</td>'
                        + '<td>' + item.design + '</td>'
                        + '<td class="text-right">' + item.rollNumber + '</td>'
                        + '<td class="text-right">' + item.designNumber + '</td>'
                        + '<td class="text-right">' + item.jobNumber + '</td>'
                        + '<td class="text-right">' + item.qty + '</td>'
                        + '<td class="text-center"><a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
                        + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a></td>'
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
    if ($('#ProductId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Please select Product',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#ProductId').focus();
            }
        });
        v = false;
        return v;
    }
    if ($('#LotNumber').val() == null || $('#LotNumber').val() == '') {
        $.alert({
            title: 'Error',
            content: 'Please enter lot number',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#LotNumber').focus();
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
    var selectProduct = $('#ProductId').select2('data');
    var selectDesign = $('#DesignId').select2('data');
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#ProductId').val());
        row.find('td:eq(2)').text((selectProduct[0].name || selectProduct[0].text));
        row.find('td:eq(3)').text($('#LotNumber').val());
        row.find('td:eq(4)').text($('#DesignId').val());
        row.find('td:eq(5)').text(($('#DesignId').val() == null ? '' : (selectDesign[0].name || selectDesign[0].text)));
        row.find('td:eq(6)').text($('#RollNumber').val());
        row.find('td:eq(7)').text($('#DesignNumber').val());
        row.find('td:eq(8)').text($('#JobNumber').val());
        row.find('td:eq(9)').text($('#Qty').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#ProductId').val() + '</td>'
            + '<td>' + (selectProduct[0].name || selectProduct[0].text) + '</td>'
            + '<td>' + $('#LotNumber').val() + '</td>'
            + '<td hidden>' + $('#DesignId').val() + '</td>'
            + '<td>' + ($('#DesignId').val() == null ? '' : (selectDesign[0].name || selectDesign[0].text)) + '</td>'
            + '<td class="text-right">' + $('#RollNumber').val() + '</td>'
            + '<td class="text-right">' + $('#DesignNumber').val() + '</td>'
            + '<td class="text-right">' + $('#JobNumber').val() + '</td>'
            + '<td class="text-right">' + $('#Qty').val() + '</td>'
            + '<td class="text-center"><a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
            + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a></td>'
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
    if (parseInt(row.find('td:eq(4)').text()) > 0) {
        newOption($('#DesignId'), row.find('td:eq(5)').text(), parseInt(row.find('td:eq(4)').text()));
    }
    $('#RollNumber').val(parseInt(row.find('td:eq(6)').text()));
    $('#DesignNumber').val(parseInt(row.find('td:eq(7)').text()));
    $('#JobNumber').val(parseInt(row.find('td:eq(8)').text()));
    $('#Qty').val(parseFloat(row.find('td:eq(9)').text()));
}
function RR(row) {
    row.remove();
    CalcTotal();
}
//Rows

//Reset
function CalcTotal() {
    $('#tFoot tr:first').find('th:eq(1)').text('');
    var qty = 0;
    $.each($('#tBody tr'), function () {
        var r = $(this);
        qty += parseFloat(r.find('td:eq(9)').text());
    })
    $('#tFoot tr:first').find('th:eq(1)').text(qty);
}
function Reset() {
    $('#ProductId').val(null).trigger('change');
    $('#LotNumber').val('');
    $('#DesignId').val(null).trigger('change');
    $('#RollNumber').val(0);
    $('#DesignNumber').val(0);
    $('#JobNumber').val(0);
    $('#Qty').val(0);
    $('#ProductId').select2('open');
}
//Reset

//Add
function S(tBody, c, a, print) {
    $('.loader').show();
    var data = [];
    $.each(tBody.find('tr'), function () {
        var row = $(this);
        var d = {};
        d = {
            DocId: parseInt($('#DocId').val()),
            TransactionDate: $('#TransactionDate').val(),
            AccountId: parseInt($('#AccountId').val()),
            ProductDetailId: parseInt(row.find('td:eq(1)').text()),
            LotNumber: row.find('td:eq(3)').text(),
            DesignId: parseInt(row.find('td:eq(4)').text()),
            RollNumber: parseInt(row.find('td:eq(6)').text()),
            DesignNumber: parseInt(row.find('td:eq(7)').text()),
            JobNumber: parseInt(row.find('td:eq(8)').text()),
            Qty: parseFloat(row.find('td:eq(9)').text()),
            Print: print
        }
        data.push(d);
    })
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: {vm: data},
        success: function () {
            $('.loader').hide();
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
$('#RollNumber').on('focus', function () {
    $(this).select();
})
$('#DesignNumber').on('focus', function () {
    $(this).select();
})
$('#JobNumber').on('focus', function () {
    $(this).select();
})
$('#Qty').on('focus', function () {
    $(this).select();
})
//focus eve

//keydown eve
$('#LotNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#DesignId').select2('open');
    }
})
$('#RollNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#DesignNumber').focus();
    }    
})
$('#DesignNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#JobNumber').focus();
    }
})
$('#JobNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Qty').focus();
    }
})
$('#Qty').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})
//keydown eve

//select2 Eve
$('#AccountId').on('select2:close', function () {
    $('#ProductId').select2('open');
})
$('#ProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#LotNumber').focus();
    }
})
$('#DesignId').on('select2:close', function () {
    var id = $(this).val();
    $('#RollNumber').focus();
})
//select2 Eve