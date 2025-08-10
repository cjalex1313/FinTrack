import { createRouter, createWebHistory } from 'vue-router'
import { jwtDecode } from 'jwt-decode'
import HomeView from '../views/HomeView.vue'
import BaseLayout from '@/layouts/BaseLayout.vue'
import AuthLayout from '@/layouts/AuthLayout.vue'
import LoginView from '@/views/auth/LoginView.vue'

const isGuid = (guid: string): boolean => {
  // Return false for non-string or empty inputs
  if (!guid || typeof guid !== 'string') {
    return false
  }

  const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i

  return guidRegex.test(guid)
}

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'BaseLayout',
      meta: { requireAuth: true },
      component: BaseLayout,
      children: [
        {
          path: '/',
          name: 'Home',
          component: HomeView,
        },
        {
          path: '/incomes',
          name: 'Incomes',
          component: () => import('@/views/IncomeView.vue'),
        },
        {
          path: '/expense-buckets',
          name: 'ExpenseBuckets',
          component: () => import('@/views/ExpenseBucketView.vue'),
        },
        {
          path: '/expenses',
          name: 'Expenses',
          component: () => import('@/views/ExpensesView.vue'),
        },
        {
          path: '/recurring-expenses',
          name: 'RecurringExpenses',
          component: () => import('@/views/RecurringExpensesView.vue'),
        },
        {
          path: '/profile',
          name: 'Profile',
          component: () => import('@/views/ProfileView.vue'),
        },
      ],
    },
    {
      path: '/auth',
      name: 'AuthLayout',
      component: AuthLayout,
      children: [
        {
          path: '',
          name: 'Login',
          component: LoginView,
        },
        {
          path: 'register',
          name: 'Register',
          component: () => import('@/views/auth/RegisterView.vue'),
        },
        {
          path: 'confirm-email',
          name: 'ConfirmEmail',
          component: () => import('@/views/auth/ConfirmEmailView.vue'),
        },
        {
          path: 'forgot-password',
          name: 'ForgotPassword',
          component: () => import('@/views/auth/ForgotPasswordView.vue'),
        },
        {
          path: 'set-forgot-password',
          name: 'ResetPassword',
          component: () => import('@/views/auth/ResetPasswordView.vue'),
        },
      ],
    },
    {
      path: '/admin',
      name: 'AdminLayout',
      component: () => import('@/layouts/AdminLayout.vue'),
      children: [
        {
          path: '',
          name: 'AdminDashboard',
          component: () => import('@/views/admin/AdminView.vue'),
        },
      ],
    },
  ],
})

router.beforeEach((to, from, next) => {
  const jwt = localStorage.getItem('jwt')
  if (to.meta.requireAuth && !jwt) {
    next({ name: 'Login' })
    return
  }
  if (!to.meta.requiresAuth && jwt && to.path.startsWith('/auth') && to.name != 'ConfirmEmail') {
    next({ name: 'Home' })
    return
  }
  if (to.meta.requireAdmin) {
    if (!jwt) {
      next({ name: 'Login' })
      return
    }
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const decoded: any = jwtDecode(jwt)
    if (!decoded?.roles?.includes('Admin')) {
      next({ name: 'Login' })
      return
    }
  }
  next()
})

export default router
