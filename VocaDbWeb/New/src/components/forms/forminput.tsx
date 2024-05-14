import { cookies } from "next/headers";

export const FormInput = ({
	onChange,
}: {
	onChange?: () => any;
	name: string;
}) => {
	return cookies().get("theme")?.value === "classic" ? (
		<input id="hey1" onChange={onChange} />
	) : (
		<input id="hey2" onChange={onChange} />
	);
};

