import { MantineProvider } from '@mantine/core';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';

import { routes } from './routes';

const NewApp = (): React.ReactElement => {
	const router = createBrowserRouter(routes);

	return (
		<MantineProvider
			withNormalizeCSS
			withGlobalStyles
			theme={{
				colors: {
					teal: [
						'#EDF8F7',
						'#CCEAE9',
						'#ACDDDB',
						'#8BD0CD',
						'#6AC3BF',
						'#4AB6B1',
						'#3B918E',
						'#2C6D6A',
						'#1D4947',
						'#0F2423',
					],
				},
				primaryColor: 'teal',
				colorScheme: 'dark',
			}}
		>
			<RouterProvider router={router} />
		</MantineProvider>
	);
};

export default NewApp;
