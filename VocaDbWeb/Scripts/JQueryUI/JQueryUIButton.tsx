import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

const useJQueryUIButton = (
	el: React.RefObject<any>,
	options: JQueryUI.ButtonOptions,
): void => {
	React.useEffect(() => {
		const $el = $(el.current);
		$el.button(options);
		return (): void => $el.button('destroy');
	});
};

type JQueryUIButtonProps = {
	as: React.ElementType;
} & JQueryUI.ButtonOptions &
	React.HTMLAttributes<HTMLElement>;

const JQueryUIButton: BsPrefixRefForwardingComponent<
	'button',
	JQueryUIButtonProps
> = React.forwardRef<HTMLButtonElement, JQueryUIButtonProps>(
	({ as: Component, icons, ...props }, ref) => {
		const el = React.useRef<HTMLElement>(undefined!);
		useImperativeHandle<HTMLElement, HTMLElement>(ref, () => el.current);
		useJQueryUIButton(el, { icons: icons });

		return <Component ref={el} {...props} />;
	},
);

export default JQueryUIButton;
