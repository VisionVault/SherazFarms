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
                        + '<td>' + item.account + '</td>'
                        + '<td class="text-center">' + item.vehicleNumber + '</td>'
                        + '<td class="text-center">' + item.driverName + '</td>'
                        + '<td class="text-center">' + item.actualWeight + '</td>'
                        + '<td class="text-center">' + item.amount + '</td>'
                        + '<td class="text-center">' + item.paymentTerm + '</td>'
                        + '<td class="text-center">'
                        + '<a href="#" class="btnPrint"><i class="fas fa-print"></i></a> | '
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
                    { "width": "8.33%" },
                    { "width": "4.15%" },
                    { "width": "16.66%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                ],
                footerCallback: function (row, data, start, end, display) {
                    var api = this.api();

                    // Remove the formatting to get integer data for summation
                    var intVal = function (i) {
                        return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
                    };

                    // Total over all pages
                    total = api
                        .column(7)
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        }, 0);

                    // Total over this page
                    pageTotal = api
                        .column(7, { page: 'current' })
                        .data()
                        .reduce(function (a, b) {
                            return intVal(a) + intVal(b);
                        }, 0);

                    // Update footer
                    $(api.column(7).footer()).html('Rs.' + pageTotal + ' ( Rs.' + total + ' total)');
                },
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
                newOption($('#AccountId'), data[0].account, data[0].accountId);
                $('#VehicleNumber').val(data[0].vehicleNumber);
                $('#DriverName').val(data[0].driverName);
                $('#Remarks').val(data[0].remarks);
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td hidden>' + item.productId + '</td>'
                        + '<td>' + item.product + '</td>'
                        + '<td hidden>' + item.locationId + '</td>'
                        + '<td>' + item.location + '</td>'
                        + '<td class="text-right">' + item.emptyWeight + '</td>'
                        + '<td class="text-right">' + item.loadedWeight + '</td>'
                        + '<td class="text-right">' + item.actualWeight + '</td>'
                        + '<td class="text-right">' + item.rate + '</td>'
                        + '<td class="text-right">' + item.amount + '</td>'
                        + '<td class="text-right">' + item.discountP + '</td>'
                        + '<td class="text-right">' + item.net + '</td>'
                        + '<td class="text-center">'
                        + '<a href="#" class="btnUR"><span class="fa fa-edit"></span></a> | '
                        + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a>'
                        + '</td>'
                        + '</tr>';
                    tBody.append(row);
                })
                $('#BillDiscount').val(data[0].billDiscount);
                $('#Payment').val(data[0].payment);
                if (data[0].paymentTermId == 1) {
                    $('#RadioCash').prop('checked', true);
                }
                else if (data[0].paymentTermId == 2) {
                    $('#RadioCredit').prop('checked', true);
                }
                else {
                    $('#RadioBank').prop('checked', true);
                    newOption($('#BankAcId'), data[0].bankAc, data[0].bankAcId);
                }
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
    var select = $('#ProductId').select2('data');
    var location = $('#LocationId').select2('data');
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#ProductId').val());
        row.find('td:eq(2)').text((select[0].name || select[0].text));
        row.find('td:eq(3)').text($('#LocationId').val());
        row.find('td:eq(4)').text((location[0].name || location[0].text));
        row.find('td:eq(5)').text($('#EmptyWeight').val());
        row.find('td:eq(6)').text($('#LoadedWeight').val());
        row.find('td:eq(7)').text($('#ActualWeight').val());
        row.find('td:eq(8)').text($('#Rate').val());
        row.find('td:eq(9)').text($('#Amount').val());
        row.find('td:eq(10)').text($('#DiscountP').val());
        row.find('td:eq(11)').text($('#Net').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#ProductId').val() + '</td>'
            + '<td>' + (select[0].name || select[0].text) + '</td>'
            + '<td hidden>' + $('#LocationId').val() + '</td>'
            + '<td>' + (location[0].name || location[0].text) + '</td>'
            + '<td class="text-right">' + $('#EmptyWeight').val() + '</td>'
            + '<td class="text-right">' + $('#LoadedWeight').val() + '</td>'
            + '<td class="text-right">' + $('#ActualWeight').val() + '</td>'
            + '<td class="text-right">' + $('#Rate').val() + '</td>'
            + '<td class="text-right">' + $('#Amount').val() + '</td>'
            + '<td class="text-right">' + $('#DiscountP').val() + '</td>'
            + '<td class="text-right">' + $('#Net').val() + '</td>'
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
    newOption($('#ProductId'), row.find('td:eq(2)').text(), parseInt(row.find('td:eq(1)').text()));
    newOption($('#LocationId'), row.find('td:eq(4)').text(), parseInt(row.find('td:eq(3)').text()));
    $('#EmptyWeight').val(parseFloat(row.find('td:eq(5)').text()));
    $('#LoadedWeight').val(parseFloat(row.find('td:eq(6)').text()));
    $('#ActualWeight').val(parseFloat(row.find('td:eq(7)').text()));
    $('#Rate').val(parseFloat(row.find('td:eq(8)').text()));
    $('#Amount').val(parseFloat(row.find('td:eq(9)').text()));
    $('#DiscountP').val(parseFloat(row.find('td:eq(10)').text()));
    $('#Net').val(parseFloat(row.find('td:eq(11)').text()));
    rIndex = row.index();
}
function RR(row) {
    row.remove();
    CalcTotal();
}
//Rows

//Reset
function CalcWeight() {
    var emptyW = parseFloat($('#EmptyWeight').val()),
        loadedW = parseFloat($('#LoadedWeight').val()),
        actualW = 0;
    if (emptyW < loadedW) {
        actualW = (loadedW - emptyW).toFixed(2);
    }
    $('#ActualWeight').val(actualW);
    CalcAmount();
}
function CalcAmount() {
    var weight = parseFloat($('#ActualWeight').val()),
        rate = parseFloat($('#Rate').val()),
        amount = (weight * rate).toFixed(0),
        discRate = parseFloat($('#DiscountP').val()),
        net = amount;
    if (discRate > 0) {
        net = (weight * discRate).toFixed(0);
    }
    $('#Amount').val(amount);
    $('#Net').val(net);
}
function CalcTotal() {
    $('#tFoot tr:first').find('th:eq(1)').text('');
    $('#tFoot tr:first').find('th:eq(3)').text('');
    $('#tFoot tr:first').find('th:eq(5)').text('');
    var weight = 0, amount = 0, net = 0;
    $.each($('#tBody tr'), function () {
        var r = $(this);
        weight += parseFloat(r.find('td:eq(7)').text());
        amount += parseFloat(r.find('td:eq(9)').text());
        net += parseFloat(r.find('td:eq(11)').text());
    })
    $('#tFoot tr:first').find('th:eq(1)').text(weight);
    $('#tFoot tr:first').find('th:eq(3)').text(amount);
    $('#tFoot tr:first').find('th:eq(5)').text(net);
    CGT();
}
function CGT() {
    $('#Net').val(0);
    var amount = parseFloat($('#tFoot tr:first').find('th:eq(5)').text()), disc = parseFloat($('#BillDiscount').val()), net = amount - disc;
    if (net > 0) {
        $('#BillNet').val(net);
    }
}
function Reset() {
    $('#ProductId').val(null).trigger('change');
    $('#LocationId').val(null).trigger('change');
    $('#EmptyWeight').val(0);
    $('#LoadedWeight').val(0);
    $('#ActualWeight').val(0);
    $('#Rate').val(0);
    $('#Amount').val(0);
    $('#DiscountP').val(0);
    $('#Net').val(0);
    $('#ProductId').select2('open');
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
            AccountId: parseInt($('#AccountId').val()),
            VehicleNumber: $('#VehicleNumber').val(),
            DriverName: $('#DriverName').val(),
            Remarks: $('#Remarks').val(),
            ProductId: parseInt(row.find('td:eq(1)').text()),
            LocationId: parseInt(row.find('td:eq(3)').text()),
            EmptyWeight: parseFloat(row.find('td:eq(5)').text()),
            LoadedWeight: parseFloat(row.find('td:eq(6)').text()),
            ActualWeight: parseFloat(row.find('td:eq(7)').text()),
            Rate: parseFloat(row.find('td:eq(8)').text()),
            Amount: parseFloat(row.find('td:eq(9)').text()),
            DiscountP: parseFloat(row.find('td:eq(10)').text()),
            Discount: 0,
            Net: parseFloat(row.find('td:eq(11)').text()),
            BillDiscountP: 0,
            BillDiscount: parseFloat($('#BillDiscount').val()),
            Payment: parseFloat($('#Payment').val()),
            PaymentTermId: parseInt($('input[name="PaymentTerm"]:checked').val()),
            BankAcId: parseInt($('#BankAcId').val()),
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
                    var w = window.open('/Reports/Invoice?d=' + data.d + '&t=' + data.t + '&hr=' + $('#HR').is(':checked'));
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
    var w = window.open('/Reports/Invoice?d=' + d + '&t=' + t + '&hr=' + $('#HR').is(':checked'));
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
$('#EmptyWeight').on('blur', function () {
    CalcWeight();
})
$('#LoadedWeight').on('blur', function () {
    CalcWeight();
})
$('#ActualWeight').on('blur', function () {
    CalcWeight();
})
$('#Rate').on('blur', function () {
    CalcAmount();
})
$('#DiscountP').on('blur', function () {
    CalcAmount();
})
$('#Payment').on('blur', function () {
    CGT();
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
$('#EmptyWeight').on('focus', function () {
    $(this).select();
})
$('#LoadedWeight').on('focus', function () {
    $(this).select();
})
$('#ActualWeight').on('focus', function () {
    $(this).select();
})
$('#Rate').on('focus', function () {
    $(this).select();
})
$('#Amount').on('focus', function () {
    $(this).select();
})
$('#DiscountP').on('focus', function () {
    $(this).select();
})
$('#Payment').on('focus', function () {
    $(this).select();
})
//focus eve

//input eve
$('#EmptyWeight').on('input', function () {
    CN($(this));
})
$('#LoadedWeight').on('input', function () {
    CN($(this));
})
$('#ActualWeight').on('input', function () {
    CN($(this));
})
$('#Rate').on('input', function () {
    CN($(this));
})
$('#Amount').on('input', function () {
    CN($(this));
})
$('#DiscountP').on('input', function () {
    CN($(this));
})
$('#Payment').on('input', function () {
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
$('#VehicleNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#DriverName').focus();
    }
})
$('#DriverName').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Remarks').focus();
    }
})
$('#Remarks').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#ProductId').select2('open');
    }
})
$('#EmptyWeight').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#LoadedWeight').focus();
    }
})
$('#LoadedWeight').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#ActualWeight').focus();
    }
})
$('#ActualWeight').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Rate').focus();
    }
})
$('#Rate').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#DiscountP').focus();
    }
})
$('#DiscountP').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})
$('#BillDiscount').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Payment').focus();
    }
})
$('#Payment').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnSave').focus();
    }
})
//keydown eve

//select2 Eve
$('#AccountId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#VehicleNumber').focus();
    }
})
$('#ProductId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#LocationId').select2('open');
    }
})
$('#LocationId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#EmptyWeight').focus();
    }
})
//select2 Eve