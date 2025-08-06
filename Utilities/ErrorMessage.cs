namespace NHT_Marine_BE.Utilities
{
    public static class ErrorMessage
    {
        // Authentication related
        public const string USERNAME_EXISTED = "USERNAME_EXISTED";
        public const string EMAIL_EXISTED = "EMAIL_EXISTED";
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        public const string INCORRECT_USERNAME_OR_PASSWORD = "INCORRECT_USERNAME_OR_PASSWORD";
        public const string GOOGLE_AUTH_FAILED = "GOOGLE_AUTH_FAILED";
        public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string NO_PERMISSION = "NO_PERMISSION";

        // Users related
        public const string ROLE_NOT_FOUND = "ROLE_NOT_FOUND";
        public const string ROLE_EXISTED = "ROLE_EXISTED";
        public const string ROLE_BEING_USED = "ROLE_BEING_USED";

        // Services related
        public const string UPLOAD_IMAGE_FAILED = "UPLOAD_IMAGE_FAILED";
        public const string DELETE_IMAGE_FAILED = "DELETE_IMAGE_FAILED";

        // Application related
        public const string DATA_VALIDATION_FAILED = "DATA_VALIDATION_FAILED";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";

        // Categories related
        public const string CATEGORY_NOT_FOUND = "CATEGORY_NOT_FOUND";
        public const string CATEGORY_EXISTED = "CATEGORY_EXISTED";
        public const string CATEGORY_BEING_USED = "CATEGORY_BEING_USED";

        // Products related
        public const string PRODUCT_NOT_FOUND = "PRODUCT_NOT_FOUND";
        public const string PRODUCT_EXISTED = "PRODUCT_EXISTED";
        public const string PRODUCT_ITEM_NOT_FOUND = "PRODUCT_ITEM_NOT_FOUND";
        public const string PRODUCT_IDS_MISMATCH = "PRODUCT_IDS_MISMATCH";
        public const string PRODUCT_BEING_USED = "PRODUCT_BEING_USED";

        // Orders and carts related
        public const string QUANTITY_EXCEED_CURRENT_STOCK = "QUANTITY_EXCEED_CURRENT_STOCK";
        public const string CART_NOT_FOUND = "CART_NOT_FOUND";
        public const string CART_ITEM_NOT_FOUND = "CART_ITEM_NOT_FOUND";

        // Promotions related
        public const string PROMOTION_NOT_FOUND = "PROMOTION_NOT_FOUND";
        public const string PROMOTION_EXISTED = "PROMOTION_EXISTED";

        // Stock related
        public const string DAMAGE_TYPE_NOT_FOUND = "DAMAGE_TYPE_NOT_FOUND";
        public const string DAMAGE_TYPE_BEING_USED = "DAMAGE_TYPE_BEING_USED";
        public const string DAMAGE_TYPE_EXISTED = "DAMAGE_TYPE_EXISTED";
        public const string STORAGE_TYPE_NOT_FOUND = "STORAGE_TYPE_NOT_FOUND";
        public const string STORAGE_TYPE_BEING_USED = "STORAGE_TYPE_BEING_USED";
        public const string STORAGE_TYPE_EXISTED = "STORAGE_TYPE_EXISTED";
        public const string SUPPLIER_NOT_FOUND = "SUPPLIER_NOT_FOUND";
        public const string SUPPLIER_BEING_USED = "SUPPLIER_BEING_USED";
        public const string SUPPLIER_EXISTED = "SUPPLIER_EXISTED";
        public const string SUPPLIER_ADDRESS_EXISTED = "SUPPLIER_ADDRESS_EXISTED";
        public const string SUPPLIER_CONTACT_EMAIL_EXISTED = "SUPPLIER_CONTACT_EMAIL_EXISTED";
        public const string SUPPLIER_CONTACT_PHONE_EXISTED = "SUPPLIER_CONTACT_PHONE_EXISTED";

        // Transactions related
        public const string DELIVERY_SERVICE_NOT_FOUND = "DELIVERY_SERVICE_NOT_FOUND";
        public const string DELIVERY_SERVICE_EXISTED = "DELIVERY_SERVICE_EXISTED";
        public const string DELIVERY_SERVICE_BEING_USED = "DELIVERY_SERVICE_BEING_USED";
        public const string DELIVERY_CONTACT_PHONE_EXISTED = "DELIVERY_CONTACT_PHONE_EXISTED";
        public const string ORDER_STATUS_NOT_FOUND = "ORDER_STATUS_NOT_FOUND";
        public const string ORDER_STATUS_EXISTED = "ORDER_STATUS_EXISTED";
        public const string ORDER_STATUS_BEING_USED = "ORDER_STATUS_BEING_USED";
        public const string CANNOT_DELETE_DEFAULT_ORDER_STATUS = "CANNOT_DELETE_DEFAULT_ORDER_STATUS";
    }
}
