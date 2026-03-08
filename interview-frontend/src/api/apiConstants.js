// Base API URL
export const API_URL = "http://localhost:10010/api/v1";
// Endpoint paths
export const ENDPOINTS = {
  START_PLAN: "/Plan",
  PROCEDURES: "/Procedures",
  PLAN_PROCEDURES: (planId) => `/PlanProcedure?$filter=planId eq ${planId}&$expand=procedure,planProcedureUsers($expand=user)`,
  ADD_PROCEDURE_TO_PLAN: "/Plan/AddProcedureToPlan",
  USERS: "/Users",
  ADD_USER_TO_PLAN_PROCEDURE: "/PlanProcedure/AddUser",
  REMOVE_USER_FROM_PLAN_PROCEDURE: "/PlanProcedure/RemoveUser",
  REMOVE_ALL_USERS_FROM_PLAN_PROCEDURE: "/PlanProcedure/RemoveAllUsers"
};

// Other constants
export const Toast_DURATION = 2000; // 2 seconds