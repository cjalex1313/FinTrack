<template>
  <div class="max-w-4xl mx-auto p-6">
    <h1 class="text-3xl font-bold text-gray-900 mb-8">Profile Settings</h1>

    <!-- Profile Information Section -->
    <div class="bg-white shadow rounded-lg mb-8">
      <div class="px-6 py-4 border-b border-gray-200">
        <h2 class="text-xl font-semibold text-gray-900">Personal Information</h2>
        <p class="text-sm text-gray-600 mt-1">Update your first and last name</p>
      </div>
      <div class="p-6">
        <form @submit.prevent="updateProfile" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700 mb-1">
                First Name
              </label>
              <input
                id="firstName"
                v-model="profileForm.firstName"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter your first name"
              />
            </div>
            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700 mb-1">
                Last Name
              </label>
              <input
                id="lastName"
                v-model="profileForm.lastName"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                placeholder="Enter your last name"
              />
            </div>
          </div>
          <div class="flex justify-end">
            <button
              type="submit"
              :disabled="isUpdatingProfile"
              class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="isUpdatingProfile">Updating...</span>
              <span v-else>Update Profile</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Change Password Section -->
    <div class="bg-white shadow rounded-lg">
      <div class="px-6 py-4 border-b border-gray-200">
        <h2 class="text-xl font-semibold text-gray-900">Change Password</h2>
        <p class="text-sm text-gray-600 mt-1">Update your password</p>
      </div>
      <div class="p-6">
        <form @submit.prevent="changePassword" class="space-y-4">
          <div>
            <label for="currentPassword" class="block text-sm font-medium text-gray-700 mb-1">
              Current Password
            </label>
            <input
              id="currentPassword"
              v-model="passwordForm.oldPassword"
              type="password"
              class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              placeholder="Enter your current password"
              required
            />
          </div>
          <div>
            <label for="newPassword" class="block text-sm font-medium text-gray-700 mb-1">
              New Password
            </label>
            <input
              id="newPassword"
              v-model="passwordForm.newPassword"
              type="password"
              class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              placeholder="Enter your new password"
              required
            />
          </div>
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 mb-1">
              Confirm New Password
            </label>
            <input
              id="confirmPassword"
              v-model="passwordForm.confirmPassword"
              type="password"
              class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
              placeholder="Confirm your new password"
              required
            />
          </div>
          <div class="flex justify-end">
            <button
              type="submit"
              :disabled="isChangingPassword || !isPasswordValid"
              class="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span v-if="isChangingPassword">Changing Password...</span>
              <span v-else>Change Password</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Success/Error Messages -->
    <div
      v-if="message"
      class="mt-4 p-4 rounded-md"
      :class="messageType === 'success' ? 'bg-green-50 text-green-800' : 'bg-red-50 text-red-800'"
    >
      {{ message }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAuthApi } from '@/api/authApi'
import type { ProfileDTO, UpdateProfileNamesDTO, ChangePasswordDTO } from '@/api/models'

const authApi = useAuthApi()

// Profile form state
const profileForm = ref<UpdateProfileNamesDTO>({
  firstName: '',
  lastName: '',
})

// Password form state
const passwordForm = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: '',
})

// Loading states
const isUpdatingProfile = ref(false)
const isChangingPassword = ref(false)

// Message state
const message = ref('')
const messageType = ref<'success' | 'error'>('success')

// Computed property to check if password form is valid
const isPasswordValid = computed(() => {
  return (
    passwordForm.value.newPassword &&
    passwordForm.value.newPassword === passwordForm.value.confirmPassword &&
    passwordForm.value.newPassword.length >= 6
  )
})

// Load user profile on component mount
onMounted(async () => {
  try {
    const profile = await authApi.getProfile()
    profileForm.value = {
      firstName: profile.firstName || '',
      lastName: profile.lastName || '',
    }
  } catch (error) {
    showMessage('Failed to load profile information', 'error')
  }
})

// Update profile function
const updateProfile = async () => {
  isUpdatingProfile.value = true
  try {
    await authApi.updateProfileNames(profileForm.value)
    showMessage('Profile updated successfully!', 'success')
  } catch (error) {
    showMessage('Failed to update profile. Please try again.', 'error')
  } finally {
    isUpdatingProfile.value = false
  }
}

// Change password function
const changePassword = async () => {
  if (!isPasswordValid.value) {
    showMessage('Please ensure passwords match and are at least 6 characters long.', 'error')
    return
  }

  isChangingPassword.value = true
  try {
    const request: ChangePasswordDTO = {
      oldPassword: passwordForm.value.oldPassword,
      newPassword: passwordForm.value.newPassword,
    }
    await authApi.changePassword(request.oldPassword, request.newPassword)
    showMessage('Password changed successfully!', 'success')
    // Clear password form
    passwordForm.value = {
      oldPassword: '',
      newPassword: '',
      confirmPassword: '',
    }
  } catch (error) {
    showMessage(
      'Failed to change password. Please check your current password and try again.',
      'error',
    )
  } finally {
    isChangingPassword.value = false
  }
}

// Helper function to show messages
const showMessage = (msg: string, type: 'success' | 'error') => {
  message.value = msg
  messageType.value = type
  // Clear message after 5 seconds
  setTimeout(() => {
    message.value = ''
  }, 5000)
}
</script>
