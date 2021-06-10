import RepositoryFactory from '@Repositories/RepositoryFactory';
import { Container } from 'inversify';

import HttpClient from './HttpClient';
import UrlMapperFactory from './UrlMapperFactory';

const container = new Container();
container.bind(HttpClient).toSelf().inSingletonScope();
container.bind(UrlMapperFactory).toSelf().inSingletonScope();
container.bind(RepositoryFactory).toSelf().inSingletonScope();

export { container };
