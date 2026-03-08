import { useEffect, useState, useRef, useMemo } from "react";
import ReactSelect from "react-select";
import { useApiAction } from "../../../hooks/useApiAction";
import { removeAllUsersFromPlanProcedure, getPlanProcedures } from "../../../api/api";

const PlanProcedureItem = ({
  planId,
  procedure,
  users,
  planProcedures,
  handleAddUserToPlanProcedure,
  handleRemoveUserFromPlanProcedure,
  setPlanProcedures
}) => {

  const [selectedUsers, setSelectedUsers] = useState([]);
  const prevSelectedUsersRef = useRef([]);

  const userCount = planProcedures?.planProcedureUsers?.length ?? 0;

  const initialSelectedUsers = useMemo(() => {
    if (!planProcedures?.planProcedureUsers) return [];

    return planProcedures.planProcedureUsers
      .filter(ppu => ppu?.user)
      .map(ppu => ({
        label: ppu.user.name,
        value: ppu.user.userId
      }));

  }, [planProcedures?.planProcedureUsers]);

  useEffect(() => {
    setSelectedUsers(initialSelectedUsers);
    prevSelectedUsersRef.current = initialSelectedUsers;
  }, [initialSelectedUsers]);

  const removeAllUsersAction = useApiAction({
    apiCall: removeAllUsersFromPlanProcedure,
    successMessage: "All users removed successfully",
    errorMessage: "Failed to remove all users",
    onSuccess: async () => {
      const updated = await getPlanProcedures(planId);
      setPlanProcedures(updated);
    },
    throwOnError: true
  });

  const handleAssignUserToProcedure = async (newSelected = [], actionMeta) => {

    const prevIds = prevSelectedUsersRef.current.map(u => u.value);
    const newIds = newSelected.map(u => u.value);

    if (["select-option", "select-value"].includes(actionMeta.action)) {

      const usersToAdd = newIds.filter(id => !prevIds.includes(id));

      try {
        await Promise.all(
          usersToAdd.map(id =>
            handleAddUserToPlanProcedure(procedure.procedureId, id)
          )
        );

        setSelectedUsers(newSelected);
        prevSelectedUsersRef.current = newSelected;

      } catch {}
    }

    if (actionMeta.action === "clear") {
      removeAllUsersAction.execute(planId, procedure.procedureId);
    }

    if (["remove-value", "deselect-option", "pop-value"].includes(actionMeta.action)) {

      const usersToRemove = prevIds.filter(id => !newIds.includes(id));

      try {
        await Promise.all(
          usersToRemove.map(id =>
            handleRemoveUserFromPlanProcedure(procedure.procedureId, id)
          )
        );

        setSelectedUsers(newSelected);
        prevSelectedUsersRef.current = newSelected;

      } catch {}
    }
  };

  return (
    <div className="py-2 border-bottom mb-2">

      <div className="fw-bold">
        {procedure.procedureTitle}
      </div>

      <ReactSelect
        className="mt-2"
        placeholder="Select User to Assign"
        isMulti
        options={users}
        value={selectedUsers}
        onChange={handleAssignUserToProcedure}
      />

      <div className="mt-2 d-flex align-items-center gap-3">

        <p className="mb-0">
          Users Assigned: {userCount}
        </p>

        <button
          className="btn btn-danger btn-sm d-flex align-items-center gap-2"
          onClick={async () => {
            try {
              await removeAllUsersAction.execute(planId, procedure.procedureId);
            } catch {}
          }}
          disabled={removeAllUsersAction.loading || userCount === 0}
        >
          {removeAllUsersAction.loading && (
            <span className="spinner-border spinner-border-sm" />
          )}

          {removeAllUsersAction.loading
            ? "Removing..."
            : "Remove All Users"}
        </button>

      </div>
    </div>
  );
};

export default PlanProcedureItem;