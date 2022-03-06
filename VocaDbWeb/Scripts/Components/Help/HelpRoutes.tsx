import React from 'react';

const HelpIndexEn = React.lazy(() => import('./HelpIndex'));
const HelpIndexJa = React.lazy(() => import('./HelpIndex.ja'));
const HelpIndexZhHans = React.lazy(() => import('./HelpIndex.zh-Hans'));

const HelpRoutes = (): React.ReactElement => {
	if (vdb.values.externalHelpPath) {
		return (
			// eslint-disable-next-line jsx-a11y/iframe-has-title
			<iframe
				src={vdb.values.externalHelpPath}
				style={{ width: '100%', height: '1200px' }}
			/>
		);
	}

	const locale = new Intl.Locale(vdb.values.uiCulture);

	switch (locale.language) {
		case 'ja':
			return <HelpIndexJa />;

		case 'zh':
			return <HelpIndexZhHans />;

		default:
			return <HelpIndexEn />;
	}
};

export default HelpRoutes;
