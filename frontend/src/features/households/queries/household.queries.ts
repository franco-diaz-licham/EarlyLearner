import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { mapAddChildFormToRequest, mapHouseholdResponseToModel, mapHouseholdResponsesToModels, mapInviteCarerFormToRequest, mapRenameHouseholdFormToRequest, mapUpdateChildFormToRequest } from '../mappers/household.mapper';
import { householdService } from '../services/household.services';
import type { AddChildForm, InviteCarerForm, RenameHouseholdForm } from '../types/household.types';

export const householdKeys = {
  all: ['households'] as const,
  lists: () => [...householdKeys.all, 'list'] as const,
  list: () => [...householdKeys.lists()] as const,
  details: () => [...householdKeys.all, 'detail'] as const,
  detail: (householdId: string) => [...householdKeys.details(), householdId] as const
};

export const useHouseholdsQuery = () =>
  useQuery({
    queryKey: householdKeys.list(),
    queryFn: async () => {
      const households = await householdService.list();
      return mapHouseholdResponsesToModels(households);
    }
  });

export const useHouseholdQuery = (householdId: string) =>
  useQuery({
    queryKey: householdKeys.detail(householdId),
    queryFn: async () => {
      const household = await householdService.get(householdId);
      return mapHouseholdResponseToModel(household);
    },
    enabled: Boolean(householdId)
  });

export const useUpdateHouseholdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, form }: { householdId: string; form: RenameHouseholdForm }) => {
      const household = await householdService.update(householdId, mapRenameHouseholdFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useInviteHouseholdCarerMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, form }: { householdId: string; form: InviteCarerForm }) => {
      const household = await householdService.inviteCarer(householdId, mapInviteCarerFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useRemoveHouseholdCarerMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, carerId }: { householdId: string; carerId: string }) => {
      const household = await householdService.removeCarer(householdId, carerId);
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useAddHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, form }: { householdId: string; form: AddChildForm }) => {
      const household = await householdService.addChild(householdId, mapAddChildFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useRemoveHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, childId }: { householdId: string; childId: string }) => {
      const household = await householdService.removeChild(householdId, childId);
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useUploadHouseholdChildAvatarMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, childId, file }: { householdId: string; childId: string; file: File }) => {
      const household = await householdService.uploadChildAvatar(householdId, childId, file);
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};

export const useUpdateHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ householdId, childId, form }: { householdId: string; childId: string; form: AddChildForm }) => {
      const household = await householdService.updateChild(householdId, childId, mapUpdateChildFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      queryClient.setQueryData(householdKeys.detail(household.id), household);
    }
  });
};
