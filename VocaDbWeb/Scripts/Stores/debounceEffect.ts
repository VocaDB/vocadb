import { IReactionPublic } from 'mobx';

// Code from: https://github.com/mobxjs/mobx/issues/1956#issuecomment-534864621
export default function debounceEffect<T>(
	effect: (arg: T, r: IReactionPublic) => void,
	debounceMs: number,
): (arg: T, prev: T, r: IReactionPublic) => void {
	let timer: NodeJS.Timeout;
	return (arg: T, prev: T, r: IReactionPublic): void => {
		clearTimeout(timer);
		timer = setTimeout(() => effect(arg, r), debounceMs);
	};
}
