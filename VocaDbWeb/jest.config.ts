import type { Config } from '@jest/types';
import { pathsToModuleNameMapper } from 'ts-jest';

// `resolveJsonModule` in `tsconfig.json` needs to be set to `true`, so that `tsconfig.json` can be imported.
// See also: https://github.com/VocaDB/vocadb/pull/885
import { compilerOptions } from './tsconfig.json';

const config: Config.InitialOptions = {
	testRegex: 'Scripts/Tests/.*\\.test\\.tsx?$',
	transform: {
		'^.+\\.[tj]sx?$': [
			'ts-jest',
			{
				tsconfig: { esModuleInterop: true, allowJs: true, module: 'CommonJS' },
			},
		],
	},
	transformIgnorePatterns: ['node_modules/(?!lodash-es/)'],
	testEnvironment: 'jsdom',
	moduleNameMapper: pathsToModuleNameMapper(compilerOptions.paths, {
		prefix: '<rootDir>/',
	}),
	setupFiles: ['./Scripts/Tests/setup.js'],
	setupFilesAfterEnv: ['jest-expect-message', './Scripts/ArrayExtensions.ts'],
};
export default config;
