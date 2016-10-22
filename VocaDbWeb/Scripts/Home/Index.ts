
interface JQuery {
	scrollable: (params: any) => void;
}

$(() => {

	function initRatingButtons() {
		const urlMapper = new vdb.UrlMapper(vdb.values.baseAddress);
		const repo = new vdb.repositories.UserRepository(urlMapper);
		const ratingBar = $("#rating-bar");
		const songId = ratingBar.data('song-id');
		const rating = ratingBar.data('rating');
		const viewModel = new vdb.viewModels.PVRatingButtonsViewModel(repo, { id: songId, vote: rating }, () => {
			vdb.ui.showSuccessMessage(vdb.resources.song.thanksForRating);				
		}, vdb.values.isLoggedIn);
		ko.applyBindings(viewModel, ratingBar[0]);		
	}

	initRatingButtons();

	$('#songs-navi .scrollable-item').click(function (e) {
		e.preventDefault();

		$("#songs-navi .scrollable-item").removeClass("active");
		$(this).addClass("active");

		const songId = $(this).find(".js-songId").val();
		$.post("../Home/PVContent", { songId: songId }, (content: string) => {
			$("#songPreview").html(content);
			initRatingButtons();
		});

	});

	$("#songs-navi .scrollable-item").eq(0).addClass("active");
	$(".scrollable").scrollable({ vertical: true, mousewheel: true, keyboard: false });

	$("#newAlbums img").vdbAlbumToolTip();
	$("#topAlbums img").vdbAlbumToolTip();

});
