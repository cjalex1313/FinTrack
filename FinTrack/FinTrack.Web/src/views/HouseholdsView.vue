<template>
  <div class="max-w-5xl mx-auto p-6">
    <!-- Household Setup Wizard Content -->
    <div v-if="showSetupWizard">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-3xl font-bold text-gray-900">Create New Household</h1>
        <button
          @click="showSetupWizard = false"
          class="text-gray-400 hover:text-gray-600 transition-colors p-2 rounded-md hover:bg-gray-100"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            ></path>
          </svg>
        </button>
      </div>
      <HouseholdSetupWizard @completed="onSetupCompleted" />
    </div>

    <!-- Household Management Content -->
    <div v-else>
      <div class="flex justify-between items-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Households</h1>

        <!-- Add Household Button (only show if user is not an owner of any household) -->
        <button
          v-if="!isOwnerOfAnyHousehold"
          @click="showSetupWizard = true"
          class="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md font-medium transition-colors duration-200 flex items-center gap-2"
          :disabled="householdStore.loading"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 4v16m8-8H4"
            ></path>
          </svg>
          Add Household
        </button>
      </div>

      <!-- Household Selector -->
      <div class="mb-6">
        <label for="household-select" class="block text-sm font-medium text-gray-700 mb-2">
          Select Household
        </label>
        <select
          id="household-select"
          v-model="selectedHouseholdId"
          @change="handleHouseholdChange"
          class="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
          :disabled="householdStore.loading"
        >
          <option value="" disabled>Choose a household...</option>
          <option
            v-for="household in sortedHouseholds"
            :key="household.householdId"
            :value="household.householdId"
          >
            {{ household.householdName }}
            <span v-if="household.role !== null"> ({{ getRoleDisplayName(household.role) }})</span>
          </option>
        </select>
      </div>

      <!-- Current Household Display -->
      <div
        v-if="householdStore.currentHousehold"
        class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6"
      >
        <h2 class="text-lg font-semibold text-blue-900 mb-2">Current Household</h2>
        <p class="text-blue-800">
          <strong>Name:</strong> {{ householdStore.currentHousehold.householdName }}
        </p>
        <p class="text-blue-800" v-if="householdStore.currentHousehold.role !== null">
          <strong>Your Role:</strong> {{ getRoleDisplayName(householdStore.currentHousehold.role) }}
        </p>
      </div>

      <!-- Household Members Section (only show if user is owner) -->
      <div v-if="isOwnerOfCurrentHousehold" class="mb-6">
        <div class="bg-white border border-gray-200 rounded-lg p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Household Members</h3>
            <div class="flex items-center space-x-2">
              <button
                @click="showInviteDialog = true"
                class="bg-indigo-600 hover:bg-indigo-700 text-white px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200 flex items-center gap-1"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 4v16m8-8H4"
                  ></path>
                </svg>
                Invite Member
              </button>
              <button
                @click="loadHouseholdMembers"
                :disabled="loadingMembers"
                class="text-indigo-600 hover:text-indigo-800 text-sm font-medium disabled:opacity-50"
              >
                <svg
                  class="w-4 h-4 inline mr-1"
                  :class="{ 'animate-spin': loadingMembers }"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
                  ></path>
                </svg>
                Refresh
              </button>
            </div>
          </div>

          <!-- Loading State for Members -->
          <div v-if="loadingMembers" class="text-center py-4">
            <p class="text-gray-600">Loading members...</p>
          </div>

          <!-- Members List -->
          <div v-else-if="householdMembers.length > 0" class="space-y-3">
            <div
              v-for="member in householdMembers"
              :key="member.householdId + '-' + member.userEmail"
              class="flex items-center justify-between p-4 bg-gray-50 rounded-lg"
            >
              <div class="flex items-center space-x-3">
                <div class="w-10 h-10 bg-indigo-100 rounded-full flex items-center justify-center">
                  <svg
                    class="w-5 h-5 text-indigo-600"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                    ></path>
                  </svg>
                </div>
                <div>
                  <p class="font-medium text-gray-900">
                    <span v-if="member.userFirstName || member.userLastName">
                      {{ member.userFirstName }} {{ member.userLastName }}
                    </span>
                    <span v-else class="text-gray-500">No name provided</span>
                  </p>
                  <p class="text-sm text-gray-600">{{ member.userEmail }}</p>
                </div>
              </div>
              <div class="flex items-center space-x-2">
                <span
                  class="px-2 py-1 text-xs font-medium rounded-full"
                  :class="getStatusColorClasses(member.status)"
                >
                  {{ getStatusDisplayName(member.status) }}
                </span>
                <span
                  class="px-2 py-1 text-xs font-medium rounded-full"
                  :class="{
                    'bg-green-100 text-green-800': member.role === Owner,
                    'bg-blue-100 text-blue-800': member.role === Admin,
                    'bg-gray-100 text-gray-800': member.role === Member,
                  }"
                >
                  {{ getRoleDisplayName(member.role) }}
                </span>
                <!-- Delete button (only show for members who are not the current user) -->
                <button
                  v-if="member.userEmail !== currentUserEmail"
                  @click="confirmDeleteMember(member)"
                  class="ml-2 p-1 text-red-600 hover:text-red-800 hover:bg-red-50 rounded transition-colors duration-200"
                  title="Remove member"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                    />
                  </svg>
                </button>
              </div>
            </div>
          </div>

          <!-- No Members State -->
          <div v-else class="text-center py-8">
            <svg
              class="w-12 h-12 text-gray-400 mx-auto mb-3"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
              ></path>
            </svg>
            <p class="text-gray-500">No members found</p>
            <p class="text-sm text-gray-400">
              Members will appear here once they join the household
            </p>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="householdStore.loading" class="text-center py-4">
        <p class="text-gray-600">Loading households...</p>
      </div>

      <!-- No Households State -->
      <div
        v-else-if="!householdStore.households || householdStore.households.length === 0"
        class="text-center py-4"
      >
        <p class="text-gray-600">No households available.</p>
      </div>
    </div>

    <!-- Invite Member Dialog -->
    <!-- Pending Household Invites -->
    <div v-if="pendingInvites.length > 0 || loadingPendingInvites" class="mt-8">
      <div class="bg-white border border-gray-200 rounded-lg p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900">Pending Household Invites</h3>
          <button
            @click="loadPendingInvites"
            :disabled="loadingPendingInvites"
            class="text-indigo-600 hover:text-indigo-800 text-sm font-medium disabled:opacity-50"
          >
            <svg
              class="w-4 h-4 inline mr-1"
              :class="{ 'animate-spin': loadingPendingInvites }"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
              ></path>
            </svg>
            Refresh
          </button>
        </div>

        <div v-if="loadingPendingInvites" class="text-center py-4">
          <p class="text-gray-600">Loading pending invites...</p>
        </div>

        <div v-else-if="pendingInvites.length > 0" class="space-y-3">
          <div
            v-for="invite in pendingInvites"
            :key="invite.householdId + '-' + invite.userEmail"
            class="flex items-center justify-between p-4 bg-gray-50 rounded-lg"
          >
            <div>
              <p class="font-medium text-gray-900">
                {{ invite.householdName || 'Unnamed Household' }}
              </p>
            </div>

            <div class="flex items-center space-x-2">
              <button
                @click="acceptInviteAction(invite.householdId)"
                :disabled="processingInvite === invite.householdId"
                class="bg-green-600 hover:bg-green-700 text-white px-3 py-1.5 rounded-md text-sm font-medium transition-colors duration-200"
              >
                Accept
              </button>
              <button
                @click="rejectInviteAction(invite)"
                :disabled="processingInvite === invite.householdId"
                class="bg-red-50 text-red-600 hover:bg-red-100 px-3 py-1.5 rounded-md text-sm font-medium transition-colors duration-200"
              >
                Reject
              </button>
            </div>
          </div>
        </div>

        <div v-else class="text-center py-6 text-gray-500">No pending invites</div>
      </div>
    </div>

    <InviteMemberDialog
      v-if="showInviteDialog && householdStore.currentHousehold"
      :household-id="householdStore.currentHousehold.householdId"
      @close="showInviteDialog = false"
      @invited="handleMemberInvited"
    />
  </div>
</template>

<script setup lang="ts">
import { useHouseholdStore } from '@/stores/household'
import { useAuthStore } from '@/stores/auth'
import { useHouseholdApi } from '@/api/householdApi'
import type { HouseholdMemberDTO } from '@/api/models'
import { computed, onMounted, ref, watch } from 'vue'
import HouseholdSetupWizard from '@/components/HouseholdSetupWizard.vue'
import InviteMemberDialog from '@/components/household/InviteMemberDialog.vue'
import { HouseholdMemberRole, HouseholdMemberStatus } from '@/models/role'
import { useConfirm } from 'primevue/useconfirm'
import { useCustomToast } from '@/composables/useCustomToast'

const householdStore = useHouseholdStore()
const authStore = useAuthStore()
const householdApi = useHouseholdApi()
const confirm = useConfirm()
const toast = useCustomToast()

const selectedHouseholdId = ref<string>('')
const showSetupWizard = ref<boolean>(false)
const showInviteDialog = ref<boolean>(false)
const householdMembers = ref<HouseholdMemberDTO[]>([])
const loadingMembers = ref<boolean>(false)

const currentHouseholdId = computed(() => householdStore.currentHousehold?.householdId || '')
const currentUserEmail = computed(() => authStore.authData.profile?.email || '')

// Check if user is an owner of any household
const isOwnerOfAnyHousehold = computed(() => {
  if (!householdStore.households) return false
  return householdStore.households.some((household) => household.role === HouseholdMemberRole.Owner)
})

// Check if user is the owner of the current household
const isOwnerOfCurrentHousehold = computed(() => {
  return householdStore.currentHousehold?.role === HouseholdMemberRole.Owner
})

// Function to convert role enum to human-readable text
const getRoleDisplayName = (role: number | null): string => {
  if (role === null) return 'Unknown'

  switch (role) {
    case 0:
      return 'Member'
    case 1:
      return 'Admin'
    case 2:
      return 'Owner'
    default:
      return 'Unknown'
  }
}

// Function to convert status enum to human-readable text
const getStatusDisplayName = (status: number | null): string => {
  if (status === null) return 'Unknown'

  switch (status) {
    case 0:
      return 'Pending Response'
    case 1:
      return 'Expired'
    case 2:
      return 'Rejected'
    case 3:
      return 'Active'
    default:
      return 'Unknown'
  }
}

// Function to get status color classes
const getStatusColorClasses = (status: number | null): string => {
  if (status === null) return 'bg-gray-100 text-gray-800'

  switch (status) {
    case 0: // Pending Response
      return 'bg-yellow-100 text-yellow-800'
    case 1: // Expired
      return 'bg-gray-100 text-gray-800'
    case 2: // Rejected
      return 'bg-red-100 text-red-800'
    case 3: // Active
      return 'bg-green-100 text-green-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

// Load household members
const loadHouseholdMembers = async () => {
  if (!householdStore.currentHousehold || !isOwnerOfCurrentHousehold.value) {
    householdMembers.value = []
    return
  }

  try {
    loadingMembers.value = true
    householdMembers.value = await householdApi.getHouseholdMembers(
      householdStore.currentHousehold.householdId,
    )
  } catch (error) {
    console.error('Failed to load household members:', error)
    householdMembers.value = []
  } finally {
    loadingMembers.value = false
  }
}

// Watch for current household changes and load members if user is owner
watch(
  () => householdStore.currentHousehold,
  () => {
    loadHouseholdMembers()
  },
  { immediate: true },
)

onMounted(async () => {
  await householdStore.loadHouseholds()
  // Load pending invites for the current user
  await loadPendingInvites()
  // Set the selected value to match the current household
  selectedHouseholdId.value = currentHouseholdId.value
})

const handleHouseholdChange = () => {
  if (selectedHouseholdId.value && householdStore.households) {
    const selectedHousehold = householdStore.households.find(
      (h) => h.householdId === selectedHouseholdId.value,
    )
    if (selectedHousehold) {
      householdStore.setCurrentHousehold(selectedHousehold)
    }
  }
}

const onSetupCompleted = async () => {
  showSetupWizard.value = false
  // Reload households to include the new one
  await householdStore.loadHouseholds()
  // Set the selected value to match the current household (should be the new one)
  selectedHouseholdId.value = currentHouseholdId.value
}

const handleMemberInvited = () => {
  showInviteDialog.value = false
  // Refresh the members list to show the updated data
  loadHouseholdMembers()
}

const confirmDeleteMember = (member: HouseholdMemberDTO) => {
  const memberName =
    member.userFirstName && member.userLastName
      ? `${member.userFirstName} ${member.userLastName}`
      : member.userEmail
  confirm.require({
    message: `Are you sure you want to remove ${memberName} from this household?`,
    header: 'Remove Member',
    icon: 'pi pi-exclamation-triangle',
    rejectProps: {
      label: 'Cancel',
      severity: 'secondary',
      outlined: true,
    },
    acceptProps: {
      label: 'Remove',
      severity: 'danger',
    },
    accept: () => {
      deleteMember(member)
    },
  })
}

const deleteMember = async (member: HouseholdMemberDTO) => {
  if (!householdStore.currentHousehold) return

  try {
    await householdApi.removeHouseholdMember(
      householdStore.currentHousehold.householdId,
      member.userEmail,
    )

    toast.add({
      severity: 'success',
      summary: 'Member Removed',
      detail: `${member.userEmail} has been removed from the household`,
      life: 3000,
    })

    // Refresh the members list
    loadHouseholdMembers()
  } catch (error: any) {
    console.error('Failed to remove member:', error)

    let errorMessage = 'Failed to remove member. Please try again.'

    if (error.response?.status === 404) {
      errorMessage = 'Member not found or already removed.'
    } else if (error.response?.status === 403) {
      errorMessage = 'You do not have permission to remove this member.'
    }

    toast.add({
      severity: 'error',
      summary: 'Remove Failed',
      detail: errorMessage,
      life: 5000,
    })
  }
}

// Pending household invites state and actions
const pendingInvites = ref<HouseholdMemberDTO[]>([])
const loadingPendingInvites = ref<boolean>(false)
const processingInvite = ref<string | null>(null)

const loadPendingInvites = async () => {
  try {
    loadingPendingInvites.value = true
    pendingInvites.value = await householdApi.getPendingHouseholdInvites()
  } catch (error) {
    console.error('Failed to load pending invites:', error)
    pendingInvites.value = []
  } finally {
    loadingPendingInvites.value = false
  }
}

const acceptInviteAction = async (householdId: string) => {
  try {
    processingInvite.value = householdId
    await householdApi.acceptInvite(householdId)

    toast.add({
      severity: 'success',
      summary: 'Invite Accepted',
      detail: 'You have joined the household',
      life: 3000,
    })

    // Refresh households and pending invites
    await householdStore.loadHouseholds()
    await loadPendingInvites()
  } catch (error) {
    console.error('Failed to accept invite:', error)
    toast.add({
      severity: 'error',
      summary: 'Accept Failed',
      detail: 'Unable to accept invite',
      life: 5000,
    })
  } finally {
    processingInvite.value = null
  }
}

const rejectInviteAction = async (invite: HouseholdMemberDTO) => {
  if (!invite) return
  try {
    processingInvite.value = invite.householdId

    await householdApi.rejectHouseholdInvite(invite.householdId)

    toast.add({
      severity: 'success',
      summary: 'Invite Rejected',
      detail: 'Invite has been rejected',
      life: 3000,
    })

    await loadPendingInvites()
  } catch (error) {
    console.error('Failed to reject invite:', error)
    toast.add({
      severity: 'error',
      summary: 'Reject Failed',
      detail: 'Unable to reject invite',
      life: 5000,
    })
  } finally {
    processingInvite.value = null
  }
}

// Expose HouseholdMemberRole and HouseholdMemberStatus to template
const { Owner, Admin, Member } = HouseholdMemberRole
const { PendingRespone, Expired, Rejected, Active } = HouseholdMemberStatus

const sortedHouseholds = computed(() => {
  if (!householdStore.households) return []
  return [...householdStore.households].sort((a, b) => {
    if (a.role === HouseholdMemberRole.Owner && b.role !== HouseholdMemberRole.Owner) {
      return -1
    } else if (a.role !== HouseholdMemberRole.Owner && b.role === HouseholdMemberRole.Owner) {
      return 1
    }
    return 0
  })
})
</script>
