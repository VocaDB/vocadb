interface VdbConfig {
	staticContentHost: string;
	amazonComAffiliateId: string;
	amazonJpAffiliateId: string;
	playAsiaAffiliateId: string;
	/** URL of the site path, for example "/" */
	baseAddress: string;
}

export const vdbConfig: VdbConfig = {
	staticContentHost: 'https://localhost:44398/static' /* TODO */,
	amazonComAffiliateId: 'vocvocdat-20' /* TODO */,
	amazonJpAffiliateId: '' /* TODO */,
	playAsiaAffiliateId: '852809' /* TODO */,
	baseAddress: '/' /* TODO */,
};
