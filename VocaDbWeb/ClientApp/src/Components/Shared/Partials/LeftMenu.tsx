import React from 'react';
import { useTranslation } from 'react-i18next';
import { useVocaDbPage } from '../../../VocaDbPageProvider';
import MainNavigationItems from './MainNavigationItems';
import PatreonLink from './PatreonLink';

interface BannerLinkProps {
  title: string;
  url: string;
  img: string;
}

const BannerLink = ({
  title,
  url,
  img,
}: BannerLinkProps): React.ReactElement => (
  <a href={url} onClick={(): void => {} /* TODO */}>
    <img src={'' /* TODO */} alt={title} title={title} />
  </a>
);

interface BannerProps {
  title: string;
  url: string;
  img: string;
}

const Banner = ({ title, url, img }: BannerProps): React.ReactElement => (
  <li>
    <BannerLink title={title} url={url} img={img} />
  </li>
);

interface SmallBannerProps {
  title: string;
  url: string;
  img: string;
}

const SmallBanner = ({
  title,
  url,
  img,
}: SmallBannerProps): React.ReactElement => (
  <div>
    <BannerLink title={title} url={url} img={img} />
  </div>
);

interface SocialLinkProps {
  title: string;
  url: string;
  img: string;
}

const SocialLink = ({
  title,
  url,
  img,
}: SocialLinkProps): React.ReactElement => (
  <BannerLink title={title} url={url} img={img} />
);

const LeftMenu = (): React.ReactElement => {
  const { t } = useTranslation(['ViewRes']);
  const {
    brandableStrings,
    config,
    appLinks,
    bannerUrl,
    bigBanners,
    smallBanners,
    socialLinks,
    login,
  } = useVocaDbPage().props;

  return (
    <div className="span2 menu">
      <div className="well">
        <a href={'/'}>
          <img
            src={bannerUrl /* TODO */}
            className="site-logo"
            alt={brandableStrings.layout.siteName}
            title={brandableStrings.layout.siteName}
          />
        </a>
        <p className="slogan">{/* TODO */}</p>
      </div>

      <div className="well sidebar-nav">
        <MainNavigationItems />
        {login.manager.isLoggedIn && (
          <React.Fragment>
            <br />
            <p className="user">
              {t('ViewRes:Layout.Welcome')}{' '}
              <a href={`/User/Profile/${login.user.name}`}>{login.user.name}</a>
            </p>
          </React.Fragment>
        )}
      </div>

      <div className="well">
        {appLinks && appLinks.length > 0 && (
          <div id="appLinks">
            {appLinks.map((link, i) => (
              <SmallBanner
                title={link.title}
                url={link.url}
                img={link.bannerImg}
                key={i}
              />
            ))}
          </div>
        )}

        <h4>{t('ViewRes:Layout.SocialSites')}</h4>
        <div id="socialSites">
          {socialLinks &&
            socialLinks.map((link, i) => (
              <SocialLink
                title={link.title}
                url={link.url}
                img={link.bannerImg}
                key={i}
              />
            ))}
        </div>

        <br />
        <h4>{t('ViewRes:Layout.RelatedSites')}</h4>
        <ul id="banners">
          {bigBanners &&
            bigBanners.map((link, i) => (
              <Banner
                title={link.title}
                url={link.url}
                img={link.bannerImg}
                key={i}
              />
            ))}
        </ul>
        <div id="banners-small">
          {smallBanners &&
            smallBanners.map((link, i) => (
              <SmallBanner
                title={link.title}
                url={link.url}
                img={link.bannerImg}
                key={i}
              />
            ))}
        </div>
        {config.siteSettings.patreonLink && (
          <React.Fragment>
            <hr />
            <PatreonLink />
          </React.Fragment>
        )}
        {/* TODO */}
      </div>

      {/* TODO */}
    </div>
  );
};

export default LeftMenu;
