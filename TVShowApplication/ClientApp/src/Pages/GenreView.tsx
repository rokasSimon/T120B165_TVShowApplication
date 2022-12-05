import { useEffect, useState } from "react";
import { Button, Spinner } from "react-bootstrap";
import { Link, useParams } from "react-router-dom";
import { formatRoute, Routes } from "../apiRoutes";
import { Role } from "../AuthenticationTypes";
import { useAuthState } from "../AuthProvider";
import { useAxiosContext } from "../AxiosInstanceProvider";
import { Genre, UpdateGenreDTO } from "../Models/GenreModels";
import './CSS/GenreList.css';

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
            <h2>Series in this genre:</h2>
            {genre.seriesLinks && genre.seriesLinks.map((series, index) => {
                return (
                    <div key={index}>
                        <Link to={series} />
                    </div>
                );
            })}
            {!genre?.seriesLinks && <p className="text-center fw-light">No series in this genre</p>}
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
            <h2>Series in this genre:</h2>
            {genre.seriesLinks && genre.seriesLinks.map((series, index) => {
                return (
                    <div key={index}>
                        <Link to={series} />
                    </div>
                );
            })}
            {!genre?.seriesLinks && <p className="text-center fw-light">No series in this genre</p>}
        </div>
    );
}

export default GenreView;