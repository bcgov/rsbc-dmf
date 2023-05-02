export const PidpStateName = {
  dashboard: 'dashboard',
};
export interface NamedState {
  stateName: string;
}
export interface DashboardStateModel extends NamedState {
  titleText: string;
  titleDescriptionText: string;
  userProfileFullNameText: string;
  userProfileRoleNameText: string;
}
export const defaultDashboardState: DashboardStateModel = {
  stateName: PidpStateName.dashboard,
  titleText: '',
  titleDescriptionText: '',
  userProfileFullNameText: '',
  userProfileRoleNameText: '',
};
export interface ApplicationStateModel {
  all: NamedState[];
}
export const defaultApplicationState: ApplicationStateModel = {
  all: [{ ...defaultDashboardState }],
};
