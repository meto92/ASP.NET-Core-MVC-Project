$(() => {
    const $alerts = $(".alerts");
    const $usersCount = $("#usersCount");
    const $contractsCount = $("#contractsCount");

    const url = "/dashboard";
    const userRegisteredMethodName = "UserRegistered";
    const contractCreatedMethodName = "ContractCreated";
    const userRegisteredMessage = "A new user has registered.";
    const contractCreatedMessage = "A new contract has been made.";
    const alertDisplayTime = 7000;
    const alertFadeOut = 1000;

    const createSuccessAlert = (message) => {
        return $("<div>").addClass("alert alert-success alert-dismissible")
            .text(message)
            .append($("<button>")
                .addClass("close")
                .attr("type", "button")
                .attr("data-dismiss", "alert")
                .attr("aria-label", "Close")
                .append($("<span>")
                    .attr("aria-hidden", "true")
                    .html("&times;")));
    };

    const manageAlert = ($alert) => {
        $alerts.append($alert);

        setTimeout(() => {
            $alert.fadeOut(alertFadeOut, () => {
                $alert.remove();
            });
        }, alertDisplayTime);
    };

    const increaseValueByOne = ($element) => {
        const numberValue = +$element.text();

        if (!numberValue && numberValue != 0) {
            return;
        }

        $element.text(numberValue + 1);
    };

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .build();

    connection.on(userRegisteredMethodName, () => {
        manageAlert(createSuccessAlert(userRegisteredMessage));

        increaseValueByOne($usersCount);
    });

    connection.on(contractCreatedMethodName, () => {
        manageAlert(createSuccessAlert(contractCreatedMessage));

        increaseValueByOne($contractsCount);
    });

    connection.start()
        .catch((err) => {
            console.error(err.toString());
    });
})()