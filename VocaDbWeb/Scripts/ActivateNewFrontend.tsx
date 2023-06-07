import { useEffect, useState } from 'react';

const ToggleNewFrontend = (): React.ReactElement => {
	const [betaFrontend, setBetaFrontend] = useState(
		localStorage.getItem('new_beta') === 'true',
	);

	useEffect(() => {
		localStorage.setItem('new_beta', betaFrontend ? 'true' : 'false');
	}, [betaFrontend]);

	return (
		<>
			<label className="switch">
				<input
					type="checkbox"
					checked={betaFrontend}
					onChange={(e): void => setBetaFrontend(e.target.checked)}
				/>{' '}
				<span className="slider">Enable new beta frontend</span>
			</label>
		</>
	);
};

export const Component = ToggleNewFrontend;

export default ToggleNewFrontend;
