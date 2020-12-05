
interface KnockoutBindingHandlers {

    sortable: KnockoutBindingHandler;

}

/* 
    Note: currently replaced by knockout-sortable. Code from Knock me out blog, but doesn't work.
    Knockout binding for a sortable list using JQuery UI.    

    Binding value: Function to be called when an item has been moved and sorting has stopped. 
    Arguments are the moved item and its new index.
*/
ko.bindingHandlers.sortable = {
    init: function (element, valueAccessor) {
        var list = valueAccessor();
        $(element).sortable({
            update: (event, ui) => {
                var data = ko.dataFor(ui.item[0]);
                var index: number = ui.item.index();
                if (index > 0) {
                    list.remove(data);
                    list.splice(index, 0, data);
                }
            }
        });
    }
};
