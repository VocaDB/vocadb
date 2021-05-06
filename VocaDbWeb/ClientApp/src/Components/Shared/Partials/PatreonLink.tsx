import React from 'react';
import { useVocaDbPage } from '../../../VocaDbPageProvider';

const PatreonLink = (): React.ReactElement => {
  const { brandableStrings, config } = useVocaDbPage().props;

  return (
    <React.Fragment>
      <p>
        <small>{brandableStrings.layout.paypalDonateTitle}</small>
      </p>

      <a href={config.siteSettings.patreonLink} target="_blank">
        <img
          src={'/Content/patreon.png'}
          alt="Support on Patreon"
          title="Support on Patreon"
        />
      </a>
    </React.Fragment>
  );
};

export default PatreonLink;
