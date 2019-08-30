$(() => {
    const $price = $("input[name=Price]");
    const $name = $("input[name=Name]");
    const $typeId = $("[name=TypeId]");
    const $suggestPrice = $("#suggest-price");
    
    const url = "/api/suggestPrice";
    const bootstrapDisplayInlineBlockClass = "d-inline-block";

    const suggestPrice = () => {
        const name = $name.val().trim();

        if (!name) {
            return;
        }

        const obj = {
            name
        };

        $.ajax({
            url: url,
            type: "GET",
            data: obj,
            contentType: "application/json; charset=utf-8",
            success: (price) => {
                $price.val(+price.toFixed(0));
            },
            error: (err) => {
                console.error(err);
            }
        })
    };

    const showHideSuggestPriceButton = () => {
        const selectedType = $typeId.find(":selected").text();

        if (selectedType.toLowerCase().includes("laptop")) {
            $suggestPrice.addClass(bootstrapDisplayInlineBlockClass);
        }
        else {
            $suggestPrice.removeClass(bootstrapDisplayInlineBlockClass).hide();
        }
    };

    $suggestPrice.click((e) => {
        e.preventDefault();

        suggestPrice();
    });

    $typeId.change(showHideSuggestPriceButton);

    showHideSuggestPriceButton();
})()