import useVocaDbPage from '@Components/useVocaDbPage';
import React from 'react';

const PatreonLink = (): React.ReactElement => {
  const { brandableStrings, config } = useVocaDbPage().props;

  return (
    <>
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
    </>
  );
};

export default PatreonLink;
