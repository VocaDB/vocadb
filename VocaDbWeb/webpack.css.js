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

// https://github.com/JeffreyWay/laravel-mix/issues/1914#issuecomment-503392761

mix
	.options({
		processCssUrls: false
	})


	// Base CSS
	.less("Content/Styles/base.less", "Content/Styles", {
		strictMath: true
	})
	.less("Content/Styles/PVViewer_Black.less", "Content/Styles")
	.less("Content/Styles/ExtLinks.less", "Content/Styles")
	.less("Content/Styles/Overrides.less", "Content/Styles")
	.less("Content/Styles/Search.less", "Content/Styles")
	.less("Content/Styles/song.less", "Content/Styles", {
		strictMath: true
	})
	.less("Content/Styles/userpage.less", "Content/Styles")
	.styles([
		"Content/bootstrap.css",
		"Content/bootstrap-responsive.css",
		"Content/Site.css",
		"Content/Styles/base.css",
		//"Content/Styles/Snow2013.css",
		"Content/Styles/PVViewer_Black.css",
		"Content/Styles/ExtLinks.css",
		"Content/Styles/Overrides.css",
		"Content/Styles/StyleOverrides.css",
		"Content/Styles/Search.css",
		"Content/Styles/song.css",
		"Content/Styles/userpage.css"
	], "Content/css.css")

	.less("Content/Styles/embedSong.less", "Content/Styles")
	.styles([
		"Content/bootstrap.css",
		"Content/Styles/embedSong.css"
	], "Content/embedSong.css")

	// CSS for jqxRating
	.styles([
		"Scripts/jqwidgets27/styles/jqx.base.css"
	], "Scripts/jqwidgets27/styles/css.css");

