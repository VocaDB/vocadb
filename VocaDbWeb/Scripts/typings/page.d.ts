declare function page(route: string);
declare function page(
	route: string,
	callback: (context: page.PageContext) => void,
);

declare module page {
	interface PageContext {
		params: any;
	}

	export function start();
}
