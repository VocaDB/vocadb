import RepositoryFactory from '@Repositories/RepositoryFactory';
import { Container } from 'inversify';

import HttpClientFactory from './HttpClientFactory';

const container = new Container();
container.bind(HttpClientFactory).toSelf().inSingletonScope();
container.bind(RepositoryFactory).toSelf().inSingletonScope();

export { container };
