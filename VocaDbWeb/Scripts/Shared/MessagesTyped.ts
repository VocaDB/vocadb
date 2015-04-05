
module vdb {
	
	export class ui {

		public static showSuccessMessage = (message?: string) => {
			var successMessage = $("#successMessage");
			var successMessageString = $("#successMessageString");
			if (message) successMessageString.text(message);
			return successMessage.show().delay(4000).fadeOut("normal");
		}

		public static showErrorMessage = (message?: string) => {
			var errorMessage = $("#errorMessage");
			var errorMessageString = $("#errorMessageString");
			if (message) errorMessageString.text(message);
			return errorMessage.show().delay(4000).fadeOut("normal");
		}

		public static showWarnMessage = (message?: string) => {
			var warnMessage = $("#warnMessage");
			var warnMessageString = $("#warnMessageString");
			if (message) warnMessageString.text(message);
			return warnMessage.show().delay(4000).fadeOut("normal");
		}

		public static showLoadingMessage = () => {
			var ajaxLoadingMessage = $("#loadingMessage");
			if (ajaxLoadingMessage) return ajaxLoadingMessage.show();
		}

		public static hideLoadingMessage = () => {
			var ajaxLoadingMessage = $("#loadingMessage");
			if (ajaxLoadingMessage) return ajaxLoadingMessage.hide();
		}

		private static thanksForRatingMessage: string;

		public static showThankYouForRatingMessage = () => {
			ui.showSuccessMessage(ui.thanksForRatingMessage);
		}

		private static positionMessageWrapper = () => {
			var messages = $("#messages");
			if (messages.offset() && messages.offset().top <= $(window).scrollTop() + 10) {
				return messages.addClass("fixed");
			} else {
				return messages.removeClass("fixed");
			}
		}

		private static initMessages = () => {

			//$(".alert").alert();
			ui.positionMessageWrapper();
			$(window).scroll(() => {
				return ui.positionMessageWrapper();
			});
			if ($("#successMessageString").text() !== "") ui.showSuccessMessage();
			if ($("#errorMessageString").text() !== "") ui.showErrorMessage();
			if ($("#warnMessageString").text() !== "") ui.showWarnMessage();

		}

		public static initAll = (thanksForRatingMessage: string) => {
			ui.thanksForRatingMessage = thanksForRatingMessage;
			ui.initMessages();
		}

	}

} 