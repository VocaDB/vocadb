// Code from: https://stackoverflow.com/a/61903296.
export const getScript = (url: string): Promise<void> =>
	new Promise((resolve, reject) => {
		const script = document.createElement('script');
		script.src = url;
		script.async = true;

		script.onerror = reject;

		// @ts-ignore
		script.onload = script.onreadystatechange = function (): void {
			// @ts-ignore
			const loadState = this.readyState;

			if (loadState && loadState !== 'loaded' && loadState !== 'complete')
				return;

			// @ts-ignore
			script.onload = script.onreadystatechange = null;

			resolve();
		};

		document.head.appendChild(script);
	});
