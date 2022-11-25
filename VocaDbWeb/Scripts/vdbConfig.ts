interface VdbConfig {
	/** URL of the site path, for example "/" */
	baseAddress: string;
}

export const vdbConfig: VdbConfig = {
	baseAddress: (vdb.values as any).baseAddress,
};
