$(() => {
    const activeClassName = "active";
    const dataAttrName = "data_name";

    let type = "";
    let searchTerm = "";

    const filterProducts = () => {        
        $(".product").addClass("d-flex")
            .attr("display", "none")
            .filter((index, article) => {
                return !$(article).find(".product-type")
                    .text()
                    .includes(type)
                    || !$(article).find(".product-name")
                        .text()
                        .toLowerCase()
                        .includes(searchTerm.toLowerCase());
            })
            .removeClass("d-flex")
            .css("display", "none");
    };

    $("aside nav.types a").click(e => {
        const $clickedA = $(e.target);

        type = $clickedA.attr(dataAttrName);

        $(`aside nav.types a.${activeClassName}`).removeClass(activeClassName);
        $clickedA.addClass(activeClassName);

        filterProducts();
    });

    $("input[name=search]").on("input", e => {
        searchTerm = $(e.target).val().trim();

        filterProducts();
    });
})()