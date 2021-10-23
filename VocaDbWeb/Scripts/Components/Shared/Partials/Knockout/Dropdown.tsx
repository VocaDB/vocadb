import ButtonGroup from '@Bootstrap/ButtonGroup';
import BootstrapDropdown from '@Bootstrap/Dropdown';
import React from 'react';

interface DropdownProps {
	items: Record<string, string>;
	value: string;
	onChange: (value: string) => void;
}

const Dropdown = React.memo(
	({ items, value, onChange }: DropdownProps): React.ReactElement => {
		return (
			<BootstrapDropdown as={ButtonGroup}>
				<BootstrapDropdown.Toggle>
					<span>{items[value]}</span> <span className="caret" />
				</BootstrapDropdown.Toggle>
				<BootstrapDropdown.Menu>
					{Object.entries(items).map(([k, v]) => (
						<BootstrapDropdown.Item onClick={(): void => onChange(k)} key={k}>
							{v}
						</BootstrapDropdown.Item>
					))}
				</BootstrapDropdown.Menu>
			</BootstrapDropdown>
		);
	},
);

export default Dropdown;
