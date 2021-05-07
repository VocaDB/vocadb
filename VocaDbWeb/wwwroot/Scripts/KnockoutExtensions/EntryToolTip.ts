import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import EntryRefContract from '../DataContracts/EntryRefContract';
import functions from '../Shared/GlobalFunctions';

declare global {
  interface KnockoutBindingHandlers {
    albumToolTip: KnockoutBindingHandler;
    artistToolTip: KnockoutBindingHandler;
    entryToolTip: KnockoutBindingHandler;
    eventToolTip: KnockoutBindingHandler;
    songToolTip: KnockoutBindingHandler;
    tagToolTip: KnockoutBindingHandler;
    userToolTip: KnockoutBindingHandler;
  }
}

export function initToolTip(
  element: HTMLElement,
  relativeUrl: string,
  id: number,
  params?: any,
  foreignDomain?: string,
): void {
  const whitelistedDomains = [
    'http://vocadb.net',
    'https://vocadb.net',
    'http://utaitedb.net',
    'https://utaitedb.net',
    'https://touhoudb.com',
  ];
  const url =
    foreignDomain &&
    _.some(whitelistedDomains, (domain) =>
      _.includes(foreignDomain.toLocaleLowerCase(), domain),
    )
      ? functions.mergeUrls(foreignDomain, relativeUrl)
      : functions.mapAbsoluteUrl(relativeUrl);
  const data = _.assign({ id: id }, params);

  $(element).qtip({
    content: {
      text: 'Loading...',
      ajax: {
        url: url,
        type: 'GET',
        data: data,
        dataType: foreignDomain ? 'jsonp' : undefined,
      },
    },
    position: {
      viewport: $(window),
    },
    style: {
      classes: 'tooltip-wide',
    },
  });
}

export interface TooltipOptions {
  toolTipDomain?: string;
  version?: number;
}

ko.bindingHandlers.entryToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<EntryRefContract>,
  ): void => {
    var value: EntryRefContract = ko.unwrap(valueAccessor());

    switch (value.entryType) {
      case 'Album':
        initToolTip(element, '/Album/PopupContent', value.id);
        break;
      case 'Artist':
        initToolTip(element, '/Artist/PopupContent', value.id);
        break;
    }
  },
};

ko.bindingHandlers.albumToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<number>,
  ): void => {
    initToolTip(element, '/Album/PopupContent', ko.unwrap(valueAccessor()));
  },
};

ko.bindingHandlers.artistToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<number>,
  ): void => {
    initToolTip(element, '/Artist/PopupContent', ko.unwrap(valueAccessor()));
  },
};

ko.bindingHandlers.eventToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<number>,
  ): void => {
    const culture = vdb.values.uiCulture || undefined;
    initToolTip(element, '/Event/PopupContent', ko.unwrap(valueAccessor()), {
      culture: culture,
    });
  },
};

ko.bindingHandlers.songToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<number>,
    allPropertiesAccessor: () => TooltipOptions,
  ): void => {
    const allProps = allPropertiesAccessor();
    initToolTip(
      element,
      '/Song/PopupContentWithVote',
      ko.unwrap(valueAccessor()),
      { version: allProps.version },
      allProps.toolTipDomain,
    );
  },
};

ko.bindingHandlers.tagToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<number>,
  ): void => {
    var culture = vdb.values.uiCulture || undefined;
    var lang =
      ContentLanguagePreference[vdb.values.languagePreference] || undefined;
    initToolTip(element, '/Tag/PopupContent', ko.unwrap(valueAccessor()), {
      culture: culture,
      lang: lang,
    });
  },
};

ko.bindingHandlers.userToolTip = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<number>,
  ): void => {
    var culture = vdb.values.uiCulture || undefined;
    initToolTip(element, '/User/PopupContent', ko.unwrap(valueAccessor()), {
      culture: culture,
    });
  },
};
