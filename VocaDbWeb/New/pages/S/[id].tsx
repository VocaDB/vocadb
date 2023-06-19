import { useRouter } from 'next/router';

export default function Page() {
	const router = useRouter();
	return <p>Song: {router.query.id}</p>;
}

