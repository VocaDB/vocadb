import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

import useCloak from './useCloak';

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
	({ as: Component, disabled, icons, ...props }, ref) => {
		const el = React.useRef<HTMLElement>(undefined!);
		useImperativeHandle<HTMLElement, HTMLElement>(ref, () => el.current);
		useJQueryUIButton(el, { disabled: disabled, icons: icons });

		const cloak = useCloak();

		return <Component {...props} ref={el} style={cloak} />;
	},
);

export default JQueryUIButton;
