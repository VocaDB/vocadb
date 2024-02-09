import { SongWithPVAndVoteContract } from "@/types/api/song"
import { getBestThumbImageUrl } from "@/lib/utils"

import "react-multi-carousel/lib/styles.css"

import Link from "next/link"

import { Card, CardContent, CardFooter } from "@/components/ui/card"
import CustomImage from "@/components/image"

import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "./ui/carousel"

interface FeaturedSongCardProps {
  song: SongWithPVAndVoteContract
}

/* const FeaturedSongCard = ({ song }: FeaturedSongCardProps) => {
  const artist_split = song.artistString.split("feat.")
  const [dragging, setDragging] = useState(false)

  const onDragStart = () => {
    setDragging(true)
    window.addEventListener("mouseup", () => setDragging(false), { once: true })
  }

  return (
    <Link
      draggable={false}
      className="h-full w-full select-none sm:w-5/6"
      href={`/S/${song.id}`}
      style={{ pointerEvents: dragging ? "none" : "auto" }}
    >
      <Card onMouseDown={onDragStart} className="h-full cursor-pointer">
        <CardContent className="pt-5">
          <CustomImage
            className="pointer-events-none rounded-sm"
            width={320}
            height={180}
            src={getBestThumbImageUrl(song.pvs)}
            mode="crop"
            alt=""
          />
        </CardContent>
        <CardFooter className="flex-col">
          <p className="text-center font-bold">{song.name}</p>
          <p className="text-center font-light">{artist_split[0]}</p>
          {artist_split.length > 1 && (
            <p className="line-clamp-1 text-center font-light">{`feat. ${
              artist_split[1] ?? ""
            }`}</p>
          )}
        </CardFooter>
      </Card>
    </Link>
  )
}
 */
const FeaturedSongCard2 = ({ song }: FeaturedSongCardProps) => {
  return (
    <Link href={`/S/${song.id}`}>
      <Card>
        <CardContent>
          <CustomImage
            alt=""
            width={320}
            height={180}
            mode="crop"
            src={getBestThumbImageUrl(song.pvs)}
          />
        </CardContent>
      </Card>
    </Link>
  )
}

interface FeaturedSongsCarouselProps {
  songs: SongWithPVAndVoteContract[]
}

export const FeaturedSongsCarousel = ({
  songs,
}: FeaturedSongsCarouselProps) => {
  return (
    <Carousel opts={{ loop: true }} className="w-full max-w-4xl">
      <CarouselContent>
        {songs.map((song) => (
          <CarouselItem className="basis-1/3" key={song.id}>
            <FeaturedSongCard2 song={song} />
          </CarouselItem>
        ))}
      </CarouselContent>
      <CarouselPrevious />
      <CarouselNext />
    </Carousel>
  )
}
