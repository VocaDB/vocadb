
function tabLoaded(controllerUrl, entryId, event, ui) {

	$("#createComment").click(function () {

		var message = $("#newCommentMessage").val();

		if (message == "") {
			alert("Message cannot be empty");
			return false;
		}

		$("#newCommentMessage").val("");

		$.post(controllerUrl + "/CreateComment", { entryId: entryId, message: message }, function (result) {

			$("#newCommentPanel").after(result);

		});

		return false;

	});

	$(document).on("click", "a.deleteComment", function () {

		if (!confirm("Are you sure you want to delete this comment?"))
			return false;

		var btn = this;
		var id = getId(this);

		$.post(controllerUrl + "/DeleteComment", { commentId: id }, function () {

			$(btn).parent().parent().parent().parent().remove();

		});

		return false;

	});

}
