<script setup lang="ts">
import { RouterView, useRoute, useRouter } from 'vue-router'
import { Menu, Button } from 'primevue'
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'

const menu: any = ref(null)
const authStore = useAuthStore()
const route = useRoute()
const router = useRouter()

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

const toggleMenu = (event: any) => {
  // Use the menu ref's toggle method, passing the browser event
  // This tells the menu where to position itself (relative to the button)
  if (menu.value) {
    menu.value.toggle(event)
  }
}

const goBack = () => {
  router.back()
}
</script>

<template>
  <div class="min-h-full">
    <div>
      <!-- Static sidebar for desktop -->
      <div class="fixed inset-y-0 z-50 flex w-48 lg:w-72 flex-col">
        <!-- Sidebar component, swap this element with another sidebar if you like -->
        <div class="flex grow flex-col gap-y-5 overflow-y-auto bg-indigo-600 px-6 pb-4">
          <div class="flex h-16 shrink-0 items-center">
            <img
              src="https://tailwindcss.com/plus-assets/img/logos/mark.svg?color=white"
              alt="Your Company"
              class="h-8 w-auto"
            />
            <div class="ml-2 lg:ml-8 text-white font-semibold">Admin portal</div>
          </div>
          <nav class="flex flex-1 flex-col">
            <ul role="list" class="flex flex-1 flex-col gap-y-7">
              <li>
                <ul role="list" class="-mx-2 space-y-1">
                  <li>
                    <!-- Current: "bg-indigo-700 text-white", Default: "text-indigo-200 hover:text-white hover:bg-indigo-700" -->
                    <!-- <a
                      href="#"
                      class="group flex gap-x-3 rounded-md bg-indigo-700 p-2 text-sm/6 font-semibold text-white"
                    >
                      <svg
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="1.5"
                        data-slot="icon"
                        aria-hidden="true"
                        class="size-6 shrink-0 text-white"
                      >
                        <path
                          d="m2.25 12 8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25"
                          stroke-linecap="round"
                          stroke-linejoin="round"
                        />
                      </svg>
                      Dashboard
                    </a> -->
                    <RouterLink
                      to="/admin"
                      class="flex items-center gap-x-3 rounded-md p-2 text-sm/6 font-semibold text-indigo-200 hover:bg-indigo-700 hover:text-white"
                      aria-current="page"
                      exactActiveClass="bg-indigo-700 text-white"
                    >
                      <i class="pi pi-search" style="font-size: 1rem"></i>
                      Dashboard
                    </RouterLink>
                  </li>
                  <li>
                    <RouterLink
                      to="/admin/curriculum"
                      class="flex items-center gap-x-3 rounded-md p-2 text-sm/6 font-semibold text-indigo-200 hover:bg-indigo-700 hover:text-white"
                      aria-current="page"
                      exactActiveClass="bg-indigo-700 text-white"
                    >
                      <i class="pi pi-bars" style="font-size: 1rem"></i>
                      Curriculum
                    </RouterLink>
                  </li>
                </ul>
              </li>
            </ul>
          </nav>
        </div>
      </div>

      <div class="pl-48 lg:pl-72">
        <div
          class="sticky top-0 z-40 flex justify-between h-16 shrink-0 items-center gap-x-4 border-b border-gray-200 bg-white px-4 shadow-xs sm:gap-x-6 sm:px-6 lg:px-8"
        >
          <div>
            <Button
              v-if="route.meta.goBack"
              @click="goBack"
              severity="secondary"
              icon="pi pi-angle-left"
            ></Button>
          </div>
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

        <main class="py-10">
          <div class="px-4 sm:px-6 lg:px-8">
            <RouterView :key="route.fullPath" />
          </div>
        </main>
      </div>
    </div>
  </div>
</template>
