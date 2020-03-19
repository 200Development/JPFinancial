//TODO: convert into jQuery extension (https://www.sitepoint.com/5-ways-declare-functions-jquery/)

function updateMultipleRelatedSliders(changedAccount, slidersClass) {

    if (Number(changedAccount.defaultValue) !== NaN && Number(changedAccount.value) !== NaN) {
        const valueDiff = Number(changedAccount.value) - Number(changedAccount.defaultValue);
        const sliders = $('.' + slidersClass);

        sliders.each(function () {
            const max = Number(this.max);

            if (this.id === changedAccount.id) {
                this.setAttribute('value', this.value);
            } else {
                if (max !== NaN) {
                    const newMax = max - valueDiff;
                    this.max = newMax.toString();
                }
            }
        });

        const disposableIncomeElement = $('#disposableIncomeValue')[0];
        let disposableIncome = disposableIncomeElement.value.replace('$', '');
        disposableIncome = Number(disposableIncome);
        if (disposableIncome !== NaN) {
            disposableIncome -= valueDiff;
            disposableIncomeElement.value = '$' + disposableIncome.toString();
        }
    } else {
        console.log("Error getting slider delta");
    }
};