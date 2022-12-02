import { useState } from 'react';
import { Container } from 'react-bootstrap';
import TopNavbar from './TopNavbar';
import Footer from './Footer';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';
import { AuthenticationContext, LocalStorageAuthenticationManager, User } from './authentication';
import { Outlet, useOutletContext } from 'react-router-dom';

function App() {
    let [userData, setUser] = useState<User | null>(null);

    const authenticationManager = new LocalStorageAuthenticationManager(userData, setUser);
    authenticationManager.loadFromLocalStorage();

  return (
      <div className="App d-flex flex-column">
          <TopNavbar authenticationManager={authenticationManager} />
          <Container className="bg-dark flex-fill">
              <Outlet context={{ authenticationManager }} />
          </Container>
          <Footer />
    </div>
  );
}

export function useAuthenticationContext() {
    return useOutletContext<AuthenticationContext>();
}

export default App;
