$(document).ready(function () {
    $("#divReceive").show();
    $("#divSpend").hide();
    $("#divAmount").hide();
    $("#divDebitAccount").show();
    $("#divCreditAccount").hide();

    $("#ddType").change(function () {
        var type = document.getElementById("ddType").selectedIndex;
        var receive = $("#divReceive");
        var spend = $("#divSpend");
        var amount = $("#divAmount");
        var debitAccount = $("#divDebitAccount");
        var creditAccount = $("#divCreditAccount");

        /* Income */
        if (type === 0) {
            receive.show();
            spend.hide();
            amount.hide();
            debitAccount.show();
            creditAccount.hide();
        }
            /* Expense */
        else if (type === 1) {
            receive.hide();
            spend.show();
            amount.hide();
            debitAccount.hide();
            creditAccount.show();
        }
            /* Transfer */
        else if (type === 2) {
            receive.hide();
            spend.hide();
            amount.show();
            debitAccount.show();
            creditAccount.show();
        }
    });
});