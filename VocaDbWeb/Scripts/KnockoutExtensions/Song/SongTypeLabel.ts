
interface KnockoutBindingHandlers {
	songTypeLabel: KnockoutBindingHandler;
}

module vdb.knockoutExtensions.song {

	interface SongTypeLabelInfo {
		addClass: string;
		removeClasses: string;
		content: string;
	}

	var songTypeLabelInfos = {
		Cover: {
			addClass: "",
			removeClasses: "label-important label-inverse label-info label-success",
			content: "C"
		},
		DramaPV: {
			addClass: "label-success",
			removeClasses: "label-info label-inverse label-important",
			content: "D"
		},
		Instrumental: {
			addClass: "label-inverse",
			removeClasses: "label-info label-important label-success",
			content: "I"
		},
		Mashup: {
			addClass: "",
			removeClasses: "label-important label-inverse label-info label-success",
			content: "M"
		},
		Original: {
			addClass: "label-info",
			removeClasses: "label-important label-inverse",
			content: "O"
		},
		Other: {
			addClass: "",
			removeClasses: "label-important label-inverse label-info",
			content: "O"
		},
		Remaster: {
			addClass: "label-info",
			removeClasses: "label-important label-inverse label-success",
			content: "R"
		},
		Remix: {
			addClass: "",
			removeClasses: "label-important label-inverse label-info label-success",
			content: "R"
		},
		MusicPV: {
			addClass: "label-success",
			removeClasses: "label-important label-inverse label-info",
			content: "PV"
		}
	};

	export function songTypeLabel(element: HTMLElement, valueAccessor: () => string) {

		var val = valueAccessor();

		if (!val) {
			$(element).removeClass("label");
			return;
		}

		var typeInfo: SongTypeLabelInfo = songTypeLabelInfos[val];

		if (typeInfo) {
			$(element).removeClass(typeInfo.removeClasses).addClass("label " + typeInfo.addClass);
			$(element).text(typeInfo.content);
		}

	}

}

ko.bindingHandlers.songTypeLabel = {
	init: vdb.knockoutExtensions.song.songTypeLabel
};
