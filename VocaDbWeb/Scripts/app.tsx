import React from 'react';
import { render } from 'react-dom';
import { InertiaApp } from '@inertiajs/inertia-react';
import { InertiaProgress } from '@inertiajs/progress';
import './i18n';

InertiaProgress.init();

const app = document.getElementById('app');

render(
	<InertiaApp
		initialPage={app ? JSON.parse(app.dataset.page!) : "{}"}
		resolveComponent={name => import(`./Pages/${name}`).then(module => module.default)}
	/>,
	app
);
