import { create } from "zustand";

interface AuthState {
  isAuthenticated: boolean;
  login: () => void;
}

const useAuthStore = create<AuthState>((set) => ({
  isAuthenticated: false,
  login: () => {
    set({ isAuthenticated: true });
  },
}));

export { useAuthStore };
