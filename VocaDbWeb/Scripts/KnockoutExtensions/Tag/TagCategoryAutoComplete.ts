import functions from '@Shared/GlobalFunctions';
import $ from 'jquery';
import ko, { Observable } from 'knockout';

declare global {
  interface KnockoutBindingHandlers {
    tagCategoryAutoComplete: KnockoutBindingHandler;
  }
}

// Tag category autocomplete search box.
ko.bindingHandlers.tagCategoryAutoComplete = {
  init: (
    element: HTMLElement,
    valueAccessor: () => Observable<string>,
    allBindingsAccessor?: () => any,
  ): void => {
    var url = functions.mapAbsoluteUrl('/api/tags/categoryNames');
    var clearValue: boolean = ko.unwrap(allBindingsAccessor!().clearValue);

    $(element).autocomplete({
      source: (ui: { term: any }, callback: (result: string[]) => void) =>
        $.getJSON(url, { query: ui.term }, callback),
      select: (event: Event, ui) => {
        var value = valueAccessor();
        value(ui.item.label);

        if (clearValue) {
          $(element).val('');
          return false;
        } else {
          return true;
        }
      },
    });
  },
};
