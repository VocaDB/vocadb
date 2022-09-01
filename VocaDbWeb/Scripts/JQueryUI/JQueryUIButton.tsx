import { BsPrefixRefForwardingComponent } from '@/Bootstrap/helpers';
import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

type JQueryUIButtonProps = {
	as: React.ElementType;
} & JQueryUI.ButtonOptions &
	React.HTMLAttributes<HTMLElement>;

const JQueryUIButton: BsPrefixRefForwardingComponent<
	'button',
	JQueryUIButtonProps
> = React.forwardRef<HTMLButtonElement, JQueryUIButtonProps>(
	({ as: Component, disabled, icons, text, ...props }, ref) => {
		const el = React.useRef<HTMLElement>(undefined!);
		useImperativeHandle<HTMLElement, HTMLElement>(ref, () => el.current);

		const options = React.useMemo(
			() => ({ disabled: disabled, icons: icons, text: text }),
			[disabled, icons, text],
		);

		React.useLayoutEffect(() => {
			const $el = $(el.current);
			$el.button(options);
			return (): void => $el.button('destroy');
		}, [options]);

		return <Component {...props} disabled={disabled} ref={el} />;
	},
);

export default JQueryUIButton;
