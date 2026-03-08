import { useNavigate } from "react-router-dom";
import { startPlan } from "./api/api";
import Layout from "./components/Layout/Layout";
import { useState } from "react";
import { toast } from "react-toastify";

const App = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const start = async () => {
        if (loading) return; // prevent double API call

        try {
            setLoading(true);
            const plan = await startPlan();
            navigate(`/plan/${plan.planId}`);
        } catch (error) {
            console.error("Error starting plan:", error);
            toast.error("Failed to start the plan. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Layout>
            <div className="container">
                <div className="text-center mt-4">
                    <h3>Start Here</h3>
                    <p>Click "start" to begin</p>

                    <button
                        className="btn btn-primary d-inline-flex align-items-center justify-content-center"
                        onClick={start}
                        disabled={loading}
                        style={{ minWidth: "160px" }}
                    >
                        {loading ? (
                            <>
                                <span className="spinner-border spinner-border-sm me-2"></span>
                                Starting...
                            </>
                        ) : (
                            "Start"
                        )}
                    </button>
                </div>
            </div>
        </Layout>
    );
};

export default App;