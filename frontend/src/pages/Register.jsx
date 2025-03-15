import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Register.module.css";

const Register = () => {
  const [formData, setFormData] = useState({
    name: "",
    lastName: "",
    username: "",
    email: "",
    passwordHash: "",
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  // Redirect if already logged in
  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      navigate("/userForm");
    }
  }, [navigate]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name === "password" ? "passwordHash" : name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const response = await fetch("http://localhost:5000/api/user/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });

      const data = await response.json();

      if (response.ok) {
        setMessage("User registered successfully!");
        setTimeout(() => navigate("/login"), 1000);
      } else {
        setMessage(data?.message || "Registration failed.");
      }
    } catch (error) {
      setMessage("Error connecting to the server.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <input type="text" name="name" placeholder="First Name" value={formData.name} onChange={handleChange} required />
        <input type="text" name="lastName" placeholder="Last Name" value={formData.lastName} onChange={handleChange} required />
        <input type="text" name="username" placeholder="Username" value={formData.username} onChange={handleChange} required />
        <input type="email" name="email" placeholder="Email" value={formData.email} onChange={handleChange} required />
        <input type="password" name="password" placeholder="Password" value={formData.passwordHash} onChange={handleChange} required />
        <button type="submit" disabled={loading}>{loading ? "Registering..." : "Register"}</button>
      </form>
      {message && <p className={styles.message}>{message}</p>}
    </div>
  );
};

export default Register;
