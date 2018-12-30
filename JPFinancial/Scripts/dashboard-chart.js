type = ['', 'info', 'success', 'warning', 'danger'];




$(".pick-class-label").click(function () {
    var new_class = $(this).attr("new-class");
    var old_class = $("#display-buttons").attr("data-class");
    var display_div = $("#display-buttons");
    if (display_div.length) {
        var display_buttons = display_div.find(".btn");
        display_buttons.removeClass(old_class);
        display_buttons.addClass(new_class);
        display_div.attr("data-class", new_class);
    }
});

function doSomething(staticMetrics) {
    if (staticMetrics) {

       
        var totalExpenses = staticMetrics.ExpensesByMonth;
        var mandatoryExpenses = staticMetrics.MandatoryExpensesByMonth;
        var discretionaryExpenses = staticMetrics.DiscretionarySpendingByMonth;

        var totalLength = Object.keys(totalExpenses).length;
        var mandatoryLength = Object.keys(mandatoryExpenses).length;
        var discretionaryLength = Object.keys(discretionaryExpenses).length;

        var monthsArray = new Array(totalLength);
        var expensesArray = new Array(totalLength);
        var mandatoryArray = new Array(mandatoryLength);
        var discretionaryArray = new Array(discretionaryLength);
        var highestAmount = 0;

        for (prop in totalExpenses) {
            monthsArray.push(prop);
            expensesArray.push(totalExpenses[prop]);
            if (totalExpenses[prop] > highestAmount) {
                highestAmount = totalExpenses[prop];
            }
        }

        for (prop in mandatoryExpenses) {
            mandatoryArray.push(mandatoryExpenses[prop]);
            if (mandatoryExpenses[prop] > highestAmount) {
                highestAmount = mandatoryExpenses[prop];
            }
        }

        for (prop in discretionaryExpenses) {
            discretionaryArray.push(discretionaryExpenses[prop]);
            if (discretionaryExpenses[prop] > highestAmount) {
                highestAmount = discretionaryExpenses[prop];
            }
        }

        debugger;
        monthsArray = monthsArray.filter(function(n) { return n != undefined });
        expensesArray = expensesArray.filter(function(n) { return n != undefined });
        mandatoryArray = mandatoryArray.filter(function(n) { return n != undefined });
        discretionaryArray = discretionaryArray.filter(function(n) { return n != undefined });

        var dataSales = {
            labels: monthsArray,
            series: [expensesArray, mandatoryArray, discretionaryArray]
        };

        highestAmount = highestAmount * 1.25;

        var optionsSales = {
            lineSmooth: true,
            low: 0,
            high: highestAmount,
            showArea: false,
            height: "245px",
            axisX: {
                showGrid: false
            },
            lineSmooth: Chartist.Interpolation.simple({
                divisor: 2
            }),
            showLine: true,
            showPoint: false
        };

        var responsiveSales = [
            [
                "screen and (max-width: 640px)", {
                    axisX: {
                        labelInterpolationFnc: function(value) {
                            return value[0];
                        }
                    }
                }
            ]
        ];

        Chartist.Line("#chartExpenses", dataSales, optionsSales, responsiveSales);
    }
}