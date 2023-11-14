import { EntryStatus } from "./entry"
import { OptionalGeoPointContract } from "./geopoint"
import { LocalizedStringWithIdContract } from "./localized"
import { ReleaseEventContract } from "./release-event"
import { WebLinkContract } from "./weblink"

export interface VenueForApiContract {
  additionalNames?: string
  address: string
  addressCountryCode: string
  coordinates: OptionalGeoPointContract
  deleted: boolean
  description: string
  events: ReleaseEventContract[]
  id: number
  name: string
  names?: LocalizedStringWithIdContract[]
  status: EntryStatus
  version?: number
  webLinks: WebLinkContract[]
}
