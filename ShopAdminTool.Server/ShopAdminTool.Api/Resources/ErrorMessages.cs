namespace ShopAdminTool.Api.Resources
{
    public static class ErrorMessages
    {
        public const string IdsAreNotEqualExceptionMessage = "Ids from Query and from Body don't match";
        public const string ApiKeyAbsent = "Api Key was not provided.";
        public const string UnauthorizedClient = "Unauthorized client.";
        public const string GeneralExceptionMessage = "An exception was thrown.";
    }
}