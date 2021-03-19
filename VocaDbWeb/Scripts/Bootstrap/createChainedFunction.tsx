// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/createChainedFunction.tsx#L10

function createChainedFunction(...funcs) {
	return funcs
		.filter((f) => f != null)
		.reduce((acc, f) => {
			if (typeof f !== 'function') {
				throw new Error(
					'Invalid Argument Type, must only provide functions, undefined, or null.',
				);
			}

			if (acc == null) return f;

			return function chainedFunction(...args) {
				acc.apply(this, args);
				f.apply(this, args);
			}
		}, null);
}

export default createChainedFunction;
