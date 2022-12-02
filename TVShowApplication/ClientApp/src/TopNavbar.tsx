import { Container, Nav, Navbar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { AuthenticationManager } from './authentication';

function TopNavbar(props: any) {
    const authenticationManager: AuthenticationManager = props.authenticationManager;

    let userSection;
    if (authenticationManager.isAuthenticated()) {
        userSection =
            <Nav>
                {/*<Link className="link-light mx-1" to={'/user/profile'}>Profile</Link>*/}
                <Link className="link-light mx-1" to={'/user/signout'}>Sign Out</Link>
            </Nav>
    } else {
        userSection =
            <Nav>
                <Link className="link-light mx-1" to={'/user/register'}>Register</Link>
                <Link className="link-light mx-1" to={'/user/login'}>Login</Link>
            </Nav>
    }

    return (
        <Navbar collapseOnSelect expand="lg" variant="dark" bg="dark" className="border-bottom border-secondary">
            <Container>
                <Navbar.Brand href='/'>TV Show Application</Navbar.Brand>
                <Navbar.Toggle />
                <Navbar.Collapse>
                    <Nav className="me-auto">
                        <Link className="link-light mx-1" to={'/genre'}>Genres</Link>
                        <Link className="link-light mx-1" to={'/series'}>Series</Link>
                    </Nav>
                    {userSection}
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
}

export default TopNavbar;