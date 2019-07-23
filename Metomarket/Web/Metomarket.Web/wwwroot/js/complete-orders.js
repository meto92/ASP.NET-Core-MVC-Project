 $(() => {
    const $periodInMonths = $("input[name=PeriodInMonths]");
    const $pricePerMonthField = $("input[name=price-per-month]");
    const total = +$("input[name=Total]").val().substring(1);

    const changePricePerMonthValue = () => {
        const monthsCount = +$("input[name=PeriodInMonths]:checked").val();

        if (!monthsCount || monthsCount < 0) {
            return;
        }

        const pricePerMonth = total / monthsCount;

        $pricePerMonthField.val(`$${pricePerMonth.toFixed(2)}`);
    };

    changePricePerMonthValue();

    $periodInMonths.change(changePricePerMonthValue);
})()