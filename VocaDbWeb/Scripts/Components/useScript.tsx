import React from 'react';

// Code from: https://stackoverflow.com/questions/34424845/adding-script-tag-to-react-jsx/34425083#34425083
const useScript = (src: string): void => {
	React.useEffect(() => {
		const script = document.createElement('script');

		script.src = src;
		script.async = true;

		document.body.appendChild(script);

		return (): void => {
			document.body.removeChild(script);
		};
	}, [src]);
};

export default useScript;
