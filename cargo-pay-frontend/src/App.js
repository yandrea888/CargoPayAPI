
import './App.css';
import Login from "./Login";
import CardBalance from './CardBalance';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";


function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/card-balance" element={<CardBalance />} />
      </Routes>
    </Router>
  );
}

export default App;
