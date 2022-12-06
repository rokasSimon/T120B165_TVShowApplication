import { useEffect, useState } from "react";
import { Badge, Button, Card, CardImg, Col, ListGroup, ListGroupItem, Row, Spinner } from "react-bootstrap";
import { Link, useNavigate, useParams } from "react-router-dom";
import { formatRoute, Routes } from "../apiRoutes";
import { Role } from "../AuthenticationTypes";
import { useAuthState } from "../AuthProvider";
import { useAxiosContext } from "../AxiosInstanceProvider";
import { Series } from "../Models/SeriesModels";
import './CSS/Series.css';
import placeholderImage from '../Images/placeholder_img.jpg';
import { Genre } from "../Models/GenreModels";

type SeriesViewParams = {
    genreId: string,
    seriesId: string
}

type GenreLink = {
    genreName: string,
    genreRoute: string,
}

function SeriesView(props: any) {
    const authAxios = useAxiosContext();
    const auth = useAuthState();

    const [isLoading, setIsLoading] = useState(true);
    const [series, setSeries] = useState<Series | null>(null);
    const [genreLinks, setGenreLinks] = useState<GenreLink[]>([]);

    const params = useParams<SeriesViewParams>();
    const navigate = useNavigate();

    useEffect(() => {

        if (!params.genreId || !params.seriesId) {
            navigate('/error');
            return;
        }

        const genreId = parseInt(params.genreId);
        const seriesId = parseInt(params.seriesId);
        if (isNaN(genreId) || isNaN(seriesId)) {
            navigate('/error');
            return;
        }

        const fetchSeries = async (genreId: number, seriesId: number) => {
            const route = formatRoute(Routes.GetSeriesById, genreId.toString(), seriesId.toString());

            try {
                const response = await authAxios.get<Series>(route);

                let links = [...genreLinks];
                for (const genreRoute of response.data.genres) {
                    const fetchRoute = `/api${genreRoute}`;

                    const genreResponse = await authAxios.get<Genre>(fetchRoute);
                    links = [...links, { genreName: genreResponse.data.name, genreRoute: genreRoute }];
                }

                setGenreLinks(links);
                setSeries(response.data);
                setIsLoading(false);
            } catch (e) {
                console.error(e);
            }
        };

        fetchSeries(genreId, seriesId);

    }, []);

    let body;
    if (!auth.user) {
        body = undefined;
    } else if (isLoading) {
        body = <Spinner animation="border" />
    } else if (auth.user.Role == Role.Admin || auth.user.Role == Role.Poster) {
        body = <SeriesBasicView series={series!} seriesGenres={genreLinks} />
    } else {
        body = <SeriesBasicView series={series!} seriesGenres={genreLinks} />
    }

    return (
        <div>
            {body}
        </div>
    );
}

type SeriesProps = {
    series: Series,
    seriesGenres: GenreLink[],
}

function SeriesBasicView({ series, seriesGenres }: SeriesProps) {

    const handleImageError = (e: React.SyntheticEvent<HTMLImageElement, Event>) => {
        console.log('Replaced broken image');
        e.currentTarget.src = placeholderImage;
    };

    const directorItems = series.directors.map((director, idx) => {
        return (
            <li className="inline-item fw-light" key={idx}>{director}</li>
        );
    });

    const castItems = series.starringCast.map((actor, idx) => {
        return (
            <li className="inline-item fw-light" key={idx}>{actor}</li>
        );
    });

    const genreItems = seriesGenres.map((g, idx) => {
        return (
            <li className="inline-item fw-light" key={idx}>
                <Link to={g.genreRoute}>{g.genreName}</Link>
            </li>
        );
    });

    return (
        <div>
            <Col>
                <Row>
                    <h1>{series.name}</h1>
                    <hr className="hr" />
                </Row>
                <Row xl={12} md={6}>
                    <Col xl={4}>
                        <img className="large-img" src={series.coverImagePath ? series.coverImagePath : placeholderImage} onError={e => handleImageError(e)} />
                        <hr className="hr" />
                    </Col>
                    <Col xl={8}>
                        <Row>
                            <h4>Synopsis</h4>
                            <div className="jumbotron mt-3">
                                {series.description}
                            </div>
                            <hr className="hr" />
                        </Row>
                        <Row>
                            <ul className="list-group list-group-dark mb-3">
                                <li className="list-group-item d-flex flex-row">
                                    <p className="user-select-none fw-bold mb-0">Directors</p>
                                    <ul className="no-bullet">
                                        {directorItems}
                                    </ul>
                                </li>
                                <li className="list-group-item d-flex flex-row">
                                    <p className="user-select-none fw-bold mb-0">Cast</p>
                                    <ul className="no-bullet">
                                        {castItems}
                                    </ul>
                                </li>
                                <li className="list-group-item d-flex flex-row">
                                    <p className="user-select-none fw-bold mb-0">Genres</p>
                                    <ul className="no-bullet">
                                        {genreItems}
                                    </ul>
                                </li>
                            </ul>
                            <hr className="hr" />
                        </Row>
                    </Col>
                </Row>
            </Col>
        </div>    
    );
}

export default SeriesView;