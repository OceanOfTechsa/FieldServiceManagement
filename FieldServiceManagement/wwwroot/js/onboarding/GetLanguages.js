function GetLanguages(search, loading, vm) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Search/GetLanguages',
        type: 'POST',
        data: { SearchName: search, __RequestVerificationToken: token },
        success: function (result) {
            vm.options = result.map(function (x) {
                return {
                    id: x.id,
                    name: '('+x.code+')' + " " + x.name
                };
            });
        },
        error: function () { loading(false); }
    });
}