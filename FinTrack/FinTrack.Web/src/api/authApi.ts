import { useBaseApi } from './baseApi'
import type { LoginResponse, ProfileDTO } from './models'

export function useAuthApi() {
  const { baseApi } = useBaseApi()

  const login = async (email: string, password: string) => {
    const response = await baseApi.post<LoginResponse>('api/auth/login', {
      email,
      password,
    })
    return response.data
  }

  const getProfile = async () => {
    const response = await baseApi.get<ProfileDTO>('api/auth/profile')
    return response.data
  }

  const register = async (email: string, password: string) => {
    await baseApi.post<void>('api/auth/register', {
      email,
      password,
    })
  }

  const confirmEmail = async (userId: string, token: string) => {
    const response = await baseApi.patch<LoginResponse>('api/auth/confirm-email', {
      userId,
      token,
    })
    return response.data
  }

  const externalLoginCallback = async (userData) => {
    const response = await baseApi.post<LoginResponse>('api/auth/ExternalLoginCallback', userData)
    debugger;
    return response.data
  }
  return { login, getProfile, register, confirmEmail, externalLoginCallback }
}
