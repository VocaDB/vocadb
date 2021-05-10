import Button from '@Bootstrap/Button';
import Dropdown from '@Bootstrap/Dropdown';
import useVocaDbPage from '@Components/useVocaDbPage';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import Autocomplete from '@JQueryUI/Autocomplete';
import EntryType from '@Models/EntryType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import React from 'react';
import { useTranslation } from 'react-i18next';

import MainNavigationItems from './Partials/MainNavigationItems';

const GlobalSearchBox = (): React.ReactElement => {
  const { t } = useTranslation([
    'Resources',
    'ViewRes',
    'ViewRes.User',
    'VocaDb.Web.Resources.Domain',
  ]);
  const { brandableStrings, login } = useVocaDbPage().props;

  const allObjectTypes = [
    EntryType.Undefined,
    EntryType.Artist,
    EntryType.Album,
    EntryType.Song,
    EntryType.Tag,
    EntryType.User,
    EntryType.ReleaseEvent,
    EntryType.SongList,
  ]; /* TODO */
  const entryType = EntryType.Undefined; /* TODO */
  const entryTypeName = t(
    `VocaDb.Web.Resources.Domain:EntryTypeNames.${EntryType[entryType]}`,
  );
  const reportCount = 123; /* TODO */
  const hasNotifications = reportCount > 0;
  const isLoaded = false; /* TODO */
  const unreadMessages: UserMessageSummaryContract[] = []; /* TODO */

  const ensureMessagesLoaded = (): void => {}; /* TODO */

  return (
    <form
      action="/Home/GlobalSearch"
      method="post"
      className="navbar-form form-inline pull-left navbar-search"
      id="globalSearchBox"
    >
      {/* TODO */}

      <a
        className="btn btn-navbar"
        data-toggle="collapse"
        data-target=".nav-collapse"
      >
        <span className="icon-bar"></span>
        <span className="icon-bar"></span>
        <span className="icon-bar"></span>
      </a>

      <div className="brand hidden-phone">
        <a href={'/'}>{brandableStrings.siteName}</a>
      </div>

      <div className="input-prepend input-append navbar-searchBox">
        <Dropdown>
          <Dropdown.Toggle variant="info" href="#">
            <span>{entryTypeName}</span> <span className="caret"></span>
          </Dropdown.Toggle>
          <Dropdown.Menu>
            {allObjectTypes.map((entryType) => (
              /* REVIEW */
              <Dropdown.Item /* TODO */ key={entryType}>
                {t(
                  `VocaDb.Web.Resources.Domain:EntryTypeNames.${EntryType[entryType]}`,
                )}
              </Dropdown.Item>
            ))}
          </Dropdown.Menu>
          {/* TODO */}
        </Dropdown>
        <Autocomplete
          type="text"
          name="globalSearchTerm"
          id="globalSearchTerm"
          className="globalSearchBox search-query"
          size={50}
          placeholder={t('ViewRes:Shared.Search')}
          maxLength={255}
        />
        <Button type="submit" variant="info" title={t('ViewRes:Shared.Search')}>
          <i className="icon-search"></i>
        </Button>
      </div>

      <Dropdown className="navbar-languageBar">
        <Dropdown.Toggle variant="info" href="#" className="navbar-languageBtn">
          <i className="icon-user"></i>{' '}
          <span className="visible-desktop">{t('ViewRes:Layout.Account')}</span>
          {hasNotifications && (
            <>
              {' '}
              <span className="badge badge-small badge-important">!</span>
            </>
          )}{' '}
          <span className="caret"></span>
        </Dropdown.Toggle>

        <Dropdown.Menu>
          {!login.manager.isLoggedIn ? (
            <>
              <Dropdown.Item href={'/User/Login'} /* TODO */>
                {t('ViewRes:Layout.LogIn')}
              </Dropdown.Item>
              <Dropdown.Item href={'/User/Create'}>
                {t('ViewRes:Layout.Register')}
              </Dropdown.Item>
            </>
          ) : (
            <>
              <Dropdown.Item href={`/User/Profile/${login.manager.name}`}>
                {t('ViewRes.User:MySettings.Profile')}
              </Dropdown.Item>
              <Dropdown.Item
                href={`/User/Profile/${login.manager.name}#Albums`}
              >
                {t('ViewRes:TopBar.MyAlbums')}
              </Dropdown.Item>
              <Dropdown.Item href={`/User/Profile/${login.manager.name}#Songs`}>
                {t('ViewRes:TopBar.MySongs')}
              </Dropdown.Item>
              <Dropdown.Item href={'/User/MySettings'}>
                {t('ViewRes.User:Details.MySettings')}
              </Dropdown.Item>
              {login.canManageEntryReports && (
                <Dropdown.Item href={'/Admin/ViewEntryReports'}>
                  {t('ViewRes:TopBar.EntryReports')}
                  {reportCount > 0 && (
                    <>
                      {' '}
                      <span className="badge badge-small badge-important">
                        {reportCount}
                      </span>
                    </>
                  )}
                </Dropdown.Item>
              )}
              <Dropdown.Item href={'/User/Logout'}>
                {t('ViewRes:Layout.LogOut')}
              </Dropdown.Item>
            </>
          )}
          <Dropdown.Divider />
          <Dropdown.Header>
            {t('ViewRes.User:MySettings.DefaultLanguageSelection')}
          </Dropdown.Header>
          {Object.keys(ContentLanguagePreference)
            .filter(
              (key) =>
                !isNaN(
                  Number(
                    ContentLanguagePreference[
                      key as keyof typeof ContentLanguagePreference
                    ],
                  ),
                ),
            )
            .map((lp) => (
              <Dropdown.Item href="#" /* TODO */ key={lp}>
                {lp === login.manager.languagePreference ? (
                  <i className="menuIcon icon-ok"></i>
                ) : (
                  <i className="menuIcon icon-"></i>
                )}{' '}
                {t(`Resources:ContentLanguageSelectionNames.${lp}`)}
              </Dropdown.Item>
            ))}
        </Dropdown.Menu>
      </Dropdown>

      {login.manager.isLoggedIn && (
        <Dropdown
          className="navbar-languageBar"
          onToggle={ensureMessagesLoaded}
        >
          <Dropdown.Toggle
            variant="info"
            href="#"
            className="navbar-languageBtn"
          >
            <i className="icon-envelope"></i>
            {login.user!.unreadMessagesCount > 0 && (
              <>
                {' '}
                <span className="badge badge-small badge-important">
                  {login.user!.unreadMessagesCount}
                </span>
              </>
            )}{' '}
            <span className="caret"></span>
          </Dropdown.Toggle>

          <Dropdown.Menu>
            {login.user!.unreadMessagesCount > 0 && !isLoaded && (
              <Dropdown.ItemText>
                {t('ViewRes:Shared.Loading')}
              </Dropdown.ItemText>
            )}
            {unreadMessages.map((unreadMessage) => (
              <Dropdown.Item
                href={`/User/Messages?messageId=${unreadMessage.id}`}
                key={unreadMessage.id}
              >
                <div className="media">
                  <div className="pull-left media-image-usermessage">
                    {/* TODO */}
                    <img src="/Content/vocadb_40.png" alt="Notification" />
                  </div>
                  <div className="media-body media-body-usermessage">
                    <span /* TODO */></span>
                    <small>{unreadMessage.createdFormatted}</small>
                    <br />
                    <span>{unreadMessage.subject}</span>
                  </div>
                </div>
              </Dropdown.Item>
            ))}
            {isLoaded && unreadMessages.length === 0 && (
              <Dropdown.ItemText>
                {t('ViewRes:TopBar.NoUnreadMessages')}
              </Dropdown.ItemText>
            )}
            <Dropdown.Divider />
            <Dropdown.Item href={'/User/Messages'}>
              {t('ViewRes:TopBar.ViewAllMessages')}
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      )}

      <div className="nav-collapse collapse">
        <MainNavigationItems />
      </div>
    </form>
  );
};

export default GlobalSearchBox;
