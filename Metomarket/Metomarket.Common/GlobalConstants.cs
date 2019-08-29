namespace Metomarket.Common
{
    public static class GlobalConstants
    {
        public const string AdministratorRoleName = "Admin";

        public const string RootAdministratorUsername = "root";

        public const string RootAdministratorEmail = "admin@metomarket.bg";

        public const string RootAdministratorAddress = "Earth";

        public const string StringLengthErrorMessageFormat = "The {0} must be at least {2} and at max {1} characters long.";

        public const int UsernameMinLength = 3;

        public const int UsernameMaxLength = 25;

        public const int PasswordMinLength = 6;

        public const int PasswordMaxLength = 100;

        public const int UserFirstNameMaxLength = 30;

        public const int UserLastNameMaxLength = 25;

        public const int UserAddressMinLength = 10;

        public const int UserAddressMaxLength = 100;

        public const int ProductNameMinLength = 2;

        public const int ProductNameMaxLength = 20;

        public const int ProductImageUrlMaxLength = 2500;

        public const int ProductTypeNameMinLength = 2;

        public const int ProductTypeNameMaxLength = 20;

        public const int CreditCompanyNameMaxLength = 25;

        public const int CacheExpirationMinutes = 2;
    }
}