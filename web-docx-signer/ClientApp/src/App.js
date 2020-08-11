import React, { Component, useState } from 'react';
import { SignaturePage } from './components/SignaturePage.js';

import './custom.css'

const App = () =>{

  const [haveSelectedfiles, setHaveSelectedfiles] = useState (false);

  return (
   <SignaturePage />
  );
}

export default App;
