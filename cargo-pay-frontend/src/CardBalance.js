import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import "./CardBalance.css";

const CardBalance = () => {
  const [cardNumber, setCardNumber] = useState("");
  const [balance, setBalance] = useState(null);
  const navigate = useNavigate();

  const fetchBalance = async () => {
    const token = localStorage.getItem("token"); 

    if (!cardNumber) {
        alert("Please enter the card number.");
      return;
    }

    try {
      const response = await axios.get(`https://localhost:7058/api/Card/${cardNumber}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

        console.log("API response:", response.data);

      setBalance(response.data.balance || 0);
    } catch (error) {
      console.error("Error getting balance:", error.response?.data || error.message);
    }
  };
  
  
  return (
    <div className="balance-container">
      <h2>Check Balance</h2>
      <input
        type="text"
        placeholder="Enter the card number"
        value={cardNumber}
        onChange={(e) => setCardNumber(e.target.value)}
      />
      <button onClick={fetchBalance}>Check Balance</button>

      {balance !== null && <p className="balance-amount">💰 ${balance}</p>}
      <hr></hr>

      <button className="back-button" onClick={() => navigate("/")}>Return</button>
    </div>
  );
};

export default CardBalance;
