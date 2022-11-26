// https://stackoverflow.com/questions/51504506/too-many-react-context-providers/58924810#58924810

interface ComposeProps {
	components: Array<
		React.JSXElementConstructor<React.PropsWithChildren<unknown>>
	>;
	children?: React.ReactNode;
}

export const Compose = ({
	components = [],
	children,
}: ComposeProps): React.ReactElement => {
	return (
		<>
			{components.reduceRight(
				(acc, Component) => (
					<Component>{acc}</Component>
				),
				children,
			)}
		</>
	);
};
