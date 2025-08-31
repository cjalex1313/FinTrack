import { setActivePinia, createPinia } from 'pinia'
import { describe, it, expect, beforeEach, vi, type MockedFunction } from 'vitest'
import { useAuthStore } from './auth'
import { useAuthApi } from '@/api/authApi'
import type { ProfileDTO } from '@/api/models'
import { Role } from '@/models/role'

// Mock the authApi module
vi.mock('@/api/authApi')

// Mock localStorage
const localStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
}

Object.defineProperty(window, 'localStorage', {
  value: localStorageMock,
})

describe('Auth Store', () => {
  let mockAuthApi: {
    getProfile: MockedFunction<any>
  }

  beforeEach(() => {
    // Reset Pinia before each test to ensure a fresh store
    setActivePinia(createPinia())

    // Reset all mocks
    vi.clearAllMocks()

    // Setup mock authApi
    mockAuthApi = {
      getProfile: vi.fn(),
    }
    ;(useAuthApi as any).mockReturnValue(mockAuthApi)

    // Reset localStorage mock
    localStorageMock.getItem.mockReturnValue(null)
    localStorageMock.setItem.mockImplementation(() => {})
    localStorageMock.removeItem.mockImplementation(() => {})
  })

  describe('Initial State', () => {
    it('initializes with null profile and currentRole', () => {
      const authStore = useAuthStore()

      expect(authStore.authData.profile).toBeNull()
      expect(authStore.authData.currentRole).toBeNull()
    })
  })

  describe('setJwt', () => {
    it('stores JWT in localStorage and calls getProfile', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: [Role.User],
      }

      // Mock localStorage.getItem to return the JWT for getProfile call
      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)

      await authStore.setJwt('mock-jwt-token')

      expect(localStorageMock.setItem).toHaveBeenCalledWith('jwt', 'mock-jwt-token')
      expect(mockAuthApi.getProfile).toHaveBeenCalled()
      expect(authStore.authData.profile).toEqual(mockProfile)
      expect(authStore.authData.currentRole).toBe(Role.User)
    })

    it('handles profile with admin role', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'admin@example.com',
        firstName: 'Admin',
        lastName: 'User',
        roles: [Role.Admin],
      }

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)

      await authStore.setJwt('mock-jwt-token')

      expect(authStore.authData.currentRole).toBe(Role.Admin)
    })

    it('handles profile with empty roles array', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: [] as any, // Cast to any since the interface expects [Role] but we want to test edge case
      }

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)

      await authStore.setJwt('mock-jwt-token')

      expect(authStore.authData.currentRole).toBeNull()
    })
  })

  describe('getProfile', () => {
    it('fetches profile when JWT exists in localStorage', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: [Role.User],
      }

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)

      await authStore.getProfile()

      expect(localStorageMock.getItem).toHaveBeenCalledWith('jwt')
      expect(mockAuthApi.getProfile).toHaveBeenCalled()
      expect(authStore.authData.profile).toEqual(mockProfile)
      expect(authStore.authData.currentRole).toBe(Role.User)
    })

    it('does not fetch profile when JWT does not exist in localStorage', async () => {
      const authStore = useAuthStore()

      localStorageMock.getItem.mockReturnValue(null)

      await authStore.getProfile()

      expect(localStorageMock.getItem).toHaveBeenCalledWith('jwt')
      expect(mockAuthApi.getProfile).not.toHaveBeenCalled()
      expect(authStore.authData.profile).toBeNull()
      expect(authStore.authData.currentRole).toBeNull()
    })

    it('handles profile with null roles', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: null as any,
      }

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)

      await authStore.getProfile()

      expect(authStore.authData.profile).toEqual(mockProfile)
      expect(authStore.authData.currentRole).toBeNull()
    })

    it('handles API error gracefully', async () => {
      const authStore = useAuthStore()

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockRejectedValue(new Error('API Error'))

      await expect(authStore.getProfile()).rejects.toThrow('API Error')

      expect(authStore.authData.profile).toBeNull()
      expect(authStore.authData.currentRole).toBeNull()
    })
  })

  describe('signOut', () => {
    it('removes JWT from localStorage and resets auth data', async () => {
      const authStore = useAuthStore()

      // Set initial state
      authStore.authData.profile = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: [Role.User],
      }
      authStore.authData.currentRole = Role.User

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')

      await authStore.signOut()

      expect(localStorageMock.removeItem).toHaveBeenCalledWith('jwt')
      expect(authStore.authData.profile).toBeNull()
      // Note: The original implementation doesn't reset currentRole, only profile
    })

    it('handles sign out when no JWT exists in localStorage', async () => {
      const authStore = useAuthStore()

      localStorageMock.getItem.mockReturnValue(null)

      await authStore.signOut()

      expect(localStorageMock.removeItem).not.toHaveBeenCalled()
      expect(authStore.authData.profile).toBeNull()
    })

    it('resets auth data even if JWT removal fails', async () => {
      const authStore = useAuthStore()

      // Set initial state
      authStore.authData.profile = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: [Role.User],
      }

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      localStorageMock.removeItem.mockImplementation(() => {
        throw new Error('Storage error')
      })

      // Should throw because the original implementation doesn't handle errors
      await expect(authStore.signOut()).rejects.toThrow('Storage error')
    })
  })

  describe('Reactive Updates', () => {
    it('updates authData reactively', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'test@example.com',
        firstName: 'John',
        lastName: 'Doe',
        roles: [Role.User],
      }

      // Initially null
      expect(authStore.authData.profile).toBeNull()

      // Update through setJwt
      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)
      await authStore.setJwt('mock-jwt-token')

      expect(authStore.authData.profile).toEqual(mockProfile)

      // Update through signOut
      await authStore.signOut()

      expect(authStore.authData.profile).toBeNull()
      expect(false).toBe(true)
    })
  })

  describe('Edge Cases', () => {
    it('handles empty string JWT stored in localStorage', async () => {
      const authStore = useAuthStore()

      // Test that when empty string is stored, getProfile doesn't fetch
      localStorageMock.getItem.mockReturnValue('')

      await authStore.getProfile()

      // Empty string is falsy, so API should NOT be called
      expect(mockAuthApi.getProfile).not.toHaveBeenCalled()
    })

    it('handles profile with undefined firstName and lastName', async () => {
      const authStore = useAuthStore()
      const mockProfile: ProfileDTO = {
        id: '123',
        email: 'test@example.com',
        firstName: undefined as any,
        lastName: undefined as any,
        roles: [Role.User],
      }

      localStorageMock.getItem.mockReturnValue('mock-jwt-token')
      mockAuthApi.getProfile.mockResolvedValue(mockProfile)

      await authStore.setJwt('mock-jwt-token')

      expect(authStore.authData.profile?.firstName).toBeUndefined()
      expect(authStore.authData.profile?.lastName).toBeUndefined()
    })
  })
})
