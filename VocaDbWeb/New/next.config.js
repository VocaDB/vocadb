const withBundleAnalyzer = require('@next/bundle-analyzer')({
	enabled: process.env.ANALYZE === 'true',
});

module.exports = withBundleAnalyzer({
	output: 'standalone',
	reactStrictMode: false,
	eslint: {
		ignoreDuringBuilds: true,
	},
	images: {
		remotePatterns: [
			{ protocol: 'https', hostname: 'i1.ytimg.com' },
			{ protocol: 'https', hostname: 'nicovideo.cdn.nimg.jp' },
			{ protocol: 'https', hostname: 'img.cdn.nimg.jp' },
		],
	},
});

