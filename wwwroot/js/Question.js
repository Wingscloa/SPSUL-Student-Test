window.onload = function () {
    console.log('loaded Question.js');
    var select2Elements = document.querySelectorAll(".select2");
    for (var i = 0; i < select2Elements.length; i++) {
        $(select2Elements[i]).select2(
            {
                theme: 'bootstrap-5',
                width: '100%',
                dropdownAutoWidth: true,
                allowClear: true,
                closeOnSelect: false,
            });
    }
}