import React from 'react';

const useCloak = (): { display: string } | undefined => {
	const [cloak, setCloak] = React.useState(true);

	React.useEffect(() => setCloak(false), [setCloak]);

	return cloak ? { display: 'none' } : undefined;
};

export default useCloak;
