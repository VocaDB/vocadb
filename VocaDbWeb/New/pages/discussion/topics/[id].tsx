import { useRouter } from 'next/router';

export default function TopicPage() {
	const router = useRouter();
	return <p>Topic: {router.query.id}</p>;
}

