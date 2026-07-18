import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { mapAddChildFormToRequest, mapHouseholdResponseToModel, mapHouseholdResponsesToModels, mapInviteCarerFormToRequest, mapRenameHouseholdFormToRequest, mapUpdateChildFormToRequest } from '../mappers/household.mapper';
import { householdService } from '../services/household.services';
import type { AddChildForm, HouseholdModel, InviteCarerForm, RenameHouseholdForm } from '../types/household.types';

interface UpdateHouseholdChild {
  childId: string;
  form: AddChildForm;
}

export const householdKeys = {
  all: ['households'] as const,
  lists: () => [...householdKeys.all, 'list'] as const,
  list: () => [...householdKeys.lists()] as const,
  details: () => [...householdKeys.all, 'detail'] as const,
  current: () => [...householdKeys.details(), 'current'] as const
};

export const useHouseholdsQuery = () =>
  useQuery({
    queryKey: householdKeys.list(),
    queryFn: async () => {
      const households = await householdService.list();
      return mapHouseholdResponsesToModels(households);
    }
  });

export const useHouseholdQuery = () =>
  useQuery({
    queryKey: householdKeys.current(),
    queryFn: async () => {
      const household = await householdService.get();
      return mapHouseholdResponseToModel(household);
    }
  });

const setHouseholdQueryData = (queryClient: ReturnType<typeof useQueryClient>, household: HouseholdModel) => {
  queryClient.setQueryData<HouseholdModel[]>(householdKeys.list(), (current) => {
    if (!current) return [household];
    if (!current.some((item) => item.id === household.id)) return [...current, household];
    return current.map((item) => (item.id === household.id ? household : item));
  });
  queryClient.setQueryData(householdKeys.current(), household);
};

export const useUpdateHouseholdMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (form: RenameHouseholdForm) => {
      const household = await householdService.update(mapRenameHouseholdFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};

export const useInviteHouseholdCarerMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (form: InviteCarerForm) => {
      const household = await householdService.inviteCarer(mapInviteCarerFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};

export const useRemoveHouseholdCarerMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (carerId: string) => {
      const household = await householdService.removeCarer(carerId);
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};

export const useRevokeHouseholdCarerInvitationMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (invitationId: string) => {
      const household = await householdService.revokeCarerInvitation(invitationId);
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};

export const useAddHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (form: AddChildForm) => {
      const household = await householdService.addChild(mapAddChildFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};

export const useRemoveHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (childId: string) => {
      const household = await householdService.removeChild(childId);
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};

export const useUpdateHouseholdChildMutation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ childId, form }: UpdateHouseholdChild) => {
      const household = await householdService.updateChild(childId, mapUpdateChildFormToRequest(form));
      return mapHouseholdResponseToModel(household);
    },
    onSuccess: (household) => {
      void queryClient.invalidateQueries({ queryKey: householdKeys.lists() });
      setHouseholdQueryData(queryClient, household);
    }
  });
};
