import { useState } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Login.module.css";

const Login = () => {
  const [formData, setFormData] = useState({ email: "", passwordHash: "" });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const response = await fetch("http://localhost:5000/api/user/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });

      const data = await response.json();

      if (response.ok) {
        localStorage.setItem("token", data.token); // Store JWT token
        setMessage("Login successful!");
        setTimeout(() => navigate("/userform"), 1000); // Redirect after 1s
      } else if (data.message === "User not found") {
        setMessage("User not registered! Redirecting to Register...");
        setTimeout(() => navigate("/register"), 2000); // Redirect to register after 2s
      } else {
        setMessage(data.message || "Login failed.");
      }
    } catch (error) {
      setMessage("Error connecting to the server.");
    } finally {
      setLoading(false);
    }
  };

  // Redirect to Google login - in construction.
  const handleGoogleLogin = () => {
    window.location.href = "http://localhost:5000/api/user/login/google";
  };

  return (
    <div className={styles.container}>
      <h2>Login</h2>
      <form onSubmit={handleSubmit}>
        <input type="email" name="email" placeholder="Email" value={formData.email} onChange={handleChange} required />
        <input type="password" name="passwordHash" placeholder="Password" value={formData.passwordHash} onChange={handleChange} required />
        <button type="submit" disabled={loading}>{loading ? "Logging in..." : "Login"}</button>
      </form>

      {/* Google Login Button - Not working yet */}
      <button className={styles.googleLogin} onClick={handleGoogleLogin}>
        Login with Google
      </button>

      {message && <p className={styles.message}>{message}</p>}
      <p>Don't have an account? <span onClick={() => navigate("/register")} className={styles.link}>Register here</span></p>
    </div>
  );
};

export default Login;
