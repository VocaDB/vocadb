import { SongForm } from "./form";
import { FormInput } from "./forminput";

export default function ExamplePage() {
	return (
		<SongForm>
			<FormInput name="Hey1" />
			<FormInput name="Hey2" />
		</SongForm>
	);
}

