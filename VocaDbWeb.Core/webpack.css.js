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

require('laravel-mix-merge-manifest');

mix
	.mergeManifest()
	.options({
		processCssUrls: false
	})
	.setPublicPath('./wwwroot/')


	// Base CSS
	.less("../VocaDbWeb/Content/css.less", "wwwroot/Content")

	.less("../VocaDbWeb/Content/embedSong.less", "wwwroot/Content")

	// CSS for jqxRating
	.styles([
		"../VocaDbWeb/Scripts/jqwidgets27/styles/jqx.base.css"
	], "../VocaDbWeb/Scripts/jqwidgets27/styles/css.css");


if (mix.inProduction()) {
	mix.version();
}
