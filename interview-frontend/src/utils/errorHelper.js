export const getErrorMessage = (error) => {
    if (!error) return "Something went wrong";
    if (error instanceof Error) return error.message;
    if(typeof error === "string") return error;
    if(error.message) return error.message;
    return "Unexpected error occurred";
};