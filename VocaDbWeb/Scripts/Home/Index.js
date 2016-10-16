
$(document).ready(function () {

	$('#songs-navi .scrollable-item').click(function (e) {
        e.preventDefault();

        $("#songs-navi .scrollable-item").removeClass("active");
        $(this).addClass("active");

        var songId = $(this).find(".js-songId").val();
        $.post("../Home/PVContent", { songId: songId }, function (content) {
            $("#songPreview").html(content);
        });

    });

	$("#songs-navi .scrollable-item").eq(0).addClass("active");
    $(".scrollable").scrollable({ vertical: true, mousewheel: true, keyboard: false });

    function setRating(rating, callback) {

        var songId = $("#songPreview").find(".js-songId").val();

        $.post("/api/songs/" + songId + "/ratings", { rating: rating }, callback);

    }

    $("#songPreview").on("click", "#addFavoriteLink", function () {

        setRating('Favorite', function () {

            $("#removeFavoriteLink").show();
            $("#ratingButtons").hide();
            vdb.ui.showSuccessMessage(vdb.resources.song.thanksForRating);

        });

        return false;

    });

    $("#songPreview").on("click", "#addLikeLink", function () {

        setRating('Like', function () {

            $("#removeFavoriteLink").show();
            $("#ratingButtons").hide();
            vdb.ui.showSuccessMessage(vdb.resources.song.thanksForRating);

        });

        return false;

    });

    $("#songPreview").on("click", "#removeFavoriteLink", function () {

        setRating('Nothing', function () {

            $("#ratingButtons").show();
            $("#removeFavoriteLink").hide();

        });

        return false;

    });

    $("#newAlbums img").vdbAlbumToolTip();
    $("#topAlbums img").vdbAlbumToolTip();

});
