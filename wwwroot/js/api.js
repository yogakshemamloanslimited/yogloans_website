const API_BASE = "http://pfl.yogloans.com:8580/api/";
const mainurlweb = "https://test.yogloans.com/";
window.API = {
    base: API_BASE,
    allapi: {
        counts: `${API_BASE}count/get-count`,
        enquiry: `${API_BASE}enquiry`,
        verifyDetails: `${API_BASE}proc15/verify-details`,
        verifyotp: `${API_BASE}proc15/verify-otp`,
        resendOtp: `${API_BASE}proc15/resend-otp`,
        CareerQualification: `${API_BASE}career/qualification`,
        CareerApply: `${API_BASE}career/apply`,
        GetDepartment : `${API_BASE}Get-department`,
        getapplicationData : `${API_BASE}career/get-application-data/`,
        otheraddressdetails : `${API_BASE}career/other-address-details/`,
        branches : `${API_BASE}braches`,
        states : `${API_BASE}states`,
        selectedByState: `${API_BASE}selected?state_id=`
        // add other endpoints here
    }
};
