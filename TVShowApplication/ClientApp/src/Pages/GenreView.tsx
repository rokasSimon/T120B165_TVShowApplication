import { useEffect, useState } from "react";
import { Badge, Button, Card, CardImg, ListGroup, ListGroupItem, Spinner } from "react-bootstrap";
import { Link, useNavigate, useParams } from "react-router-dom";
import { formatRoute, Routes } from "../apiRoutes";
import { Role } from "../AuthenticationTypes";
import { useAuthState } from "../AuthProvider";
import { useAxiosContext } from "../AxiosInstanceProvider";
import { Genre, UpdateGenreDTO } from "../Models/GenreModels";
import { Series } from "../Models/SeriesModels";
import './CSS/GenreList.css';
import './CSS/Series.css';
import placeholderImage from '../Images/placeholder_img.jpg';
import { capText } from "../Utils";

type GenreViewParams = {
    genreId: string
}

function GenreView(props: any) {
    const authAxios = useAxiosContext();
    const auth = useAuthState();
    const [isLoading, setIsLoading] = useState(true);
    const [genre, setGenre] = useState<Genre | null>(null);
    const params = useParams<GenreViewParams>();

    useEffect(() => {

        if (!params.genreId) return;

        const genreId = parseInt(params.genreId);
        if (isNaN(genreId)) return;

        const fetchGenre = async (id: number) => {
            const route = formatRoute(Routes.GetGenre, id.toString());

            try {
                const response = await authAxios.get<Genre>(route);

                setGenre(response.data);
                setIsLoading(false);
            } catch (e) {
                console.error(e);
            }
        };

        fetchGenre(genreId);

    }, []);

    let body;
    if (!auth.user) {
        body = undefined;
    } else if (isLoading) {
        body = <Spinner animation="border" />
    } else if (auth.user.Role == Role.Admin) {
        body = <GenreAdminView genre={genre!} />
    } else {
        body = <GenreBasicView genre={genre!} />
    }

    return (
        <div>
            {body}
        </div>
    );
}

type GenreProps = {
    genre: Genre
}

function GenreBasicView({ genre }: GenreProps) {
    return (
        <div>
            <h1>{genre.name}</h1>
            <hr className="hr" />
            <div className="jumbotron mt-3">
                {genre.description}
            </div>
            <hr className="hr" />
            <h2>Series:</h2>
            <div className="card-grid">
                {genre.series.map((series, index) => {
                    return (
                        <SeriesItem key={index} fetchRoute={series} />
                    );
                })}
                {genre.series.length == 0 && <p className="text-center fw-light">No series in this genre</p>}
            </div>
        </div>    
    );
}

function GenreAdminView({ genre }: GenreProps) {

    const authAxios = useAxiosContext();

    const [description, setDescription] = useState<string>(genre.description);
    const handleSave = async () => {

        const updateRequest: UpdateGenreDTO = {
            description: description
        };

        const route = formatRoute(Routes.UpdateGenre, genre.id.toString());
        try {

            const response = await authAxios.patch(route, updateRequest);

            if (response.status != 200) {
                throw Error('Failed to update genre');
            }

        } catch (e) {
            console.error(e);

            return Promise.reject(e);
        }
    };

    return (
        <div>
            <h1>{genre.name}</h1>
            <hr className="hr" />
            <textarea className="form-control jumbotron mt-3" name="description" id="description" value={description} onChange={e => setDescription(e.target.value)} />
            <Button variant="success" onClick={e => handleSave()}>Save Changes</Button>
            <hr className="hr" />
            <h2>Series:</h2>
            <div className="card-grid">
                {genre.series.map((series, index) => {
                    return (
                        <SeriesItem key={index} fetchRoute={series} />
                    );
                })}
                {genre.series.length == 0 && <p className="text-center fw-light">No series in this genre</p>}
            </div>
        </div>
    );
}

type SeriesItemProps = {
    fetchRoute: string
}

function SeriesItem({ fetchRoute }: SeriesItemProps) {
    const authAxios = useAxiosContext();

    const [series, setSeries] = useState<Series | null>(null);

    useEffect(() => {

        const fetchSeries = async () => {

            const route = `/api${fetchRoute}`;

            try {

                const response = await authAxios.get<Series>(route);

                if (response.status !== 200) {
                    throw Error('Could not fetch series');
                }

                setSeries(response.data);

            } catch (e) {
                console.error(e);

                return Promise.reject(e);
            }

        };

        fetchSeries();

    }, []);

    const handleImageError = (e: React.SyntheticEvent<HTMLImageElement, Event>) => {
        console.log('Replaced broken image');
        e.currentTarget.src = placeholderImage;
    };

    const cardDescription = series?.description
        ? capText(series.description, 100)
        : undefined;

    const directorBadge = <Badge className="card-badge" key={'start'} bg="primary">Directors</Badge>
    const directors = series != null && series.directors.length != 0
        ? series.directors.map((director, idx) => {
            return (<Badge className="card-badge" key={idx} bg="secondary">{director}</Badge>);
        })
        : [<Badge className="card-badge" key={'none'} bg="warning">No known directors</Badge>];
    const cardDirectors = [directorBadge, ...directors];

    const castBadge = <Badge className="card-badge" key={'start'} bg="primary">Cast</Badge>
    const starringCast = series != null && series.starringCast.length != 0
        ? series.starringCast.map((cast, idx) => {
            return (<Badge className="card-badge" key={idx} bg="secondary">{cast}</Badge>);
        })
        : [<Badge className="card-badge" key={'none'} bg="warning">No known cast</Badge>];
    const cardCast = [castBadge, ...starringCast];

    const body = series === null
        ? <Spinner animation="border" />
        :
        <Card className="card-item" bg="dark" text="white">
            <Card.Img className="capped-img" variant="top" src={series.coverImagePath
                ? series.coverImagePath
                : placeholderImage
            } onError={e => handleImageError(e)} />
            <Card.Body>
                <div style={{ marginBottom: '1em' }}>
                    <Card.Title>{series.name}</Card.Title>
                    <Card.Text>
                        {cardDescription}
                    </Card.Text>
                </div>
                <ListGroup className="card-badges">
                    <ListGroupItem className="bg-dark">{cardDirectors}</ListGroupItem>
                    <ListGroupItem className="bg-dark">{cardCast}</ListGroupItem>
                </ListGroup>
            </Card.Body>
        </Card>

    return (
        <div className="series-item">
            <Link to={fetchRoute}>
                {body}
            </Link>
        </div>
    );
}
export default GenreView;