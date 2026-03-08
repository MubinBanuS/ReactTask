import { useMemo } from "react";

const ProcedureItem = ({
  procedure,
  planProcedures,
  handleAddProcedureToPlan
}) => {

  const isChecked = useMemo(() => {
    return planProcedures.some(
      p => p.procedure?.procedureId === procedure.procedureId
    );
  }, [planProcedures, procedure.procedureId]);

  return (
    <div className="py-2">

      <div className="form-check">

        <input
          className="form-check-input"
          type="checkbox"
          id={`procedure-${procedure.procedureId}`}
          checked={isChecked}
          disabled={isChecked}
          onChange={async () => { try { await handleAddProcedureToPlan(procedure); } catch {} }}
        />

        <label
          className="form-check-label"
          htmlFor={`procedure-${procedure.procedureId}`}
        >
          {procedure.procedureTitle}
        </label>

      </div>

    </div>
  );
};

export default ProcedureItem;