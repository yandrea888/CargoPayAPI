import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom"; 
import "./Login.css";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [token, setToken] = useState(null);
  const [error, setError] = useState("");
  const navigate = useNavigate(); 

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const response = await axios.post("http://localhost:5021/api/auth/login", {
        username,
        password,
      });

      setToken(response.data.token);
      localStorage.setItem("token", response.data.token);
    } catch (error) {
      setError("Invalid credentials");
    }
  };

  return (
    <div className="login-container">
  <h2>Login</h2>

  {token ? (
    <div>
      <p className="success-message">✅ Successful Login</p>
      <button onClick={() => navigate("/card-balance")}>Check Balance</button>
    </div>
  ) : (
    <form onSubmit={handleLogin}>
      <input
        type="text"
        placeholder="User"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      <button type="submit">Login</button>
    </form>
  )}

  {error && <p className="error-message">{error}</p>}
</div>

  );
};

export default Login;
