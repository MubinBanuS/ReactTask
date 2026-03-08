import { act } from 'react';
import { render, screen } from '@testing-library/react';
import { useRouteError } from 'react-router-dom';
import ErrorPage from '../components/Error/Error';


// mock useRouteError and Navbar
jest.mock('react-router-dom', () => ({
  ...jest.requireActual('react-router-dom'),
  useRouteError: jest.fn(),
  Link: ({ to, children }) => <a href={to}>{children}</a>,
}));

jest.mock('../components/Layout/Navbar/Navbar', () => () => <div>NavbarMock</div>);


describe('ErrorPage component', () => {
  it('renders error information provided by useRouteError', () => {
    (useRouteError).mockReturnValue({ statusText: 'Not Found', message: 'nope' });
    
    act(() => {
      render(<ErrorPage />);
    });

    expect(screen.getByText(/Oops!/i)).toBeInTheDocument();
    expect(screen.getByText(/Not Found/i)).toBeInTheDocument();
  });
});
