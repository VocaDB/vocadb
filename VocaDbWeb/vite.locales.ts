import { readdirSync, readFileSync } from 'fs';
import { join } from 'path';
import type { Plugin } from 'vite';

// Bundles the translations in public/locales/<lng>/<ns>.json into one module per language,
// loaded with a single request before the first render.

const virtualId = 'virtual:locales';
const resolvedVirtualId = '\0' + virtualId;
const fallbackLng = 'en';

type Resources = { [key: string]: unknown };

const isObject = (value: unknown): value is Resources =>
	typeof value === 'object' && value !== null && !Array.isArray(value);

// Same precedence as i18next's fallback: a key present in the translation wins, even if empty.
const merge = (fallback: Resources, translation: Resources): Resources => {
	const result: Resources = { ...fallback };
	for (const [key, value] of Object.entries(translation)) {
		const fallbackValue = fallback[key];
		result[key] =
			isObject(value) && isObject(fallbackValue)
				? merge(fallbackValue, value)
				: value;
	}
	return result;
};

export const locales = (dir: string): Plugin => {
	const languages = (): string[] =>
		readdirSync(dir, { withFileTypes: true })
			.filter((entry) => entry.isDirectory())
			.map((entry) => entry.name)
			.sort();

	const readLanguage = (
		lng: string,
		addWatchFile: (file: string) => void,
	): Record<string, Resources> => {
		const result: Record<string, Resources> = {};
		for (const file of readdirSync(join(dir, lng)).sort()) {
			if (!file.endsWith('.json')) continue;
			const path = join(dir, lng, file);
			addWatchFile(path);
			result[file.slice(0, -'.json'.length)] = JSON.parse(
				readFileSync(path, 'utf8'),
			);
		}
		return result;
	};

	return {
		name: 'vocadb-locales',
		resolveId(id) {
			if (id === virtualId || id.startsWith(virtualId + '/')) {
				return '\0' + id;
			}
			return undefined;
		},
		load(id) {
			if (id === resolvedVirtualId) {
				const loaders = languages().map(
					(lng) =>
						`${JSON.stringify(lng)}: () => import(${JSON.stringify(
							`${virtualId}/${lng}`,
						)}).then((m) => m.default)`,
				);
				return `export default {\n${loaders.join(',\n')}\n};\n`;
			}

			if (!id.startsWith(resolvedVirtualId + '/')) return undefined;

			const lng = id.slice(resolvedVirtualId.length + 1);
			const addWatchFile = (file: string): void => this.addWatchFile(file);
			const translation = readLanguage(lng, addWatchFile);
			const resources: Record<string, Resources> = {};
			if (lng === fallbackLng) {
				Object.assign(resources, translation);
			} else {
				const fallback = readLanguage(fallbackLng, addWatchFile);
				for (const ns of new Set([
					...Object.keys(fallback),
					...Object.keys(translation),
				])) {
					resources[ns] = merge(fallback[ns] ?? {}, translation[ns] ?? {});
				}
			}
			return `export default JSON.parse(${JSON.stringify(
				JSON.stringify(resources),
			)});\n`;
		},
	};
};
