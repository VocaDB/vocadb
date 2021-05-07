interface KnockoutBindingHandlers {
  artistTypeLabel: KnockoutBindingHandler;
}

interface ArtistTypeInfo {
  addClass: string;
  removeClasses: string;
  content: string;
}

var artistTypeInfos = {
  Vocaloid: {
    addClass: 'label-info',
    removeClasses: 'label-important label-inverse',
    content: 'V',
  },
  UTAU: {
    addClass: 'label-important',
    removeClasses: 'label-info label-inverse',
    content: 'U',
  },
  CeVIO: {
    addClass: 'label-success',
    removeClasses: 'label-info label-inverse label-important',
    content: 'C',
  },
  OtherVoiceSynthesizer: {
    addClass: 'label-inverse',
    removeClasses: 'label-info label-important',
    content: 'O',
  },
  Utaite: {
    addClass: 'label-info',
    removeClasses: 'label-important label-inverse',
    content: 'U',
  },
  OtherVocalist: {
    addClass: '',
    removeClasses: 'label-important label-inverse label-info',
    content: 'O',
  },
  SynthesizerV: {
    addClass: '',
    removeClasses: 'label-important label-inverse label-info',
    content: 'SV',
  },
};

ko.bindingHandlers.artistTypeLabel = {
  init: (
    element: HTMLElement,
    valueAccessor: () => string,
    allBindingsAccessor: () => any,
  ): void => {
    var val = valueAccessor();

    if (!val) {
      $(element).removeClass('label');
      return;
    }

    if (allBindingsAccessor().typeLabelShowTitle) {
      $(element).attr('title', val);
    }

    var typeInfo: ArtistTypeInfo = artistTypeInfos[val];

    if (typeInfo) {
      $(element)
        .removeClass(typeInfo.removeClasses)
        .addClass('label ' + typeInfo.addClass);
      $(element).text(typeInfo.content);
    }
  },
};
