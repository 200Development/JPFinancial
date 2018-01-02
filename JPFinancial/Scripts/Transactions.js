$(document).ready(function () {
    $("#divDebitAccount").show();
    $("#divCreditAccount").hide();


    $("#ddType").change(function () {
        var type = document.getElementById("ddType").selectedIndex;
        var debitAccount = $("#divDebitAccount");
        var creditAccount = $("#divCreditAccount");

        /* Income */
        if (type === 0) {
            debitAccount.show();
            creditAccount.hide();
        }
            /* Expense */
        else if (type === 1) {
            debitAccount.hide();
            creditAccount.show();
        }
            /* Transfer */
        else if (type === 2) {
            debitAccount.show();
            creditAccount.show();
        }
    });
});