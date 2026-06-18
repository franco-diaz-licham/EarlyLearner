import { create } from 'zustand';

interface CurrentUserModel {
  displayName: string;
  organisationName: string;
  roleLabel: string;
}

interface SessionState {
  currentUser: CurrentUserModel;
  setCurrentUser: (currentUser: CurrentUserModel) => void;
}

export const useSessionStore = create<SessionState>((set) => ({
  currentUser: {
    displayName: 'Early learner team',
    organisationName: 'Bright Start Centre',
    roleLabel: 'Educator'
  },
  setCurrentUser: (currentUser) => {
    set({ currentUser });
  }
}));
