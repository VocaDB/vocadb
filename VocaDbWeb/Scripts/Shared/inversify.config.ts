import RepositoryFactory from '@Repositories/RepositoryFactory';
import { Container } from 'inversify';

import HttpClient from './HttpClient';
import UrlMapperFactory from './UrlMapperFactory';
import VocaDbContext from './VocaDbContext';
import VocaDbContextAccessor from './VocaDbContextAccessor';

const container = new Container();
container.bind(VocaDbContextAccessor).toSelf().inSingletonScope();
container
	.bind(VocaDbContext)
	.toDynamicValue(
		(context) => context.container.get(VocaDbContextAccessor).vocaDbContext,
	)
	.inSingletonScope();
container.bind(HttpClient).toSelf().inSingletonScope();
container.bind(UrlMapperFactory).toSelf().inSingletonScope();
container.bind(RepositoryFactory).toSelf().inSingletonScope();

export { container };
