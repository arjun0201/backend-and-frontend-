import axios from 'axios';
import config from './config'
import Product from './../components/Product'

const axiosInstance = axios.create({
  baseURL: config.API_URL,
  headers: {
    "Content-type": "application/json",
    [config.API_KEY_HEADER]: config.API_KEY
  }
});

const getAll = () => {
  return axiosInstance.get("/products");
};

const create = (data : Product) => {
  return axiosInstance.post("/products", data);
};

const update = (data : Product) => {
  return axiosInstance.patch(`/products/${data.id}`, data);
};

const remove = (id : string) => {
  return axiosInstance.delete(`/products/${id}`);
};

const productsApi = {
  getAll,
  create,
  update,
  remove,
};

export default productsApi;