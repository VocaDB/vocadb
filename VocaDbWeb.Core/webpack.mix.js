const mix = require('laravel-mix');

/*
 |--------------------------------------------------------------------------
 | Mix Asset Management
 |--------------------------------------------------------------------------
 |
 | Mix provides a clean, fluent API for defining some Webpack build steps
 | for your Laravel application. By default, we are compiling the Sass
 | file for the application as well as bundling up all the JS files.
 |
 */

mix
	.setPublicPath('./wwwroot/')
	.webpackConfig({
		output: {
			library: 'app'
		}
	})
	.options({
		processCssUrls: false
	})


	/*.scripts([
		"wwwroot/Scripts/jquery-2.2.1.js",
		"wwwroot/Scripts/bootstrap.js",
		//"wwwroot/Scripts/jquery-ui-1.10.1.js", // doesn't work if bundled together
		"wwwroot/Scripts/knockout-3.4.1.js",
		"wwwroot/Scripts/knockout.punches.min.js",
		"wwwroot/Scripts/lodash.js",
		"wwwroot/Scripts/qTip/jquery.qtip.js",
		"wwwroot/Scripts/marked.js"
	], "wwwroot/bundles/shared/libs.js")

	.scripts([
		"wwwroot/Scripts/jquery-ui-1.10.4.js"
	], "wwwroot/bundles/shared/jqui.js")*/
	.js("wwwroot/Scripts/libs.js", "wwwroot/bundles/shared")

	// SHARED BUNDLES
	// Legacy common scripts - should be phased out
	.scripts(["wwwroot/Scripts/VocaDb.js"], "wwwroot/bundles/VocaDB.js")

	.ts("wwwroot/Scripts/App.ts", "wwwroot/bundles")

	// Included on all pages (including front page)
	// Generally the references go from viewmodels -> repositories -> models -> support classes
	.scripts([
	], "wwwroot/bundles/shared/common.js")

	// Included on all pages except the front page (to optimize front page load time).
	.scripts([
		"wwwroot/Scripts/moment-with-locales.js",
	], "wwwroot/bundles/shared/main.js")

	// Included on all entry edit and create pages (album, artist, my settings etc.)
	.scripts([
		"wwwroot/Scripts/knockout-sortable.js"
	], "wwwroot/bundles/shared/edit.js")

	.scripts([
		"wwwroot/Scripts/jquery.tools.min.js"	// REVIEW
	], "wwwroot/bundles/Home/Index.js")

	.scripts([
		"wwwroot/Scripts/jqwidgets27/jqxcore.js", "wwwroot/Scripts/jqwidgets27/jqxrating.js"
	], "wwwroot/bundles/jqxRating.js")


	// VIEW-SPECIFIC BUNDLES
	.scripts([
	], "wwwroot/bundles/ActivityEntry/Index.js")


	// HACK: this produces an empty file called Create.js to prevent 404 errors.
	// TODO: these scripts commands must be removed along with the corresponding RenderScripts in .cshtml files.
	.scripts([
	], "wwwroot/bundles/Album/Create.js")

	.scripts([
	], "wwwroot/bundles/Album/Details.js")

	.scripts([
	], "wwwroot/bundles/Album/Edit.js")

	.scripts([
	], "wwwroot/bundles/Album/Merge.js")

	.scripts([
	], "wwwroot/bundles/Artist/Create.js")

	.scripts([
		"wwwroot/Scripts/soundcloud-api.js"	// REVIEW
	], "wwwroot/bundles/Artist/Details.js")

	.scripts([
	], "wwwroot/bundles/Artist/Edit.js")

	.scripts([
	], "wwwroot/bundles/Artist/Merge.js")

	.scripts([
		"wwwroot/Scripts/page.js"
	], "wwwroot/bundles/Discussion/Index.js")

	.scripts([
	], "wwwroot/bundles/EventSeries/Details.js")

	.scripts([
	], "wwwroot/bundles/EventSeries/Edit.js")

	.scripts([
	], "wwwroot/bundles/ReleaseEvent/Details.js")

	.scripts([
	], "wwwroot/bundles/ReleaseEvent/Edit.js")

	.scripts([
		"wwwroot/Scripts/soundcloud-api.js"	// REVIEW
	], "wwwroot/bundles/Search/Index.js")

	.scripts([
	], "wwwroot/bundles/Song/Create.js")

	.scripts([
		"wwwroot/Scripts/MediaElement/mediaelement-and-player.min.js",
	], "wwwroot/bundles/Song/Details.js")

	.scripts([
	], "wwwroot/bundles/Song/Edit.js")

	.scripts([
	], "wwwroot/bundles/Song/Merge.js")

	.scripts([
		"wwwroot/Scripts/url.js"
	], "wwwroot/bundles/Song/TopRated.js")

	.scripts([
		"wwwroot/Scripts/soundcloud-api.js"	// REVIEW
	], "wwwroot/bundles/SongList/Details.js")

	.scripts([
	], "wwwroot/bundles/SongList/Edit.js")

	.scripts([
	], "wwwroot/bundles/SongList/Featured.js")

	.scripts([
	], "wwwroot/bundles/SongList/Import.js")

	.scripts([
	], "wwwroot/bundles/Tag/Details.js")

	.scripts([
	], "wwwroot/bundles/Tag/Edit.js")

	.scripts([
	], "wwwroot/bundles/Tag/Index.js")

	.scripts([
	], "wwwroot/bundles/Tag/Merge.js")

	.scripts([
	], "wwwroot/bundles/User/AlbumCollection.js")

	.scripts([
		"wwwroot/Scripts/soundcloud-api.js"	// REVIEW
	], "wwwroot/bundles/User/Details.js")

	.scripts([
	], "wwwroot/bundles/User/Index.js")

	.scripts([
	], "wwwroot/bundles/User/Messages.js")

	.scripts([
	], "wwwroot/bundles/User/MySettings.js")

	.scripts([
		"wwwroot/Scripts/soundcloud-api.js"	// REVIEW
	], "wwwroot/bundles/User/RatedSongs.js")

	.scripts([
	], "wwwroot/bundles/Venue/Details.js")

	.scripts([
	], "wwwroot/bundles/Venue/Edit.js")


	// Base CSS
	.less("wwwroot/Content/css.less", "wwwroot/Content")

	.less("wwwroot/Content/embedSong.less", "wwwroot/Content")

	// CSS for jqxRating
	.styles([
		"wwwroot/Scripts/jqwidgets27/styles/jqx.base.css"
	], "wwwroot/Scripts/jqwidgets27/styles/css.css");


if (mix.inProduction()) {
	mix.scripts([], "wwwroot/bundles/tests.js");
} else {
	mix.ts("wwwroot/Scripts/tests.ts", "wwwroot/bundles");
}


if (mix.inProduction()) {
	mix.version();
}
