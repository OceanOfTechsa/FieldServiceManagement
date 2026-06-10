namespace FieldServiceManagement.Data.DataModels.Shared
{
    public class BusinessResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static BusinessResult Ok() => new() { Success = true };
        public static BusinessResult Fail(string message) => new() { Success = false, Message = message };
    }
}
