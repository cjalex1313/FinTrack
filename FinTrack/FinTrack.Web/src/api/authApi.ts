import { useBaseApi } from './baseApi'
import type { ForgotPasswordDTO, LoginResponse, ProfileDTO, ResetPasswordDTO } from './models'

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

  const forgotPassword = async (email: string) => {
    const request: ForgotPasswordDTO = { email }
    await baseApi.post<void>('api/auth/forgot-password', request)
  }

  const resetPassword = async (userId: string, token: string, password: string) => {
    const request: ResetPasswordDTO = { userId, token, password }
    await baseApi.post<void>('api/auth/reset-password', request)
  }

  return { login, getProfile, register, confirmEmail, forgotPassword, resetPassword }
}
