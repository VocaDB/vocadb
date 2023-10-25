import { register } from "swiper/element/bundle"

import "swiper/css"

register()

export const FeaturedSongsCarousel = () => {
  return (
    <swiper-container slides-per-view="3" navigation="true" pagination="true">
      <swiper-slide>Slide 1</swiper-slide>
      <swiper-slide>Slide 2</swiper-slide>
      <swiper-slide>Slide 3</swiper-slide>
      ...
    </swiper-container>
  )
}
