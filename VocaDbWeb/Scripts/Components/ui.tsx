import Alert from '@Bootstrap/Alert';
import React from 'react';
import toast from 'react-hot-toast';

export const showSuccessMessage = (message: string): void => {
	toast.custom(
		(t) => (
			<Alert
				variant="success"
				dismissible
				// TODO: Use toast.remove instead.
				onClose={(): void => toast.dismiss(t.id)}
			>
				{message}
			</Alert>
		),
		{ duration: 4000 },
	);
};
