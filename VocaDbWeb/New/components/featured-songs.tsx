"use client"

import Carousel from "react-multi-carousel"

import { SongWithPVAndVoteContract } from "@/types/api/song"
import { getBestThumbImageUrl } from "@/lib/utils"

import "react-multi-carousel/lib/styles.css"

import Link from "next/link"

import { Card, CardContent, CardFooter } from "@/components/ui/card"
import CustomImage from "@/components/image"

interface FeaturedSongCardProps {
  song: SongWithPVAndVoteContract
}

const FeaturedSongCard = ({ song }: FeaturedSongCardProps) => {
  const artist_split = song.artistString.split("feat.")

  return (
    <Link className="h-full sm:w-5/6 w-full" href={`/S/${song.id}`}>
      <Card className="h-full hover:bg-gray-100 cursor-pointer">
        <CardContent className="pt-5">
          <CustomImage
            className="rounded-sm"
            width={320}
            height={180}
            src={getBestThumbImageUrl(song.pvs)}
            mode="crop"
            alt=""
          />
        </CardContent>
        <CardFooter className="flex-col">
          <p className="font-bold text-center">{song.name}</p>
          <p className="font-light text-center">{artist_split[0]}</p>
          {artist_split.length > 1 && (
            <p className="font-light text-center line-clamp-1">{`feat. ${
              artist_split[1] ?? ""
            }`}</p>
          )}
        </CardFooter>
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
  const responsive = {
    desktop: {
      breakpoint: { max: 3000, min: 1024 },
      items: 3,
    },
    tablet: {
      breakpoint: { max: 1024, min: 464 },
      items: 2,
    },
    mobile: {
      breakpoint: { max: 640, min: 0 },
      items: 1,
    },
  }

  return (
    <Carousel
      itemClass="flex flex-col items-center"
      infinite
      responsive={responsive}
    >
      {songs.map((song) => (
        <FeaturedSongCard song={song} key={song.id} />
      ))}
    </Carousel>
  )
}
