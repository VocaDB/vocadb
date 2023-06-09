import { forOwn } from 'lodash-es';

export class AjaxHelper {
	static createUrl = (params: {
		[key: string]: string[] | number[];
	}): string | null => {
		if (!params) return null;

		var par: string[] = [];

		forOwn(params, (val, key) => {
			par.push(
				key +
					'=' +
					(val as string[])
						.map((v) => encodeURIComponent(v || ''))
						.join('&' + key + '='),
			);
		});

		var query = par.join('&');
		return query;
	};

	static stringify = (params: any): string => {
		// Removes undefined.
		const filtered = Object.fromEntries(
			Object.entries(params).filter(([_, v]) => v != null),
		);
		// Code from: https://stackoverflow.com/questions/286141/remove-blank-attributes-from-an-object-in-javascript/30386744#30386744
		return getUrlString(filtered);
	};
}

const getUrlString = (
	params: any,
	keys: string[] = [],
	isArray = false,
): string => {
	const p = Object.keys(params)
		.map((key) => {
			let val = params[key];

			if (
				'[object Object]' === Object.prototype.toString.call(val) ||
				Array.isArray(val)
			) {
				if (Array.isArray(params)) {
					keys.push('');
				} else {
					keys.push(key);
				}
				return getUrlString(val, keys, Array.isArray(val));
			} else {
				let tKey = key;

				if (keys.length > 0) {
					const tKeys = isArray ? keys : [...keys, key];
					tKey = tKeys.reduce((str, k) => {
						return '' === str ? k : `${str}[${k}]`;
					}, '');
				}
				if (isArray) {
					return `${tKey}[]=${val}`;
				} else {
					return `${tKey}=${val}`;
				}
			}
		})
		.join('&');

	keys.pop();
	return p;
};
