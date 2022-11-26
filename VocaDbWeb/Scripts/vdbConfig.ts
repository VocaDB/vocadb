interface VdbConfig {
	staticContentHost: string;
	amazonComAffiliateId: string;
	amazonJpAffiliateId: string;
	playAsiaAffiliateId: string;
	/** URL of the site path, for example "/" */
	baseAddress: string;
}

// HACK
const values = vdb.values as any;

export const vdbConfig: VdbConfig = {
	staticContentHost: values.staticContentHost,
	amazonComAffiliateId: values.amazonComAffiliateId,
	amazonJpAffiliateId: values.amazonJpAffiliateId,
	playAsiaAffiliateId: values.playAsiaAffiliateId,
	baseAddress: values.baseAddress,
};
