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
			{ protocol: 'http', hostname: 'i2.hdslb.com' }, // TODO: Ensure that we never actually use http
			{ protocol: 'https', hostname: 'i2.hdslb.com' },
			{ protocol: 'https', hostname: 'vocadb.net' },
			{ protocol: 'http', hostname: '127.0.0.1' },
		],
	},
});

