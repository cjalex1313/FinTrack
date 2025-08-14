import { useBaseApi } from './baseApi'
import type {
  ChangePasswordDTO,
  ForgotPasswordDTO,
  LoginResponse,
  ProfileDTO,
  ResetPasswordDTO,
  UpdateProfileNamesDTO,
} from './models'

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

  const changePassword = async (oldPassword: string, newPassword: string) => {
    const request: ChangePasswordDTO = { oldPassword, newPassword }
    await baseApi.patch<void>('api/auth/change-password', request)
  }

  const updateProfileNames = async (request: UpdateProfileNamesDTO) => {
    const response = await baseApi.patch<ProfileDTO>('api/auth/profile-names', request)
    return response.data
  }

  const setPassword = async (password: string) => {
    await baseApi.patch<void>('api/auth/set-password', { password })
  }

  const externalLoginCallback = async (userData) => {
    const response = await baseApi.post<LoginResponse>('api/auth/ExternalLoginCallback', userData)
    return response.data
  }

  return {
    login,
    getProfile,
    register,
    confirmEmail,
    forgotPassword,
    resetPassword,
    changePassword,
    updateProfileNames,
    setPassword,
    externalLoginCallback,
  }
}
