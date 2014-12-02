(function () {

    this.vdb = this.vdb || {};

    this.vdb.ui = {
        initAll: function () {
            this._initMessages();
            //return this._initPopovers();
        },
        showSuccessMessage: function (message) {
            var successMessage, successMessageString;
            successMessage = $("#successMessage");
            successMessageString = $("#successMessageString");
            if (message) successMessageString.text(message);
            return successMessage.show().delay(4000).fadeOut("normal");
        },
        showErrorMessage: function (message) {
            var errorMessage, errorMessageString;
            errorMessage = $("#errorMessage");
            errorMessageString = $("#errorMessageString");
            if (message) errorMessageString.text(message);
            /*if (errorMessageString.text() === "") {
                errorMessageString.text(vdb.resources.errors.UnknownException);
            }*/
            return errorMessage.show().delay(4000).fadeOut("normal");
        },
        showWarnMessage: function (message) {
            var warnMessage, warnMessageString;
            warnMessage = $("#warnMessage");
            warnMessageString = $("#warnMessageString");
            if (message) warnMessageString.text(message);
            return warnMessage.show().delay(4000).fadeOut("normal");
        },
        showLoadingMessage: function () {
            var ajaxLoadingMessage;
            ajaxLoadingMessage = $("#loadingMessage");
            if (ajaxLoadingMessage) return ajaxLoadingMessage.show();
        },
        hideLoadingMessage: function () {
            var ajaxLoadingMessage;
            ajaxLoadingMessage = $("#loadingMessage");
            if (ajaxLoadingMessage) return ajaxLoadingMessage.hide();
        },
        showThankYouForRatingMessage: function() {
        	vdb.ui.showSuccessMessage(vdb.resources.shared.thanksForRating);
        },
        _initMessages: function () {
            var ajaxLoadingMessage, loadingCount,
              _this = this;
            $(".alert").alert();
            this._positionMessageWrapper();
            $(window).scroll(function () {
                return _this._positionMessageWrapper();
            });
            if ($("#successMessageString").text() !== "") this.showSuccessMessage();
            if ($("#errorMessageString").text() !== "") this.showErrorMessage();
            if ($("#warnMessageString").text() !== "") this.showWarnMessage();
            /*ajaxLoadingMessage = $("#ajaxLoadingMessage");
            loadingCount = 0;
            return $(document).bind("ajaxStart", function () {
                ajaxLoadingMessage.show();
                loadingCount++;
            }).bind("ajaxStop", function () {
                loadingCount--;
                if (loadingCount === 0) ajaxLoadingMessage.hide();
            });*/
        },
        _positionMessageWrapper: function () {
            var messages;
            messages = $("#messages");
            if (messages.offset() && messages.offset().top <= $(window).scrollTop() + 10) {
                return messages.addClass("fixed");
            } else {
                return messages.removeClass("fixed");
            }
        }
        /*_initPopovers: function () {
            var element, options, placement;
            element = $(".popover-icon").add(".popover-text");
            placement = element.attr("data-popover-placement") || "right";
            options = {
                placement: placement
            };
            return element.popover(options);
        }*/
    };

}).call(this);
