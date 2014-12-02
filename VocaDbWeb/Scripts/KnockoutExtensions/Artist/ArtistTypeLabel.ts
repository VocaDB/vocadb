
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
		addClass: "label-info",
		removeClasses: "label-important label-inverse",
		content: "V"
	},
	UTAU: {
		addClass: "label-important",
		removeClasses: "label-info label-inverse",
		content: "U"
	},
	OtherVoiceSynthesizer: {
		addClass: "label-inverse",
		removeClasses: "label-info label-important",
		content: "O"
	},
	Utaite: {
		addClass: "label-info",
		removeClasses: "label-important label-inverse",
		content: "U"
	},
	OtherVocalist: {
		addClass: "",
		removeClasses: "label-important label-inverse label-info",
		content: "O"
	}
};

ko.bindingHandlers.artistTypeLabel = {
	init: (element: HTMLElement, valueAccessor: () => string) => {
		var val = valueAccessor();

		if (!val) {
			$(element).removeClass("label");
			return;
		}

		var typeInfo: ArtistTypeInfo = artistTypeInfos[val];

		if (typeInfo) {
			$(element).removeClass(typeInfo.removeClasses).addClass("label " + typeInfo.addClass);
			$(element).text(typeInfo.content);
		}
		
	}
};
