const API_BASE = "http://localhost:8085/api/";
const mainurlweb = "http://localhost:5132/";

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
        GetDepartment: `${API_BASE}Get-department`,
        getapplicationData2: `${API_BASE}career/get-application-data/`,
        otheraddressdetails: `${API_BASE}career/other-address-details/`,
        branches: `${API_BASE}braches`,   // ❌ likely typo: should it be "branches"?
        states: `${API_BASE}states`,
        selectedByState: `${API_BASE}selected?state_id=`,
        Onlinepay: `${API_BASE}payonline/get-customer/`,
        Payonline: `${API_BASE}payonline/get-loan/`,
        Careerpay: `${API_BASE}career/payment/`,
        Get15g: `${API_BASE}15g/get-pdf-data`,
        Getdata15g: `${API_BASE}15g/get-data`,
        Get15g2: `${API_BASE}15g/get15g/`,
        GetAppartment: `${API_BASE}api/Get-department`,  // ❌ double "api/"
        EnachGet: `${API_BASE}enach/get-loans/`,
        EnachStop: `${API_BASE}enach/stop`,
        EnachCancle: `${API_BASE}enach/cancel`, // ❌ spelling: probably "Cancel"
        gfind: `${API_BASE}15g/find`,
        getapplicationdata: `${API_BASE}15g/get-application-data/`,
        gsubmitapplication: `${API_BASE}15g/submit-application`,
        ggetpdfdata: `${API_BASE}15g/get-pdf-data/`,
        EnqueryOccupations: `${API_BASE}Enquery/Occupations`, 
        webonline:`${API_BASE}api/proc15/online`,
        onlineCustomer: `${API_BASE}proc15/online?customerId=`,  // ❌ likely typo: should it be "onlineCustomer"
        sbifailure :`${API_BASE}sbi-failure`,
        paynow1: `${API_BASE}payonline/Paynow/step1`,
        QrData: `${API_BASE}Qrcode/customer/`
        // add other endpoints here
    }
};
