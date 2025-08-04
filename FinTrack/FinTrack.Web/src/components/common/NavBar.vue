<template>
  <nav class="bg-gray-800">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="relative flex h-16 items-center justify-between">
        <div class="absolute inset-y-0 left-0 flex items-center sm:hidden">
          <!-- Mobile menu button-->
          <button
            @click="isMobileNavOpen = !isMobileNavOpen"
            type="button"
            class="relative inline-flex items-center justify-center rounded-md p-2 text-gray-400 hover:bg-gray-700 hover:text-white focus:ring-2 focus:ring-white focus:outline-hidden focus:ring-inset"
            aria-controls="mobile-menu"
            aria-expanded="false"
          >
            <span class="absolute -inset-0.5"></span>
            <span class="sr-only">Open main menu</span>
            <!--
            Icon when menu is closed.

            Menu open: "hidden", Menu closed: "block"
          -->
            <svg
              class="block size-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke-width="1.5"
              stroke="currentColor"
              aria-hidden="true"
              data-slot="icon"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5"
              />
            </svg>
            <!--
            Icon when menu is open.

            Menu open: "block", Menu closed: "hidden"
          -->
            <svg
              class="hidden size-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke-width="1.5"
              stroke="currentColor"
              aria-hidden="true"
              data-slot="icon"
            >
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <div class="flex flex-1 items-center justify-center sm:items-stretch sm:justify-start">
          <div class="flex shrink-0 items-center">
            <img
              class="h-8 w-auto"
              src="https://tailwindcss.com/plus-assets/img/logos/mark.svg?color=indigo&shade=500"
              alt="Your Company"
            />
          </div>
          <div class="hidden sm:ml-6 sm:block">
            <div class="flex space-x-4">
              <!-- Current: "bg-gray-900 text-white", Default: "text-gray-300 hover:bg-gray-700 hover:text-white" -->
              <RouterLink
                to="/"
                class="rounded-md px-3 py-2 text-sm font-medium text-gray-300 hover:bg-gray-700 hover:text-white"
                aria-current="page"
                exactActiveClass="bg-gray-900 text-white"
                >Dashboard</RouterLink
              >
              <RouterLink
                v-if="householdStore.currentHousehold"
                to="/incomes"
                class="rounded-md px-3 py-2 text-sm font-medium text-gray-300 hover:bg-gray-700 hover:text-white"
                exactActiveClass="bg-gray-900 text-white"
                >Incomes</RouterLink
              >
              <!-- <RouterLink
                to="/skills-library"
                class="rounded-md px-3 py-2 text-sm font-medium text-gray-300 hover:bg-gray-700 hover:text-white"
                activeClass="bg-gray-900 text-white"
                >Skill Library</RouterLink
              > -->
            </div>
          </div>
        </div>
        <div
          class="absolute inset-y-0 right-0 flex items-center pr-2 sm:static sm:inset-auto sm:ml-6 sm:pr-0"
        >
          <!-- Profile dropdown -->
          <div class="relative ml-3">
            <div>
              <button
                type="button"
                @click="toggleMenu"
                class="relative flex rounded-full bg-gray-800 text-sm"
                id="user-menu-button"
                aria-expanded="false"
                aria-haspopup="true"
              >
                <span class="absolute -inset-1.5"></span>
                <span class="sr-only">Open user menu</span>
                <span
                  class="w-8 h-8 pi pi-user bg-slate-200 rounded-full"
                  style="font-size: 1.5rem"
                ></span>
              </button>
              <Menu ref="menu" :model="menuItems" :popup="true" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Mobile menu, show/hide based on menu state. -->
    <div v-if="isMobileNavOpen" class="sm:hidden" id="mobile-menu">
      <div class="space-y-1 px-2 pt-2 pb-3">
        <!-- Current: "bg-gray-900 text-white", Default: "text-gray-300 hover:bg-gray-700 hover:text-white" -->
        <RouterLink
          to="/"
          class="block rounded-md text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 text-base font-medium"
          aria-current="page"
          exactActiveClass="bg-gray-900 text-white"
          >Dashboard</RouterLink
        >
        <RouterLink
          v-if="householdStore.currentHousehold"
          to="/incomes"
          class="block rounded-md text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 text-base font-medium"
          exactActiveClass="bg-gray-900 text-white"
          >Incomes</RouterLink
        >
        <!-- <RouterLink
          to="/skills-library"
          class="block rounded-md text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 text-base font-medium"
          activeClass="bg-gray-900 text-white"
          >Skill Library</RouterLink
        > -->
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useHouseholdStore } from '@/stores/household'
import { Menu } from 'primevue'
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const menu: any = ref(null)
const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()
const householdStore = useHouseholdStore()

const menuItems = [
  {
    label: 'Sign out',
    command: async () => {
      await authStore.signOut()
      router.push({
        name: 'Login',
      })
    },
  },
]

const isMobileNavOpen = ref(false)

watch(route, () => {
  isMobileNavOpen.value = false
})

const toggleMenu = (event: any) => {
  // Use the menu ref's toggle method, passing the browser event
  // This tells the menu where to position itself (relative to the button)
  if (menu.value) {
    menu.value.toggle(event)
  }
}
</script>
