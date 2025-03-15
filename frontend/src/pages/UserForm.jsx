import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./UserForm.module.css";

const UserForm = () => {
  const [formData, setFormData] = useState({
    name: "",
    lastName: "",
    city: "",
    country: "",
    state: "",
    email: "",
    username: "",
    passwordHash: ""
  });

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  // Redirect to login if not authenticated
  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) {
      navigate("/login"); // Redirect if no token found
    }
  }, [navigate]);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const token = localStorage.getItem("token");
        const response = await fetch("http://localhost:5000/api/user/me", {
          method: "GET",
          headers: { Authorization: `Bearer ${token}` }, 
        });

        if (!response.ok) throw new Error("Failed to fetch user data");

        const data = await response.json();
        setFormData({
          name: data.name || "",
          lastName: data.lastName || "",
          city: data.city || "",
          country: data.country || "",
          state: data.state || "",
          email: data.email || "",
          username: data.username || "",
          passwordHash: data.passwordHash || "",
        });
      } catch (error) {
        setMessage("Error loading user data");
      }
    };

    fetchUserData();
  }, []);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const token = localStorage.getItem("token");
      const response = await fetch("http://localhost:5000/api/user/me", {
        method: "PUT",
        headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
        body: JSON.stringify(formData),
      });

      const data = await response.json();
      if (response.ok) {
        setMessage("Profile updated successfully!");
      } else {
        setMessage(data || "Update failed.");
      }
    } catch (error) {
      setMessage("Error connecting to the server.");
    } finally {
      setLoading(false);
    }
  };

  // Logout
  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <div className={styles.container}>
      <h2>Edit Profile</h2>
      <form onSubmit={handleSubmit}>
        <input type="text" name="name" placeholder="Name" value={formData.name} onChange={handleChange} required />
        <input type="text" name="lastName" placeholder="Last Name" value={formData.lastName} onChange={handleChange} required />
        <input type="text" name="city" placeholder="City" value={formData.city} onChange={handleChange} required />
        <input type="text" name="country" placeholder="Country" value={formData.country} onChange={handleChange} required />
        <input type="text" name="state" placeholder="State" value={formData.state} onChange={handleChange} required />
        
        {/* Disabled fields */}
        <input type="email" name="email" value={formData.email} disabled />
        <input type="text" name="username" value={formData.username} disabled />
        <input type="text" name="passwordHash" value={formData.passwordHash} disabled />

        <button type="submit" disabled={loading}>{loading ? "Saving..." : "Save Changes"}</button>
      </form>

      {message && <p className={styles.message}>{message}</p>}

      {/* Logout Button */}
      <button className={styles.logout} onClick={handleLogout}>Logout</button>
    </div>
  );
};

export default UserForm;
