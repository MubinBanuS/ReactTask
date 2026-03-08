import { useState, useEffect, useMemo, useCallback } from "react";
import { useParams } from "react-router-dom";
import { toast } from "react-toastify";

import {
  addProcedureToPlan,
  getPlanProcedures,
  getProcedures,
  getUsers,
  addUserToPlanProcedure,
  removeUserFromPlanProcedure
} from "../../api/api";

import Layout from "../Layout/Layout";
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";
import { useApiAction } from "../../hooks/useApiAction";

const Plan = () => {
  const { id } = useParams();

  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [usersRaw, setUsersRaw] = useState([]);
  const [loading, setLoading] = useState(true);

  // Derived value memoization
  const users = useMemo(
    () => usersRaw.map(u => ({ label: u.name, value: u.userId })),
    [usersRaw]
  );

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);

      try {
        const [proceduresData, planProceduresData, usersData] = await Promise.all([
          getProcedures(),
          getPlanProcedures(id),
          getUsers()
        ]);

        setProcedures(proceduresData);
        setPlanProcedures(planProceduresData);
        setUsersRaw(usersData);
      } catch (error) {
        console.error("Error loading data:", error);
        toast.error(
          "Failed to load data. Please check if the API is running.",
          { toastId: "load-error" }
        );
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [id]);

  const refreshPlanProcedures = useCallback(async () => {
    const updated = await getPlanProcedures(id);
    setPlanProcedures(updated);
  }, [id]);

  const addProcedure = useApiAction({
    apiCall: addProcedureToPlan,
    successMessage: "Procedure added to plan successfully",
    errorMessage: "Failed to add procedure to plan",
    onSuccess: refreshPlanProcedures,
    throwOnError: true
  });

  const addUser = useApiAction({
    apiCall: addUserToPlanProcedure,
    successMessage: "User added to procedure successfully",
    errorMessage: "Failed to add user to procedure",
    onSuccess: refreshPlanProcedures,
    throwOnError: true
  });

  const removeUser = useApiAction({
    apiCall: removeUserFromPlanProcedure,
    successMessage: "User removed from procedure successfully",
    errorMessage: "Failed to remove user from procedure",
    onSuccess: refreshPlanProcedures,
    throwOnError: true
  });

  // Stable handlers
  const handleAddProcedureToPlan = (procedure) => {
    addProcedure.execute(id, procedure.procedureId);
  };

  const handleAddUserToPlanProcedure = (procedureId, userId) => {
    addUser.execute(id, procedureId, userId);
  };

  const handleRemoveUserFromPlanProcedure = (procedureId, userId) => {
    removeUser.execute(id, procedureId, userId);
  };

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>

        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>

              <div className="card-body">
                {loading ? (
                  <div className="text-center">
                    <div className="spinner-border" role="status">
                      <span className="visually-hidden">Loading...</span>
                    </div>
                    <p>Loading plan data...</p>
                  </div>
                ) : (
                  <div className="row">
                    {/* PROCEDURES */}
                    <div className="col">
                      <h4>Procedures</h4>

                      {procedures.map((p) => (
                        <ProcedureItem
                          key={p.procedureId}
                          procedure={p}
                          planProcedures={planProcedures}
                          handleAddProcedureToPlan={handleAddProcedureToPlan}
                        />
                      ))}
                    </div>

                    {/* PLAN PROCEDURES */}
                    <div className="col">
                      <h4>Added to Plan</h4>

                      {planProcedures.map((p) => (
                        <PlanProcedureItem
                          key={p.procedure.procedureId}
                          planId={id}
                          procedure={p.procedure}
                          users={users}
                          planProcedures={p}
                          handleAddUserToPlanProcedure={handleAddUserToPlanProcedure}
                          handleRemoveUserFromPlanProcedure={handleRemoveUserFromPlanProcedure}
                          setPlanProcedures={setPlanProcedures}
                        />
                      ))}
                    </div>
                  </div>
                )}
              </div>

            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;