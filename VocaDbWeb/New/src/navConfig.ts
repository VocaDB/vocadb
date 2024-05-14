import {
	CirclePlay,
	Medal,
	Disc,
	ListMusic,
	Music,
	MicVocal,
	Disc3,
	Calendar,
	Tag,
	MessagesSquare,
	BookOpen,
	LogIn,
	UserPlus,
	AreaChart,
} from "lucide-react";

export const navbarConfig = [
	{
		title: "Discover",
		links: [
			{
				title: "Recent PVs",
				href: "/search?searchType=Song&sort=AdditionDate&onlyWithPVs=true",
				icon: CirclePlay,
			},
			{
				title: "Top rated songs",
				href: "/song/Rankings?dateFilterType=CreateDate&durationHours=168",
				icon: Medal,
			},
			{
				title: "Top rated albums",
				href: "/search?searchType=Album&sort=RatingAverage",
				icon: Disc,
			},
			{
				title: "Featured song lists",
				href: "/songlist/featured",
				icon: ListMusic,
			},
		],
	},
	{
		title: "Library",
		links: [
			{
				title: "Songs",
				href: "/search?searchType=Song",
				icon: Music,
			},
			{
				title: "Artists",
				href: "/search?searchType=Artist",
				icon: MicVocal,
			},
			{
				title: "Albums",
				href: "/search?searchType=Album",
				icon: Disc3,
			},
			{
				title: "Events",
				href: "/search?searchType=ReleaseEvent",
				icon: Calendar,
			},
			{
				title: "Tags",
				href: "/tag",
				icon: Tag,
			},
		],
	},
	{
		title: "Documentation",
		links: [
			{
				title: "Discussions",
				href: "/discussion",
				icon: MessagesSquare,
			},
			{
				title: "Wiki",
				href: "//wiki.vocadb.net",
				icon: BookOpen,
			},
			{
				title: "Stats",
				href: "/stats",
				icon: AreaChart,
			},
		],
	},
	{
		title: "User",
		links: [
			{
				title: "Login",
				href: "/login",
				icon: LogIn,
			},
			{
				title: "Register",
				href: "/user/create", // TODO: /register URL
				icon: UserPlus,
			},
		],
	},
];
