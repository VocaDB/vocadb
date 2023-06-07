// https://stackoverflow.com/a/61903296.
export function getScript(url: string): Promise<void> {
	return new Promise((resolve, reject) => {
		const script = document.createElement('script') as any; /* TODO */
		script.src = url;
		script.async = true;

		script.onerror = reject;

		script.onload = script.onreadystatechange = function (): void {
			const loadState = this.readyState;

			if (loadState && loadState !== 'loaded' && loadState !== 'complete')
				return;

			script.onload = script.onreadystatechange = null;

			resolve();
		};

		document.head.appendChild(script);
	});
}
