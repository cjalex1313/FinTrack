import { useAuthApi } from '@/api/authApi'
import type { ProfileDTO } from '@/api/models'
import type { Role } from '@/models/role'
import { defineStore } from 'pinia'
import { reactive } from 'vue'

const jwtKey = 'jwt'

export const useAuthStore = defineStore('auth', () => {
  const authApi = useAuthApi()

  const authData = reactive<{
    profile: ProfileDTO | null
    currentRole: Role | null
  }>({
    profile: null,
    currentRole: null,
  })

  async function setJwt(accessToken: string) {
    localStorage.setItem(jwtKey, accessToken)
    await getProfile();
  }

  async function getProfile() {
    if (localStorage.getItem(jwtKey)) {
      const profile = await authApi.getProfile()
      authData.profile = profile
      if (profile.roles && profile.roles.length > 0) {
        authData.currentRole = profile.roles[0]
      }
    }
  }

  async function signOut() {
    if (localStorage.getItem(jwtKey)) {
      localStorage.removeItem(jwtKey)
    }
    authData.profile = null
  }

  return { authData, setJwt, signOut, getProfile }
})
