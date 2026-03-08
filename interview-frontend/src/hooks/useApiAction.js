import { useState, useMemo, useCallback } from "react";
import { toast } from "react-toastify";

export const useApiAction = ({ apiCall, successMessage, errorMessage, onSuccess, throwOnError = false }) => {
  const [loading, setLoading] = useState(false);

  const execute = useCallback(async (...args) => {
    if (loading) return;
    setLoading(true);
    try {
      const result = await apiCall(...args);
      if (successMessage) toast.success(successMessage);
      if (onSuccess) await onSuccess(result, ...args);
      return result;
    } catch (error) {
      console.error("API action error:", error);
      if (errorMessage) toast.error(errorMessage);
      if (throwOnError) throw error;
    } finally {
      setLoading(false);
    }
  }, [apiCall, loading, successMessage, errorMessage, onSuccess, throwOnError]);

  // Memoize the return object to prevent unnecessary re-renders of child components
  return useMemo(() => ({ execute, loading }), [execute, loading]);
};