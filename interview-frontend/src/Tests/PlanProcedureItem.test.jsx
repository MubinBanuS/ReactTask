import React from "react";
import { render, screen, fireEvent, waitFor, act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import PlanProcedureItem from "../components/Plan/PlanProcedureItem/PlanProcedureItem";

describe("PlanProcedureItem component", () => {
  const procedure = {
    procedureId: "p1",
    procedureTitle: "Procedure 1"
  };

  const users = [
    { label: "Alice", value: "u1" },
    { label: "Bob", value: "u2" }
  ];

  const planProcedures = {
    planProcedureUsers: [
      { user: { name: "Alice", userId: "u1" } }
    ]
  };

  it("renders procedure title and select component", () => {
    act(() => {
      render(
        <PlanProcedureItem
          procedure={procedure}
          users={users}
          planProcedures={planProcedures}
          handleAddUserToPlanProcedure={jest.fn()}
          handleRemoveUserFromPlanProcedure={jest.fn()}
        />
      );
    });

    expect(screen.getByText("Procedure 1")).toBeInTheDocument();
    expect(screen.getByRole("combobox")).toBeInTheDocument();
  });

  it("calls handleAddUserToPlanProcedure when a user is added", async () => {
    const add = jest.fn();

    act(() => {
      render(
        <PlanProcedureItem
          procedure={procedure}
          users={users}
          planProcedures={planProcedures}
          handleAddUserToPlanProcedure={add}
          handleRemoveUserFromPlanProcedure={jest.fn()}
        />
      );
    });

    await act(async () => {
      const input = screen.getByRole("combobox");
      await userEvent.type(input, "Bob");
    });

    await act(async () => {
      const option = await screen.findByText("Bob");
      await userEvent.click(option);
    });

    expect(add).toHaveBeenCalledWith("p1", "u2");
  });

  it("calls handleRemoveUserFromPlanProcedure when a user is removed", async () => {
    const remove = jest.fn();

    act(() => {
      render(
        <PlanProcedureItem
          procedure={procedure}
          users={users}
          planProcedures={planProcedures}
          handleAddUserToPlanProcedure={jest.fn()}
          handleRemoveUserFromPlanProcedure={remove}
        />
      );
    });

    // Click the remove button for the first selected user (Alice)
    await act(async () => {
      const removeButton = screen.getByLabelText("Remove Alice");
      await userEvent.click(removeButton);
    });

    expect(remove).toHaveBeenCalledWith("p1", "u1");
  });

  it("displays initially selected users from planProcedures", () => {
    act(() => {
      render(
        <PlanProcedureItem
          procedure={procedure}
          users={users}
          planProcedures={planProcedures}
          handleAddUserToPlanProcedure={jest.fn()}
          handleRemoveUserFromPlanProcedure={jest.fn()}
        />
      );
    });

    expect(screen.getByText("Alice")).toBeInTheDocument();
  });
});