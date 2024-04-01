import Image from "next/image";
import LogoWhite from "../../public/VocaDB_Logo_White_Transparent_No_Outline.png";
import LogoDark from "../../public/VocaDB_Logo_Black_Transparent_No_Outline.png";
import Link from "next/link";

const Header = () => {
	return (
		<header className="sticky top-0 ml-3 w-full border-b">
			<div className="container h-12">
				<Link href="/">
					<Image
						src={LogoDark}
						className="dark:hidden"
						alt=""
						height={48}
					/>
					<Image
						src={LogoWhite}
						className="hidden dark:block"
						alt=""
						height={48}
					/>
				</Link>
			</div>
		</header>
	);
};

export default Header;

