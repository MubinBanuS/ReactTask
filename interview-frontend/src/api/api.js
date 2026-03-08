import { apiRequest } from "./apiService";
import { API_URL , ENDPOINTS } from "./apiConstants";

export const startPlan =  () => apiRequest(`${API_URL}${ENDPOINTS.START_PLAN}`, {method: "POST",body: {}});
export const addProcedureToPlan = (planId, procedureId) => apiRequest(`${API_URL}${ENDPOINTS.ADD_PROCEDURE_TO_PLAN}`, {method: "POST", body: { planId, procedureId }});
export const getProcedures = () => apiRequest(`${API_URL}${ENDPOINTS.PROCEDURES}`);
export const getPlanProcedures = (planId) => apiRequest(`${API_URL}${ENDPOINTS.PLAN_PROCEDURES(planId)}`);
export const getUsers = () => apiRequest(`${API_URL}${ENDPOINTS.USERS}`);
export const addUserToPlanProcedure = (planId, procedureId, userId) => apiRequest(`${API_URL}${ENDPOINTS.ADD_USER_TO_PLAN_PROCEDURE}`, {method: "POST", body: { planId, procedureId , userId}});
export const removeUserFromPlanProcedure = (planId, procedureId, userId) => apiRequest(`${API_URL}${ENDPOINTS.REMOVE_USER_FROM_PLAN_PROCEDURE}`, {method: "DELETE", body: { planId, procedureId , userId}});
export const removeAllUsersFromPlanProcedure = (planId, procedureId) => apiRequest(`${API_URL}${ENDPOINTS.REMOVE_ALL_USERS_FROM_PLAN_PROCEDURE}`, {method: "DELETE", body: { planId, procedureId }});