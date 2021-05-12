const mix = require('laravel-mix');
const path = require('path');

require('laravel-mix-eslint');

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
			library: 'app',
		},
	})
	.options({
		processCssUrls: false,
	})
	.alias({
		'@DataContracts': path.join(__dirname, 'wwwroot/Scripts/DataContracts'),
		'@Helpers': path.join(__dirname, 'wwwroot/Scripts/Helpers'),
		'@KnockoutExtensions': path.join(
			__dirname,
			'wwwroot/Scripts/KnockoutExtensions',
		),
		'@Models': path.join(__dirname, 'wwwroot/Scripts/Models'),
		'@Repositories': path.join(__dirname, 'wwwroot/Scripts/Repositories'),
		'@Shared': path.join(__dirname, 'wwwroot/Scripts/Shared'),
		'@ViewModels': path.join(__dirname, 'wwwroot/Scripts/ViewModels'),
	})
	.eslint({
		fix: false,
		extensions: ['ts', 'tsx'],
	})

	.js('wwwroot/Scripts/libs.js', 'wwwroot/bundles/shared')

	// SHARED BUNDLES
	// Legacy common scripts - should be phased out
	.scripts(['wwwroot/Scripts/VocaDb.js'], 'wwwroot/bundles/VocaDB.js')

	.ts('wwwroot/Scripts/App.ts', 'wwwroot/bundles')

	// Included on all pages except the front page (to optimize front page load time).
	.scripts(
		['wwwroot/Scripts/moment-with-locales.js'],
		'wwwroot/bundles/shared/main.js',
	)

	// Included on all entry edit and create pages (album, artist, my settings etc.)
	.scripts(
		['wwwroot/Scripts/knockout-sortable.js'],
		'wwwroot/bundles/shared/edit.js',
	)

	.scripts(
		[
			'wwwroot/Scripts/jqwidgets27/jqxcore.js',
			'wwwroot/Scripts/jqwidgets27/jqxrating.js',
		],
		'wwwroot/bundles/jqxRating.js',
	)

	// Base CSS
	.less('wwwroot/Content/css.less', 'wwwroot/Content')

	.less('wwwroot/Content/embedSong.less', 'wwwroot/Content')
	.less('wwwroot/Content/Styles/DarkAngel.less', 'wwwroot/Content/Styles')
	.less('wwwroot/Content/Styles/discussions.less', 'wwwroot/Content/Styles')
	.less('wwwroot/Content/Styles/songlist.less', 'wwwroot/Content/Styles')

	// CSS for jqxRating
	.styles(
		['wwwroot/Scripts/jqwidgets27/styles/jqx.base.css'],
		'wwwroot/Scripts/jqwidgets27/styles/css.css',
	);

if (mix.inProduction()) {
	mix.scripts([], 'wwwroot/bundles/tests.js');
} else {
	mix.ts('wwwroot/Scripts/tests.ts', 'wwwroot/bundles');
}

if (mix.inProduction()) {
	mix.version();
}
