$(document).ready(function () {
    initDatePickers();
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(applicationInitHandler);

    function applicationInitHandler() {
        initDatePickers();
    }

    function initDatePickers() {
        // if ($.browser.webkit) {
        $('[id*=ParameterTable] td span:contains("Date")')
            .each(function () {
                var td = $(this).parent().next();
                $('input', td).Zebra_DatePicker({
                    format: 'm/d/Y'
                });
            });
        // }
    }
});
