function GetIndustryCategories(search, industryId, loading, vm) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Search/GetIndustryCategories',
        type: 'POST',
        data: { SearchName: search, IndustryId: industryId, __RequestVerificationToken: token },
        success: function (result) {
            vm.options = result.map(function (x) {
                return { id: x.id, name: x.name };
            });
        },
        error: function () { loading(false); }
    });
}