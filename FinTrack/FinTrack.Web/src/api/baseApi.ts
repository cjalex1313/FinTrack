import axios from 'axios'
import { useToast } from 'primevue'

export function useBaseApi() {
  const baseUrl = import.meta.env.VITE_API_BASE_URL
  const timeout = 30000 // 30 seconds timeout

  const toast = useToast()

  const baseApi = axios.create({
    baseURL: baseUrl,
    timeout: timeout,
    headers: {
      'Content-Type': 'application/json',
      // You can add other default headers here
    },
  })

  // Request interceptor
  baseApi.interceptors.request.use(
    (config) => {
      const jwt = localStorage.getItem('jwt')
      if (jwt) {
        config.headers.Authorization = `Bearer ${jwt}`
      }
      return config
    },
    (error) => {
      // Handle request errors
      return Promise.reject(error)
    },
  )

  // Response interceptor
  baseApi.interceptors.response.use(
    (response) => {
      // Any status code that lie within the range of 2xx cause this function to trigger
      // Do something with response data
      return response // Extract only the data from the response
    },
    (error) => {
      console.log(error)
      if (error.response) {
        // The request was made and the server responded with a status code
        // that falls out of the range of 2xx
        if (error.response?.data?.error) {
          const customError = error.response?.data?.error
          const message = customError
          toast.add({ severity: 'error', summary: 'Error', detail: message, life: 5000 })
        } else if (error.response?.data?.title) {
          const message = error.response?.data?.title
          toast.add({ severity: 'error', summary: 'Error', detail: message, life: 5000 })
        }

        //Example error handling, display an error message
        // if (error.response.status === 401) {
        //   // Handle unauthorized errors (e.g., redirect to login)
        // }
        // else if(error.response.status === 404){
        //   //handle 404
        // }

        return Promise.reject({
          ...error.response.data,
          handled: true,
        }) //Reject with the server error response.
      } else if (error.request) {
        // The request was made but no response was received
        // `error.request` is an instance of XMLHttpRequest in the browser and an instance of
        // http.ClientRequest in node.js
        console.error('API Error: No response received', error.request)
        return Promise.reject({ message: 'No response received from the server.' })
      } else {
        // Something happened in setting up the request that triggered an Error
        console.error('API Error: Request setup error', error.message)
        return Promise.reject({ message: error.message })
      }
    },
  )

  return {
    baseApi,
  }
}
