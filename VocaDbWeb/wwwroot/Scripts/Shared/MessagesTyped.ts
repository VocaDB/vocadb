export default class ui {
  public static showSuccessMessage = (message?: string): JQuery => {
    var successMessage = $('#successMessage');
    var successMessageString = $('#successMessageString');
    if (message) successMessageString.text(message);
    return successMessage.show().delay(4000).fadeOut('normal');
  };

  public static showErrorMessage = (message?: string): JQuery => {
    var errorMessage = $('#errorMessage');
    var errorMessageString = $('#errorMessageString');
    if (message) errorMessageString.text(message);
    return errorMessage.show().delay(4000).fadeOut('normal');
  };

  public static showWarnMessage = (message?: string): JQuery => {
    var warnMessage = $('#warnMessage');
    var warnMessageString = $('#warnMessageString');
    if (message) warnMessageString.text(message);
    return warnMessage.show().delay(4000).fadeOut('normal');
  };

  public static showLoadingMessage = (): JQuery => {
    var ajaxLoadingMessage = $('#loadingMessage');
    if (ajaxLoadingMessage) return ajaxLoadingMessage.show();
    return undefined;
  };

  public static hideLoadingMessage = (): JQuery => {
    var ajaxLoadingMessage = $('#loadingMessage');
    if (ajaxLoadingMessage) return ajaxLoadingMessage.hide();
    return undefined;
  };

  private static thanksForRatingMessage: string;

  public static showThankYouForRatingMessage = (): void => {
    ui.showSuccessMessage(ui.thanksForRatingMessage);
  };

  private static positionMessageWrapper = (): JQuery => {
    var messages = $('#messages');
    if (
      messages.offset() &&
      messages.offset().top <= $(window).scrollTop() + 10
    ) {
      return messages.addClass('fixed');
    } else {
      return messages.removeClass('fixed');
    }
  };

  private static initMessages = (): void => {
    //$(".alert").alert();
    ui.positionMessageWrapper();
    $(window).scroll(() => {
      return ui.positionMessageWrapper();
    });
    if ($('#successMessageString').text() !== '') ui.showSuccessMessage();
    if ($('#errorMessageString').text() !== '') ui.showErrorMessage();
    if ($('#warnMessageString').text() !== '') ui.showWarnMessage();
  };

  public static initAll = (thanksForRatingMessage: string): void => {
    ui.thanksForRatingMessage = thanksForRatingMessage;
    ui.initMessages();
  };
}
