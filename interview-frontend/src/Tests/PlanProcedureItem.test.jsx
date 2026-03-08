import { act } from 'react';
import { render, screen } from '@testing-library/react';
import PlanProcedureItem from '../components/Plan/PlanProcedureItem/PlanProcedureItem';

describe('PlanProcedureItem component', () => {
  const procedure = { procedureId: '1', procedureTitle: 'Test Procedure' };
  const users = [{ label: 'Alice', value: 'u1' }];
  const planProcedures = { planProcedureUsers: [] };

  it('renders the procedure title', () => {
    const handleAdd = jest.fn();
    const handleRemove = jest.fn();
    act(() => {
      render(
        <PlanProcedureItem
          planId="123"
          procedure={procedure}
          users={users}
          planProcedures={planProcedures}
          handleAddUserToPlanProcedure={handleAdd}
          handleRemoveUserFromPlanProcedure={handleRemove}
          setPlanProcedures={jest.fn()}
        />
      );
    });

    expect(screen.getByText('Test Procedure')).toBeInTheDocument();
  });
});