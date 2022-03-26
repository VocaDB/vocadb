import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

const useJQueryUICheckbox = (
	el: React.RefObject<any>,
	options: JQueryUI.ButtonOptions,
): void => {
	React.useLayoutEffect(() => {
		const $el = $(el.current);
		$el.button(options);
		return (): void => $el.button('destroy');
	});
};

type JQueryUICheckboxProps = {} & JQueryUI.ButtonOptions &
	React.InputHTMLAttributes<HTMLInputElement>;

const JQueryUICheckbox: BsPrefixRefForwardingComponent<
	'input',
	JQueryUICheckboxProps
> = React.forwardRef<HTMLInputElement, JQueryUICheckboxProps>(
	({ checked, children, disabled, id, icons, onChange, ...props }, ref) => {
		const el = React.useRef<HTMLInputElement>(undefined!);
		useImperativeHandle<HTMLInputElement, HTMLInputElement>(
			ref,
			() => el.current,
		);
		useJQueryUICheckbox(el, { disabled: disabled, icons: icons });

		return (
			<>
				<input
					{...props}
					type="checkbox"
					id={id}
					checked={checked}
					onChange={onChange}
					ref={el}
				/>
				<label htmlFor={id}>{children}</label>
			</>
		);
	},
);

export default JQueryUICheckbox;
