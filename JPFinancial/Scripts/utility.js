//TODO: convert into jQuery extension (https://www.sitepoint.com/5-ways-declare-functions-jquery/)

function updateLabels(changedRange, rangeClass) {

    const id = changedRange.id.split('_')[1];

    const rangeLabel = $('#rangeLabel_' + id)[0];
    const rangeValue = Number(changedRange.value);
    const rangeDefaultValue = Number(changedRange.defaultValue);
    
    rangeLabel.value = changedRange.value;

    if (rangeDefaultValue !== NaN && rangeValue !== NaN) {
        const disposableIncomeElement = $('#disposableIncomeValue')[0];
        const delta = rangeValue - rangeDefaultValue;

        const disposableIncomeText = disposableIncomeElement.value.replace('$', '');
        let disposableIncomeValue = Number(disposableIncomeText);

        const updatedValue = disposableIncomeValue !== NaN
            ? (disposableIncomeValue -= delta).toString()
            : disposableIncomeText;
        disposableIncomeElement.value = '$' + updatedValue;

        setRangeMaxValues(rangeClass, changedRange, delta);

        // Set updated slider's defaultValue to current value for calculating delta next time through
        changedRange.defaultValue = changedRange.value;
    }
};


function setRangeMaxValues(rangeClass, changedRange, delta) {

    const ranges = $('.' + rangeClass);

    ranges.each(function () {
        const max = Number(this.max);

        if (this.id === changedRange.id) {
            this.setAttribute('value', this.value);
        } else {
            if (max !== NaN) {
                const newMax = max - delta;
                this.max = newMax.toString();
            }
        }
    });

}