import { apiPost } from '@/Helpers/FetchApiHelper';
import { getXsrfToken } from '@/Model/Repos/AntiforgeryRepository';
import {
	TextInput,
	PasswordInput,
	Checkbox,
	Anchor,
	Paper,
	Title,
	Text,
	Container,
	Group,
	Button,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import Link from 'next/link';
import { useRouter } from 'next/router';

interface FormValues {
	userName: string;
	password: string;
	submitting: boolean;
	keepLoggedIn: boolean;
	errors?: Record<string, string[]>;
}

export default function AuthenticationTitle() {
	const router = useRouter();
	const form = useForm<FormValues>({
		initialValues: {
			userName: '',
			password: '',
			submitting: false,
			keepLoggedIn: false,
		},
	});

	const onSubmit = async ({ userName, password, keepLoggedIn }: FormValues): Promise<void> => {
		getXsrfToken().then((token) => {
			apiPost(
				'/api/users/login',
				{
					userName,
					password,
					keepLoggedIn,
				},
				token
			)
				.then(() => router.push('/'))
				.catch((resp: Response) => {
					resp.json().then((data): void => {
						form.setFieldValue('errors', data.errors);
					});
				});
		});
	};

	return (
		<Container size={420} my={40}>
			<form onSubmit={form.onSubmit((values) => onSubmit(values))}>
				<Title
					align="center"
					sx={(theme) => ({
						fontFamily: `Greycliff CF, ${theme.fontFamily}`,
						fontWeight: 900,
					})}
				>
					Welcome back!
				</Title>
				<Text color="dimmed" size="sm" align="center" mt={5}>
					Do not have an account yet?{' '}
					<Anchor size="sm" component={Link} href="/User/register">
						Create account
					</Anchor>
				</Text>

				<Paper withBorder shadow="md" p={30} mt={30} radius="md">
					<TextInput
						label="Email"
						placeholder="email@vocadb.net"
						required
						{...form.getInputProps('userName')}
					/>
					<PasswordInput
						label="Password"
						placeholder="Your password"
						required
						mt="md"
						{...form.getInputProps('password')}
					/>
					<Group position="apart" mt="lg">
						<Checkbox
							label="Remember me"
							{...form.getInputProps('keepLoggedIn', { type: 'checkbox' })}
						/>
						<Anchor component="button" size="sm">
							Forgot password?
						</Anchor>
					</Group>
					<Button type="submit" fullWidth mt="xl">
						Sign in
					</Button>
				</Paper>
			</form>
		</Container>
	);
}

