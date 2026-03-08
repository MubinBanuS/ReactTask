import { act } from 'react';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Navbar from '../components/Layout/Navbar/Navbar';

describe('Navbar component', () => {
  it('renders logo link and image', () => {
    act(() => {
      render(
        <MemoryRouter future={{ v7_startTransition: true, v7_relativeSplatPath: true }}>
          <Navbar />
        </MemoryRouter>
      );
    });

    const link = screen.getByRole('link');
    expect(link).toHaveAttribute('href', '/');

    const img = screen.getByAltText(/logo/i);
    expect(img).toBeInTheDocument();
  });
});
