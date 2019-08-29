$(() => {
    const $price = $("input[name=Price]");
    const $name = $("input[name=Name]");
    const $typeId = $("[name=TypeId]");
    const $suggestPrice = $("#suggest-price");

    const url = "/products/create";
    const getPriceMethodName = "GetPrice";
    const setPriceMethodName = "SetPrice";
    const bootstrapDisplayInlineBlockClass = "d-inline-block";

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .build();
   
    const suggestPrice = () => {
        const name = $name.val().trim();

        if (!name) {
            return;
        }

        connection.invoke(getPriceMethodName, name);
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

    connection.on(setPriceMethodName, (price) => {
        $price.val(+price.toFixed(0));
    });

    connection.start()
        .catch((err) => {
            console.error(err.toString());
    });
})()