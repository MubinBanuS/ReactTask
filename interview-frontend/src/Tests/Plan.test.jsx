import { act } from 'react';
import { render, screen, waitFor, fireEvent } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import * as api from '../api/api';
import Plan from '../components/Plan/Plan';

jest.mock('../api/api');

describe('Plan component', () => {
  const procedures = [
    { procedureId: 'p1', procedureTitle: 'Proc 1' },
    { procedureId: 'p2', procedureTitle: 'Proc 2' },
  ];
  const planProcedures = [
    { procedureId: 'p1', procedure: { procedureId: 'p1', procedureTitle: 'Proc 1' } },
  ];
  const users = [{ name: 'Alice', userId: 'u1' }];

  beforeEach(() => {
    api.getProcedures.mockResolvedValue(procedures);
    api.getPlanProcedures.mockResolvedValue(planProcedures);
    api.getUsers.mockResolvedValue(users);
    api.addProcedureToPlan.mockResolvedValue();
    api.addUserToPlanProcedure.mockResolvedValue();
    api.removeUserFromPlanProcedure.mockResolvedValue();
  });

  it('fetches and displays procedures and plan procedures', async () => {
    act(() => {
      render(
        <MemoryRouter initialEntries={["/plan/123"]} future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
          <Routes>
            <Route path="/plan/:id" element={<Plan />} />
          </Routes>
        </MemoryRouter>
      );
    });

    // wait for procedures to show up
    await waitFor(() => {
      // use findByText to avoid duplicates warning
      expect(screen.getByText('Proc 2')).toBeInTheDocument();
    });
    expect(screen.getAllByText('Proc 1').length).toBeGreaterThan(0);

    // plan-p1 should also appear under "Added to Plan" section
    expect(screen.getAllByText('Proc 1').length).toBeGreaterThanOrEqual(1);
  });

  it('calls addProcedureToPlan when checkbox clicked', async () => {
    act(() => {
      render(
        <MemoryRouter initialEntries={["/plan/123"]} future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
          <Routes>
            <Route path="/plan/:id" element={<Plan />} />
          </Routes>
        </MemoryRouter>
      );
    });

    await waitFor(() => screen.getByText('Proc 2'));
    const checkbox = screen.getAllByRole('checkbox').find(cb => cb.checked === false);
    
    fireEvent.click(checkbox);
    
    // Wait for the async addProcedureToPlan call to complete
    await waitFor(() => {
      expect(api.addProcedureToPlan).toHaveBeenCalledWith('123', 'p2');
    });
  });
});
