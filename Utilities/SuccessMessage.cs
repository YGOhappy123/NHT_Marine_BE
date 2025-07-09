using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Utilities
{
    public static class SuccessMessage
    {
        // Authentication related
        public const string SIGN_IN_SUCCESSFULLY = "SIGN_IN_SUCCESSFULLY";
        public const string SIGN_UP_SUCCESSFULLY = "SIGN_UP_SUCCESSFULLY";
        public const string REFRESH_TOKEN_SUCCESSFULLY = "REFRESH_TOKEN_SUCCESSFULLY";
        public const string DEACTIVATE_ACCOUNT_SUCCESSFULLY = "DEACTIVATE_ACCOUNT_SUCCESSFULLY";
        public const string RESET_PASSWORD_EMAIL_SENT = "RESET_PASSWORD_EMAIL_SENT";
        public const string RESET_PASSWORD_SUCCESSFULLY = "RESET_PASSWORD_SUCCESSFULLY";
        public const string GOOGLE_AUTH_SUCCESSFULLY = "GOOGLE_AUTH_SUCCESSFULLY";

        // Users related
        public const string CHANGE_PASSWORD_SUCCESSFULLY = "CHANGE_PASSWORD_SUCCESSFULLY";
        public const string UPDATE_USER_SUCCESSFULLY = "UPDATE_USER_SUCCESSFULLY";
        public const string CREATE_STAFF_SUCCESSFULLY = "CREATE_STAFF_SUCCESSFULLY";

        // Services related
        public const string UPLOAD_IMAGE_SUCCESSFULLY = "UPLOAD_IMAGE_SUCCESSFULLY";
        public const string DELETE_IMAGE_SUCCESSFULLY = "DELETE_IMAGE_SUCCESSFULLY";
    }
}
