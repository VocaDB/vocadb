import { Page } from "@inertiajs/inertia";
import { usePage } from "@inertiajs/inertia-react";
import React, { Fragment, ReactElement } from "react";
import VocaDbPageProps from "../VocaDbPageProps";

const PatreonLink = (): ReactElement => {
	const {
		brandableStrings,
		config,
	} = usePage<Page<VocaDbPageProps>>().props;

	return (
		<Fragment>
			<p>
				<small>
					{brandableStrings.layout.paypalDonateTitle}
				</small>
			</p>

			<a href={config.siteSettings.patreonLink} target="_blank">
				<img src={'/Content/patreon.png'} alt="Support on Patreon" title="Support on Patreon" />
			</a>
		</Fragment>
	);
};

export default PatreonLink;
