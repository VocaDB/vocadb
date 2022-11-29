import { Layout } from '@/Components/Shared/Layout';

const HomeChat = (): React.ReactElement => {
	const title = 'IRC chat'; /* LOC */
	return (
		<Layout pageTitle={title} ready={true} title={title}>
			{/* eslint-disable-next-line jsx-a11y/iframe-has-title */}
			<iframe
				src={`https://kiwiirc.com/client/${
					'' /* TODO: Config.SiteSettings.IRCUrl */
				}`}
				style={{
					border: 0,
					width: '100%',
					height: 600,
				}}
			/>
			<br />
			<br />
			For other clients, use the address{/* LOC */}{' '}
			<a href={`irc://${'' /* TODO: Config.SiteSettings.IRCUrl */}`}>
				irc://{'' /* TODO: Config.SiteSettings.IRCUrl */}
			</a>
			.
		</Layout>
	);
};

export default HomeChat;
