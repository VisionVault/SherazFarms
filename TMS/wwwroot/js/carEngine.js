
function LD(c, a) {
    $('.loader').show();
    $.ajax({
        type: 'Get',
        url: '/' + c + '/' + a,
        success: function (data) {
            if (data.length > 0) {
                var sr = 0;
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = '<tr>'
                        + '<td class="text-center">' + sr + '</td>'
                        + '<td>' + item.carBrand + '</td>'
                        + '<td>' + item.engineNumber + '</td>'
                        + '<td>' + item.engineCC + '</td>'
                        + '<td>' + item.oilCapacity + '</td>'
                        + '<td>' + item.company + '</td>'
                        + '<td>' + item.oilFilter + '</td>'
                        + '<td>' + item.airFilter + '</td>'
                        + '<td>' + item.fuelFilter + '</td>'
                        + '<td>' + item.cabinFilter + '</td>'
                        + '<td class="text-center">'
                        + '<a href="/CarEngine/Edit?id=' + item.engineNumber + '" class=""><i class="fas fa-edit"></i></a></td>'
                        + '</tr>';
                    $('#tBody').append(row);
                })
            }
            $('#myTable').DataTable({
                columns: [
                    { "width": "8.33%" },
                    { "width": "16.66%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
                    { "width": "8.33%" },
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
            DisplayError(resp.responseText)
        }
    })
}
function Rec(c, a, id) {
    $('.loader').show();
    $.ajax({
        type: 'Get',
        url: '/' + c + '/' + a,
        data: { id: id },
        success: function (data) {
            var sr = 0;
            if (data.length > 0) {
                $('#InitialEngineNumber').val(data[0].initialEngineNumber);
                newOption($('#CarBrandId'), data[0].carBrand, data[0].carBrandId);
                $('#EngineNumber').val(data[0].engineNumber);
                $('#EngineCC').val(data[0].engineCC);
                $('#OilCapacity').val(data[0].oilCapacity);
                $.each(data, function (i) {
                    sr += 1;
                    var item = data[i];
                    var row = $('#tBody').find('tr:eq(' + i + ')');
                    row.find('td:eq(0)').text(sr);
                    row.find('td:eq(1)').text(item.company);
                    row.find('.OilFilter').val(item.oilFilter);
                    row.find('.AirFilter').val(item.airFilter);
                    row.find('.FuelFilter').val(item.fuelFilter);
                    row.find('.CabinFilter').val(item.cabinFilter);
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

function S(c, a) {
    $('.loader').show();
    var data = [];
    $.each($('#tBody tr'), function () {
        var row = $(this);
        var d = {
            InitialEngineNumber: $('#InitialEngineNumber').val(),
            CarBrandId: parseInt($('#CarBrandId').val()),
            EngineNumber: $('#EngineNumber').val(),
            EngineCC: $('#EngineCC').val(),
            OilCapacity: $('#OilCapacity').val(),
            Company: row.find('td:eq(1)').text(),
            OilFilter: row.find('.OilFilter').val(),
            AirFilter: row.find('.AirFilter').val(),
            FuelFilter: row.find('.FuelFilter').val(),
            CabinFilter: row.find('.CabinFilter').val(),
        }
        data.push(d);
    })
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: { vm: data },
        success: function (data) {
            window.location.href = '/' + c + '/Index';
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}

//Keydown
$('#EngineNumber').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#EngineCC').focus();
    }
})
$('#EngineCC').on('keydown', function (e) {
    if (e.which == 13) {
        e.preventDefault();
        $('#OilCapacity').focus();
    }
})

//select2
$('#CarBrandId').on('select2:close', function () {
    var id = $(this).val();
    if (id > 0) {
        $('#EngineNumber').focus();
    }
})