const mix = require('laravel-mix');
const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

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
			chunkFilename: 'bundles/[name].[chunkhash].js',
		},
		plugins: [
			new CleanWebpackPlugin({
				cleanOnceBeforeBuildPatterns: ['bundles/**/*'],
			}),
		],
	})
	.options({
		processCssUrls: false,
	})
	.alias({
		'@': path.join(__dirname, 'Scripts'),
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
	.scripts(['Scripts/VocaDb.js'], 'wwwroot/bundles/VocaDB.js')

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
	)

	.ts('Scripts/index.tsx', 'wwwroot/bundles')
	.react();

if (mix.inProduction()) {
	mix.version();
}
