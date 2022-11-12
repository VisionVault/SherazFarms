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
                        + '<td class="text-center">' + item.debit + '</td>'
                        + '<td class="text-center">' + item.credit + '</td>'
                        + '<td class="text-center"><a href="/' + c + '/Edit?Id=' + item.docId + '" class="btn btn-sm btn-warning shadow"><i class="fas fa-edit"></i> Edit</a> | '
                        + '<button class="btnRemove btn btn-sm btn-danger shadow"><i class="fas fa-trash"></i> Remove</button> </td>'
                        + '</tr>';
                    tBody.append(row);
                })
            }
            $('#myTable').DataTable({
                columns: [
                    { "width": "8.33%" },
                    { "width": "16.66%" },
                    { "width": "8.33%" },
                    { "width": "16.66%" },
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
                        + '<td hidden>' + item.accountId + '</td>'
                        + '<td>' + item.account + '</td>'
                        + '<td>' + item.narration + '</td>'
                        + '<td class="text-right">' + item.debit + '</td>'
                        + '<td class="text-right">' + item.credit + '</td>'
                        + '<td class="text-center"><a href="#" class="btnUR"><span class="fa fa-file-invoice"></span></a > | '
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
    if ($('#DebitAcId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Select a debit account',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#DebitAcId').select2('open');
            }
        });
        v = false;
        return v;
    }
    if ($('#CreditAcId').val() == null) {
        $.alert({
            title: 'Error',
            content: 'Select a credit account',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#CreditAcId').select2('open');
            }
        });
        v = false;
        return v;
    }
    if ($('#Narration').val() == '') {
        $.alert({
            title: 'Error',
            content: 'Please enter narration',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#Narration').focus();
            }
        });
        v = false;
        return v;
    }
    if (isNaN($('#Amount').val()) || parseFloat($('#Amount').val()) == 0) {
        $.alert({
            title: 'Error',
            content: 'Please enter rate',
            type: 'red',
            typeAnimated: true,
            onDestroy: function () {
                $('#Amount').focus();
            }
        });
        v = false;
        return v;
    }
    return v;
}
function AR(tBody) {
    var debitSelect = $('#AccountId').select2('data');
    if (rIndex > -1) {
        var row = tBody.find('tr:eq(' + rIndex + ')');
        row.find('td:eq(1)').text($('#AccountId').val());
        row.find('td:eq(2)').text((debitSelect[0].name || debitSelect[0].text));
        row.find('td:eq(3)').text($('#Narration').val());
        row.find('td:eq(4)').text($('#Debit').val());
        row.find('td:eq(5)').text($('#Credit').val());
        rIndex = -1;
    }
    else {
        var sr = $('#tBody tr').length + 1;
        var row = '<tr>'
            + '<td class="text-center">' + sr + '</td>'
            + '<td hidden>' + $('#AccountId').val() + '</td>'
            + '<td>' + (debitSelect[0].name || debitSelect[0].text) + '</td>'
            + '<td>' + $('#Narration').val() + '</td>'
            + '<td class="text-right">' + $('#Debit').val() + '</td>'
            + '<td class="text-right">' + $('#Credit').val() + '</td>'
            + '<td class="text-center"><a href="#" class="btnUR"><span class="fa fa-file-invoice"></span></a > | '
            + '<a href="#" class="btnRR"><span class="fa fa-trash"></span></a></td>'
            + '</tr>';
        tBody.append(row);
    }
    Reset();
    CalcTotal();
    var d = $('.tableFixed');
    d.scrollTop(d.prop("scrollHeight"));
}
function UR(row) {
    rIndex = row.index();
    newOption($('#AccountId'), row.find('td:eq(2)').text(), parseInt(row.find('td:eq(1)').text()));
    $('#Narration').val(row.find('td:eq(3)').text());
    $('#Debit').val(parseFloat(row.find('td:eq(4)').text()));
    $('#Credit').val(parseFloat(row.find('td:eq(5)').text()));
}
function RR(row) {
    row.remove();
    CalcTotal();
}
//Rows

//Calc
function CalcTotal() {
    var debit = 0, credit = 0;
    $('#tFoot tr:first').find('th:eq(1)').text(0);
    $('#tFoot tr:first').find('th:eq(2)').text(0);
    $.each($('#tBody tr'), function () {
        var row = $(this);
        debit += parseFloat(row.find('td:eq(4)').text());
        credit += parseFloat(row.find('td:eq(5)').text());
    })
    $('#tFoot tr:first').find('th:eq(1)').text(debit);
    $('#tFoot tr:first').find('th:eq(2)').text(credit);
}
//Calc

//Reset
function Reset() {
    $('#AccountId').val(null).trigger('change');
    $('#Narration').val(null);
    $('#Debit').val(0);
    $('#Credit').val(0);
    $('#AccountId').select2('open');
}
//Reset

//Add
function S(tBody, c, a) {
    $('.loader').show();
    var data = [];
    $.each(tBody.find('tr'), function () {
        var row = $(this);
        var d = d = {
            DocId: parseInt($('#DocId').val()),
            TransactionDate: $('#TransactionDate').val(),
            AccountId: parseInt(row.find('td:eq(1)').text()),
            Narration: row.find('td:eq(3)').text(),
            Debit: parseFloat(row.find('td:eq(4)').text()),
            Credit: parseFloat(row.find('td:eq(5)').text()),
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
                DisplayAddOrList(c);
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
$('#Debit').on('blur', function () {
})
$('#Credit').on('blur', function () {
})
//blur eve

//Click Eve
$('#btnAdd').click(function () {
    AR($('#tBody'));
})
$('#btnReset').click(function () {
    Reset();
})
$('#tBody').on('click', '.btnUR', function () {
    var row = $(this).closest('tr');
    UR(row);
})
$('#tBody').on('click', '.btnRR', function () {
    var row = $(this).closest('tr');
    RR(row);
})
//Click Eve

//focus eve
$('#Debit').on('focus', function () {
    $(this).select();
})
$('#Credit').on('focus', function () {
    $(this).select();
})
//focus eve

//keydown eve
$('#Narration').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Debit').focus();
    }    
})
$('#Debit').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Credit').focus();
    }
})
$('#Credit').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})
//keydown eve


//select2 Eve
$('#AccountId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#Narration').focus();
    }
})
//select2 Eve