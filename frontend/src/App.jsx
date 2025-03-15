import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Register from './pages/Register';
import Login from './pages/Login';
import UserForm from './pages/UserForm';
import './App.css';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/userForm" element={<UserForm />} />
      </Routes>
    </Router>
  );
}

export default App;
