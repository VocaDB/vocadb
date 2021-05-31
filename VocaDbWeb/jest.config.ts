import type { Config } from '@jest/types';
import { pathsToModuleNameMapper } from 'ts-jest/utils';

// `resolveJsonModule` in `tsconfig.json` needs to be set to `true`, so that `tsconfig.json` can be imported.
// See also: https://github.com/VocaDB/vocadb/pull/885
import { compilerOptions } from './tsconfig.json';

const config: Config.InitialOptions = {
	testRegex: 'Scripts/Tests/.*.test.ts$',
	preset: 'ts-jest',
	testEnvironment: 'jsdom',
	moduleNameMapper: pathsToModuleNameMapper(compilerOptions.paths, {
		prefix: '<rootDir>/',
	}),
	setupFilesAfterEnv: ['jest-expect-message'],
};
export default config;
