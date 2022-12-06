import { Link } from "./Other"

type Series = {
    id: number,
    name: string,
    description: string | null,
    coverImagePath: string | null,
    directors: string[],
    starringCast: string[],
    poster: Link,
    genres: string[],
    reviews: string[],
}

type CreateSeriesDTO = {
    name: string,
    description: string | null,
    coverImagePath: string | null,
    directors: string[],
    starringCast: string[],
    poster: number,
    genres: number[]
}

type UpdateSeriesDTO = {
    name: string,
    description: string,
    directors: string[],
    starringCast: string[]
}

export type { Series, CreateSeriesDTO, UpdateSeriesDTO }