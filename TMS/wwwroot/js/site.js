var product = null;
var acc = null;

//Codes
function GetMaxCode(tBox, c) {
    $('.loader').show();
    tBox.val('');
    $.ajax({
        type: 'Get',
        url: '/' + c + '/GetMaxCode',
        success: function (data) {
            tBox.val(data);
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}
function GetCode(tBox, c, a, id) {
    $('.loader').show();
    tBox.val('');
    return $.ajax({
        type: 'Get',
        url: '/' + c + '/' + a,
        data: {id: id},
        success: function (data) {
            tBox.val(data);
            $('.loader').hide();
        },
        error: function (resp) {
            $('.loader').hide();
            //DisplayError(resp.responseText);
        }
    })
}
//Codes

//Accounts
function GetAccount(param, method) {
    acc = null;
    return $.ajax({
        type: 'Get',
        url: '/Accounts/GetBy' + method,
        data: { param: param },
        success: function (data) {
            if (data != null) {
                acc = data;
            }
        },
        error: function (resp) {
            DisplayError(resp.responseText);
        }
    })
}
function GetByCar(param) {
    acc = null;
    return $.ajax({
        type: 'Get',
        url: '/Accounts/GetByCar',
        data: { param: param },
        success: function (data) {
            if (data != null) {
                acc = data;
            }
        },
        error: function (resp) {
            DisplayError(resp.responseText);
        }
    })
}
//Accounts

//Property
function GetProduct(param, method) {
    product = null;
    return $.ajax({
        type: 'Post',
        url: '/Product/GetBy' + method,
        data: param,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data != null) {
                product = data;
            }
        },
        error: function (resp) {
            DisplayError(resp.responseText);
        }
    })
}
//Property

//General a or e
function GenAE(fd, c, a) {
    $('.loader').show();
    $.ajax({
        type: 'Post',
        url: '/' + c + '/' + a,
        data: fd,
        contentType: false,
        processData: false,
        success: function () {
            $('.loader').hide();
            if (a == 'Add') {
                DisplayAddOrList(c);
            }
            if (a == 'Edit') {
                DisplayList(c);
            }
        },
        error: function (resp) {
            $('.loader').hide();
            DisplayError(resp.responseText);
        }
    })
}
//General a or e

//Add Item
function AITL(select, c, a, ph) {
    select.html('');
    $.ajax({
        type: 'Get',
        url: '/' + c + '/' + a,
        success: function (data) {
            var op = new Option('Select ' + ph, null, false, true);
            select.append(op);
            if (data.length > 0) {
                $.each(data, function (i) {
                    var item = data[i];
                    var op2 = new Option(item.name, item.id, false, false);
                    select.append(op2);
                })
            }
        },
        error: function (resp) {
            DisplayError(resp.responseText);
        }
    })
}
//Add Item

//Numeric
function CN(handler) {
    var value = handler.val();
    if (value == null || value == ''
        || isNaN(value) || value.indexOf(' ') > -1) {
        handler.val(0);
        handler.select();
    }
}
//Numeric

//Calc Date
function CalcDate(plan, sDate) {
    var months = 1;
    if (plan == 2) {
        months = 3;
    }
    if (plan == 3) {
        months = 6;
    }
    if (plan == 4) {
        months = 12;
    }
    var date = new Date(sDate);
    date = new Date(date.setMonth(date.getMonth() + +months));
    var year = date.getFullYear().toString();
    var month = (date.getMonth() + 1).toString();
    var day = date.getDate().toString();
    if (parseInt(month) < 10) {
        month = "0" + month;
    }
    if (parseInt(day) < 10) {
        day = "0" + day;
    }
    return year + '-' + month + '-' + day;
}
//Calc Date

//Select 2
function initRSelect(select, type, c, a, ph) {
    select.select2({
        placeholder: 'Select ' + ph,
        allowClear: true,
        delay: 0,
        templateResult: type == 'Acc' ? formatAccResult : type == 'Cus' ? formatCusResult : type == 'Pro' ? formatProResult : formatResult,
        templateSelection: formatSelection,
        ajax: {
            type: 'Get',
            url: '/' + c + '/' + a,
            data: function (params) {
                var q = {
                    param: params.term
                }
                return q;
            },
            processResults: function (data) {
                var d = {
                    results: data
                }
                return d;
            }
        }
    })
}
function initRSelectWGAL(select, type, c, a, ph, galId) {
    select.select2({
        placeholder: 'Select ' + ph,
        allowClear: true,
        delay: 0,
        templateResult: type == 'Acc' ? formatAccResult : formatResult,
        templateSelection: formatSelection,
        ajax: {
            type: 'Get',
            url: '/' + c + '/' + a,
            data: function (params) {
                var q = {
                    param: params.term,
                    galId: galId
                }
                return q;
            },
            processResults: function (data) {
                var d = {
                    results: data
                }
                return d;
            }
        }
    })
}
function formatResult(repo) {
    if (repo.loading) {
        return repo.text;
    }

    var con = $(
        '<div class="select2-results-repository">'
        + '<div class="name">' + repo.name + '</div>'
        + '</div>'
    );

    return con;
}
function formatSelection(repo) {
    return repo.name || repo.text;
}
function formatAccResult(repo) {
    if (repo.loading) {
        return repo.text;
    }

    var con = $(
        '<div class="select2-result-repository">'
        + '<div class="row"><div class="col-md-12">Name : ' + repo.name + '</div></div>'
        + '<div class="row">'
        + '<div class="col-md-6" style="font-size: 12px;">Contact : ' + repo.contact + '</div>'
        + '<div class="col-md-6" style="font-size: 12px;">City : ' + repo.city + '</div>'
        + '</div>'
        + '</div>'
    );
    return con;
}
function formatCusResult(repo) {
    if (repo.loading) {
        return repo.text;
    }
    var con = $(
        '<div class="select2-result-repository">'
        + '<div class="row">'
        + '<div class="col-md-12">'
        + 'Name: ' + repo.name
        + '</div>'
        + '<div class="col-md-12">'
        + 'Contact: ' + repo.contact
        + '</div>'
        + '</div>'
    );
    return con;
}
function formatProResult(repo) {
    if (repo.loading) {
        return repo.text;
    }

    var con = $(
        '<div class="select2-result-repository">'
        + '<div class="row">'
        + '<div class="col-md-12">Name : ' + repo.name + '</div>'
        + '</div>'
        + '</div>'
    );
    return con;
}
function newOption(select, text, id) {
    var op = new Option(text, id, false, true);
    select.append(op);
}
//Select 2

//Alert
function DisplayError(text) {
    $.alert({
        title: 'Error',
        content: text,
        type: 'red',
        typeAnimated: true,
        buttons: {
            OK: {
                btnClass: 'btn-red'
            }
        }
    })
}
function DisplayAddOrList(c) {
    $.alert({
        title: 'Message',
        content: 'Data saved successfully',
        type: 'green',
        typeAnimated: true,
        buttons: {
            AddNew: {
                text: 'Add Another',
                btnClass: 'btn-green',
                action: function () {
                    window.location.reload();
                }
            },
            List: {
                text: 'Go to List',
                btnClass: 'btn-blue',
                action: function () {
                    window.location.href = '/' + c + '/Index';
                }
            },
        }
    })
}
function DisplayList(c) {
    $.alert({
        title: 'Message',
        content: 'Data saved successfully',
        type: 'green',
        typeAnimated: true,
        buttons: {
            List: {
                text: 'Go to List',
                btnClass: 'btn-blue',
                action: function () {
                    window.location.href = '/' + c + '/Index';
                }
            },
        }
    })
}
//Alert
