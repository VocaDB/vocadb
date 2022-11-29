import { Layout } from '@/Components/Shared/Layout';

const HomeWiki = (): React.ReactElement => {
	return (
		<Layout pageTitle={undefined} ready={true} title={undefined}>
			{/* eslint-disable-next-line jsx-a11y/iframe-has-title */}
			<iframe
				src="https://wiki.vocadb.net/"
				style={{
					border: 0,
					width: '100%',
					height: 700,
				}}
			/>
		</Layout>
	);
};

export default HomeWiki;
