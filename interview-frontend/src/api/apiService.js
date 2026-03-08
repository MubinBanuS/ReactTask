import { getErrorMessage } from "../utils/errorHelper";
const defaultHeaders = {
    Accept: "application/json",
    "Content-Type": "application/json",
};
export const apiRequest = async ( url, options={})=>{
try{
    
        const config = {
            method: options.method || "GET",
            headers: {
                ...defaultHeaders,
                ...options.headers,
            },
            ...options
        };
        // Automatically stringify body if it's an object and not already a string or FormData
        if(config.body && typeof config.body !== "string" && !(config.body instanceof FormData))
        {
            config.body = JSON.stringify(config.body);
        }

        const response = await fetch(url, config);
        // handles Api errors
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`API request failed: ${response.status} ${response.statusText} - ${errorText}`);
        }
        // no-content responses
        if (response.status === 204 || response.status === 205) return null;
        const contentType = response.headers.get("content-type") || "";
        if (contentType.includes("application/json")) {
            return response.json();
        }
        return response.text();
    }
    catch(error){
        console.error("API request error:", error);
        // normalize error
        throw new Error(getErrorMessage(error));
    }
};
