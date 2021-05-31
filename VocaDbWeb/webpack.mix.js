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
		'@DataContracts': path.join(__dirname, 'Scripts/DataContracts'),
		'@Helpers': path.join(__dirname, 'Scripts/Helpers'),
		'@KnockoutExtensions': path.join(__dirname, 'Scripts/KnockoutExtensions'),
		'@Models': path.join(__dirname, 'Scripts/Models'),
		'@Repositories': path.join(__dirname, 'Scripts/Repositories'),
		'@Shared': path.join(__dirname, 'Scripts/Shared'),
		'@ViewModels': path.join(__dirname, 'Scripts/ViewModels'),
	})
	.eslint({
		fix: false,
		extensions: ['ts', 'tsx'],
	})
	.babelConfig({
		plugins: ['@babel/plugin-syntax-dynamic-import'],
	})

	.extract(['highcharts'], 'highcharts')
	.extract()

	.js('Scripts/libs.js', 'wwwroot/bundles/shared')

	// SHARED BUNDLES
	// Legacy common scripts - should be phased out
	.ts('Scripts/vdb.ts', 'wwwroot/bundles')

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
	mix.version();
}
