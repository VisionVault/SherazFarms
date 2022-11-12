var index = -1;

function LD(c, a) {
    $('.loader').show();
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        success: function (data) {
            var sr = 0;
            if (data.length > 0) {
                $.each(data, function (i) {
                    var item = data[i];
                    sr += 1;
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td>' + item.account + '</td>'
                        + '<td>' + item.contact + '</td>'
                        + '<td>' + item.registrationNumber + '</td>'
                        + '<td>' + item.color + '</td>'
                        + '<td>' + item.currentMilage + '</td>'
                        + '<td>' + item.nextDueMilage + '</td>'
                        + '<td class="text-center">'
                        + '<a href="/' + c + '/Edit?id=' + item.accountId + '"><i class="fas fa-edit"></i></a>'
                        + '</td>'
                        + '</tr>';
                    $('#tBody').append(row);
                })
            }
            $('#myTable').DataTable({
                columns: [
                    { "width": "8.33%" },
                    { "width": "33.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" }
                ]
            });
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            $.alert({
                title: 'Error',
                content: resp.responseText,
                theme: 'supervan'
            })

        }
    })
}
function Rec(c, a, id) {
    $('.loader').show();
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: {id: id},
        success: function (data) {
            if (data.length > 0) {
                newOption($('#AccountId'), data[0].account, data[0].accountId);
                $.each(data, function (i) {
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center" hidden>' + item.id + '</td>'
                        + '<td>' + item.registrationNumber + '</td>'
                        + '<td>' + item.color + '</td>'
                        + '<td class="text-center">'
                        + '<a href="#" class="btnUR"><i class="fas fa-edit"></i></a> | '
                        + '<a href="#" class="btnRR"><i class="fas fa-trash"></i></a>'
                        + '</td>'
                        + '</tr>';
                    $('#tBody').append(row);
                })
            }
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            $.alert({
                title: 'Error',
                content: resp.responseText,
                theme: 'supervan'
            })

        }
    })
}

function AR() {
    if (index > -1) {
        var row = $('#tBody').find('tr:eq(' + index + ')');
        row.find('td:eq(0)').text($('#Id').val());
        row.find('td:eq(1)').text($('#RegistrationNumber').val());
        row.find('td:eq(2)').text($('#Color').val());
        index = -1;
    }
    else {
        var row = '<tr>'
            + '<td class="text-center" hidden>' + $('#Id').val() + '</td>'
            + '<td>' + $('#RegistrationNumber').val() + '</td>'
            + '<td>' + $('#Color').val() + '</td>'
            + '<td class="text-center">'
            + '<a href="#" class="btnUR"><i class="fas fa-edit"></i></a> | '
            + '<a href="#" class="btnRR"><i class="fas fa-trash"></i></a>'
            + '</td>'
            + '</tr>';
        $('#tBody').append(row);
    }
    Reset();
}
function UR(row) {
    index = row.index();
    $('#Id').val(parseInt(row.find('td:eq(0)').text()));
    $('#RegistrationNumber').val(row.find('td:eq(1)').text());
    $('#Color').val(row.find('td:eq(2)').text());
    $('#RegistrationNumber').focus();
}
function RR(row) {
    var id = parseInt(row.find('td:eq(0)').text());
    if (id > 0) {
        $.alert({
            title: 'Warning',
            content: 'Do you want to delete this car? This action cannot be reversed',
            theme: 'supervan',
            buttons: {
                YES: function () {
                    $.ajax({
                        type: 'Get',
                        url: '/CustomerCar/Delete',
                        data: { id: id },
                        success: function () {
                            row.remove();
                        },
                        error: function (resp) {
                            $.alert({
                                title: 'Error',
                                content: resp.responseText,
                                theme: 'supervan',
                            })
                        }
                    })
                },
                NO: function () {
                    return;
                }
            }
        })
    }
    else {
        row.remove();
    }
}
function Reset() {
    $('#Id').val(0);
    $('#RegistrationNumber').val('');
    $('#Color').val('');
    $('#RegistrationNumber').focus();
}

function S(c, a) {
    $('.loader').show();
    var data = [];
    $.each($('#tBody tr'), function (i) {
        var row = $(this);
        var d = {
            AccountId: parseInt($('#AccountId').val()),
            Id: parseInt(row.find('td:eq(0)').text()),
            RegistrationNumber: row.find('td:eq(1)').text(),
            Color: row.find('td:eq(2)').text(),
        };
        data.push(d);
    })
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: { vm: data },
        success: function (data) {
            $('.loader').hide();
            window.location.href = '/' + c + '/Index';
        },
        error: function (resp) {
            $('.loader').hide();
            $.alert({
                title: 'Error',
                content: resp.responseText,
                theme: 'supervan'
            })

        }
    })
}

//click
$('#btnAdd').click(function (e) {
    e.preventDefault();
    AR();
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

//keydown
$('#RegistrationNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#Color').focus();
    }
})
$('#Color').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#btnAdd').focus();
    }
})

//change
$('#AccountId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#RegistrationNumber').focus();
    }
})