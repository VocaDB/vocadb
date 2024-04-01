import { navbarConfig } from "@/navConfig";
import { LucideIcon } from "lucide-react";
import Link from "next/link";
import { buttonVariants } from "./ui/button";
import { ScrollArea } from "./ui/scroll-area";
import { cn } from "@/lib/utils";

interface NavbarLinkProps {
	icon: LucideIcon;
	href: string;
	title: string;
}

const NavbarLink: React.FC<NavbarLinkProps> = ({ icon: Icon, href, title }) => {
	return (
		<Link
			className={cn(
				buttonVariants({ variant: "ghost" }),
				"w-full justify-start",
			)}
			href={href}
		>
			<Icon className="mr-2 size-4" />
			{title}
		</Link>
	);
};

const Navbar = () => {
	return (
		<aside className="fixed top-12 hidden h-[calc(100vh-3rem)] md:sticky md:block">
			<ScrollArea className="h-full border-r">
				<div className="space-y-2 px-2 py-4">
					{navbarConfig.map((item) => (
						<div key={item.title} className="py-2">
							<h2 className="mb-2 px-4 text-lg font-semibold tracking-tight">
								{item.title}
							</h2>
							{item.links.map((link) => (
								<NavbarLink
									key={link.href}
									icon={link.icon}
									href={link.href}
									title={link.title}
								/>
							))}
						</div>
					))}
				</div>
			</ScrollArea>
		</aside>
	);
};

export default Navbar;
