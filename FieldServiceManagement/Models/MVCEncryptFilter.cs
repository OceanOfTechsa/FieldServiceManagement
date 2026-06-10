using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;

namespace FieldServiceManagement.Models
{
    public class MVCDecryptFilterAttribute : Attribute, IAsyncResourceFilter
    {
        public string EncDecFullClassName;

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            IEncryptDecrypt encDec;

            if (string.IsNullOrWhiteSpace(EncDecFullClassName))
            {
                encDec = new EncryptDecrypt(AppSettings.GetUrLEncryptionKey());
            }
            else
            {
                var encryptionType = Type.GetType(EncDecFullClassName);

                if (encryptionType == null)
                    throw new ArgumentException("Cannot determine type of encryption class");

                encDec = Activator.CreateInstance(encryptionType) as IEncryptDecrypt;

                if (encDec == null)
                    throw new ArgumentException("Cannot convert " + EncDecFullClassName + " to IEncryptDecrypt");
            }

            var args = HttpUtility.ParseQueryString(context.HttpContext.Request.QueryString.ToString());
            var parametersAction = context.ActionDescriptor.Parameters;

            for (var i = 0; i < args.Count; i++)
            {
                var encryptedValue = args[i]?.Replace(' ', '+');
                var keyName = args.GetKey(i);

                var decryptedValue = encDec.DecryptString(encryptedValue);

                if (decryptedValue == null)
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/InvalidLink.cshtml"
                    };
                    return;
                }

                var param = parametersAction.FirstOrDefault(p => p.Name == keyName);

                if (param == null)
                    continue;

                var converted = ChangeType(decryptedValue, param.ParameterType);

                if (converted == null)
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/InvalidLink.cshtml"
                    };
                    return;
                }

                context.RouteData.Values[keyName ?? string.Empty] = converted;
            }

            await next();
        }

        private static object ChangeType(object value, Type conversion)
        {
            if (value == null)
                return null;

            var parType = conversion;

            if (parType.IsGenericType && parType.GetGenericTypeDefinition() == typeof(Nullable<>))
                parType = Nullable.GetUnderlyingType(parType);

            if (parType == typeof(Guid))
            {
                if (!Guid.TryParse(value.ToString(), out var guid))
                    return null;

                return guid;
            }

            try
            {
                return Convert.ChangeType(value, parType);
            }
            catch
            {
                return null;
            }
        }
    }
}