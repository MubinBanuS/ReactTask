import { useEffect, useState, useRef } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ procedure, users, planProcedures, handleAddUserToPlanProcedure, handleRemoveUserFromPlanProcedure}) => {
    const [selectedUsers, setSelectedUsers] = useState([]);
    const prevSelectedUsersRef = useRef([]);
    useEffect(() => {
        const initialSelectedUsers = !planProcedures || !Array.isArray(planProcedures.planProcedureUsers)
                ? []
                : planProcedures.planProcedureUsers
                      .filter(ppu => ppu && ppu.user)
                      .map(ppu => ({ label: ppu.user.name, value: ppu.user.userId }));;
        setSelectedUsers(initialSelectedUsers);
        prevSelectedUsersRef.current = initialSelectedUsers;
    }, [planProcedures]);
    const handleAssignUserToProcedure =  (newSelected = [], actionMeta) => {
        setSelectedUsers(newSelected);
        const prevSelectedUserIds = prevSelectedUsersRef.current.map(u => u.value);
        const newSelectedUserIds = newSelected.map(s => s.value);
        // Adds users 
        if(actionMeta.action === "select-option" || actionMeta.action === "select-value") 
        {
            const usersToAdd = newSelectedUserIds.filter(id => !prevSelectedUserIds.includes(id));
            usersToAdd.forEach(userId => handleAddUserToPlanProcedure(procedure.procedureId, userId));  
        }
        if(actionMeta.action === "clear"){
            handleRemoveUserFromPlanProcedure(procedure.procedureId); 
        }
        if(actionMeta.action === "remove-value" || actionMeta.action === "deselect-option" || actionMeta.action === "pop-value") 
        {
            const usersToRemove = prevSelectedUserIds.filter(id => !newSelectedUserIds.includes(id));           
            if(usersToRemove?.length > 0)    {
                usersToRemove.forEach(userId => handleRemoveUserFromPlanProcedure(procedure.procedureId, userId)); 
            }  
        }                   
        prevSelectedUsersRef.current = newSelected;
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={handleAssignUserToProcedure}
            />
        </div>
    );
};

export default PlanProcedureItem;
