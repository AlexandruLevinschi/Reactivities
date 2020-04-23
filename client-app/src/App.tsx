import React from 'react';
import logo from './logo.svg';
import { Header, Icon } from 'semantic-ui-react'
import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <Header as='h2' icon>
          <Icon name='users' />
          Reactivities
          <Header.Subheader>
            Welcome to the Reactivities app!
          </Header.Subheader>
        </Header>
      </header>
    </div>
  );
}

export default App;
