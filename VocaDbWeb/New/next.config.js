const withBundleAnalyzer = require("@next/bundle-analyzer")({
	enabled: process.env.ANALYZE === "true",
});

/** @type {import('next').NextConfig} */
const nextConfig = {
	images: {
		remotePatterns: [
			{
				protocol: "https",
				hostname: "vocadb.net",
				pathname: "/api/pvs/thumbnail",
			},
			{
				protocol: "https",
				hostname: "beta.vocadb.net",
				pathname: "/api/pvs/thumbnail",
			},
		],
	},
};

module.exports = withBundleAnalyzer(nextConfig);

