using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Utilities
{
    public static class ErrorMessage
    {
        // Authentication related
        public const string USERNAME_EXISTED = "USERNAME_EXISTED";
        public const string EMAIL_EXISTED = "EMAIL_EXISTED";
        public const string PHONE_NUMBER_EXISTED = "PHONE_NUMBER_EXISTED";
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        public const string INCORRECT_USERNAME_OR_PASSWORD = "INCORRECT_USERNAME_OR_PASSWORD";
        public const string GOOGLE_AUTH_FAILED = "GOOGLE_AUTH_FAILED";
        public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string NO_PERMISSION = "NO_PERMISSION";

        // Users related

        // Services related
        public const string UPLOAD_IMAGE_FAILED = "UPLOAD_IMAGE_FAILED";
        public const string DELETE_IMAGE_FAILED = "DELETE_IMAGE_FAILED";

        // Application related
        public const string DATA_VALIDATION_FAILED = "DATA_VALIDATION_FAILED";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
    }
}
