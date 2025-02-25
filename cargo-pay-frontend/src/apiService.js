import axios from "axios";

const API_URL = "http://localhost:5021/api/card";

export const getCardBalance = async (cardNumber) => {
  const token = localStorage.getItem("token"); // Recuperar el token almacenado
  if (!token) {
    throw new Error("No hay token disponible. Inicia sesión.");
  }

  try {
    const response = await axios.get(`${API_URL}/${cardNumber}`, {
      headers: {
        Authorization: `Bearer ${token}`, // Enviar el token en el header
      },
    });
    return response.data; // Retorna los datos del saldo
  } catch (error) {
    throw error.response?.data || "Error al obtener el balance.";
  }
};
