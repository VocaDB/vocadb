
import { baseAddress } from '../Shared/GlobalValues';
import { isLoggedIn } from '../Shared/GlobalValues';
import PVRatingButtonsViewModel from '../ViewModels/PVRatingButtonsViewModel';
import ui from '../Shared/MessagesTyped';
import UrlMapper from '../Shared/UrlMapper';
import UserRepository from '../Repositories/UserRepository';

interface JQuery {
	scrollable: (params: any) => void;
}

$(() => {

	function initRatingButtons() {
		const urlMapper = new UrlMapper(baseAddress);
		const repo = new UserRepository(urlMapper);
		const ratingBar = $("#rating-bar");

		if (!ratingBar.length) {
			return;
		}

		const songId = ratingBar.data('song-id');
		const rating = ratingBar.data('rating');
		const viewModel = new PVRatingButtonsViewModel(repo, { id: songId, vote: rating }, () => {
			ui.showSuccessMessage(vdb.resources.song.thanksForRating);				
		}, isLoggedIn);
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
