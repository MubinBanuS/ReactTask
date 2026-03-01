import React, { act } from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import ProcedureItem from '../components/Plan/ProcedureItem/ProcedureItem';

describe('ProcedureItem component', () => {
  const procedure = { procedureId: '1', procedureTitle: 'Test Procedure' };

  it('displays the title and unchecked box when not in plan', () => {
    const handleAdd = jest.fn();
    act(() => {
      render(
        <ProcedureItem
          procedure={procedure}
          handleAddProcedureToPlan={handleAdd}
          planProcedures={[]}
        />
      );
    });

    const label = screen.getByText(/Test Procedure/i);
    expect(label).toBeInTheDocument();

    const checkbox = screen.getByRole('checkbox');
    expect(checkbox).not.toBeChecked();

    fireEvent.click(checkbox);
    expect(handleAdd).toHaveBeenCalledWith(procedure);
  });

  it('shows checked when procedure is already in plan', () => {
    const handleAdd = jest.fn();
    const planProcedures = [{ procedureId: '1' }];
    render(
      <ProcedureItem
        procedure={procedure}
        handleAddProcedureToPlan={handleAdd}
        planProcedures={planProcedures}
      />
    );

    const checkbox = screen.getByRole('checkbox');
    expect(checkbox).toBeChecked();
  });
});
