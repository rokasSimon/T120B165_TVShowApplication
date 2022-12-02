import { useState } from "react";
import { Container } from "react-bootstrap";
import { redirect } from "react-router-dom";
import { useAuthenticationContext } from "../App";

function Login(props: any) {
    const { authenticationManager } = useAuthenticationContext();
    let [email, setEmail] = useState('');
    let [password, setPassword] = useState('');

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        authenticationManager.login({ Email: email, Password: password });

        return redirect('/');
    };

    return (
        <Container className="justify-content-center">
            <h1 className="text-center m-3">Login</h1>
            <form method="post" className="mw-100 w-75" onSubmit={e => handleSubmit(e)}>
                <div className="form-group mb-3">
                    <label htmlFor="email" className="form-label">Email</label>
                    <input type="text" className="form-control" name="email" id="email" onChange={e => setEmail(e.target.value)} />
                </div>
                <div className="form-group mb-3">
                    <label htmlFor="password" className="form-label">Password</label>
                    <input type="password" className="form-control" name="password" id="password" onChange={e => setPassword(e.target.value)} />
                </div>
                <input type="submit" className="btn btn-primary" />
            </form>
        </Container>
    );
}

export default Login;