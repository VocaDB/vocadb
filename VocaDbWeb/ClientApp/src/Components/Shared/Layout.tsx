import React from 'react';
import GlobalSearchBox from './GlobalSearchBox';
import Footer from './Partials/Footer';
import LeftMenu from './Partials/LeftMenu';

interface LayoutProps {
  children: React.ReactNode;
  subtitle?: string;
  title?: string;
}

const Layout = ({
  children,
  subtitle,
  title,
}: LayoutProps): React.ReactElement => {
  return (
    <React.Fragment>
      <div className="navbar navbar-inverse navbar-fixed-top">
        <div className="navbar-inner">
          <div id="topBar" className="container">
            <GlobalSearchBox /* TODO */ />
          </div>
        </div>
      </div>

      <div className="container-fluid">
        <div className="row-fluid">
          <LeftMenu />

          <div className="span10 rightFrame well">
            {/* TODO */}

            {title && (
              <h1 className="page-title">
                {title}
                {subtitle && <small>&nbsp;{subtitle}</small>}
              </h1>
            )}

            {/* TODO */}

            {children}

            {/* TODO */}
          </div>
        </div>
      </div>

      <Footer />
    </React.Fragment>
  );
};

export default Layout;
