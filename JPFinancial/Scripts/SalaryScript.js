$(document).ready(function () {
    var benefitWrapper = $(".BenefitsDivId");
    var addBenefitBtn = $(".addBenefitBtn");
    var expenseWrapper = $(".ExpensesDivId");
    var addExpensBtn = $(".addExpenseBtn");
    var payFrequency = $("#payFrequency");
    var paydayOfWeek = $("#paydayOfWeek");
    var selectedPayFrequencyIndex = $(payFrequency).find(":selected").index;
    var benefitDivCounter = 1;
    var expenseDivCounter = 1;


    if ($(selectedPayFrequencyIndex) === 1 ||
        $(selectedPayFrequencyIndex) === 2 ||
        $(selectedPayFrequencyIndex) === 3) {
        paydayOfWeek.collapse = false;
    } else
        paydayOfWeek.collapse = true;

    $(addBenefitBtn).click(function (e) {
        e.preventDefault();
        $(benefitWrapper).append('<div class="col-md-4 col-md-offset-2">' +
            '@Html.EditorFor(model => model.Benefit, new {htmlAttributes = new {@class = "form-control"}})' +
            '@Html.ValidationMessageFor(model => model.Benefit, "", new {@class = "text-danger"})' +
            '</div>' +
            '<div class="col-md-4 col-md-offset-2">' +
            '@Html.EditorFor(model => model.BenefitAmount, new {htmlAttributes = new {@class = "form-control"}})' +
            '@Html.ValidationMessageFor(model => model.BenefitAmount, "", new {@class = "text-danger"})' +
            '<button class"col-md-1">X</button>' +
            '</div>');
    });

    $(benefitWrapper)
        .on("click",
            ".remove_field",
            function (e) { //user click on remove text
                e.preventDefault();
                $(this).parent('div').remove();
            });

    $(addExpensBtn).click(function (e) {
        e.preventDefault();
        $(expenseWrapper).append('<div class="col-md-4 col-md-offset-2">' +
            '@Html.EditorFor(model => model.Expense, new {htmlAttributes = new {@class = "form-control"}})' +
            '@Html.ValidationMessageFor(model => model.Expense, "", new {@class = "text-danger"})' +
            '</div>' +
            '<div class="col-md-4 col-md-offset-2">' +
            '@Html.EditorFor(model => model.ExpenseAmount, new {htmlAttributes = new {@class = "form-control"}})' +
            '@Html.ValidationMessageFor(model => model.ExpenseAmount, "", new {@class = "text-danger"})' +
            '<button class"col-md-1">X</button>' +
            '</div>');
    });

    $(expenseWrapper)
        .on("click",
            ".remove_field",
            function (e) { //user click on remove text
                e.preventDefault();
                $(this).parent('div').remove();
            });
});
