using Metomarket.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Metomarket.Web.Infrastructure.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private const string CustomErrorViewPath = "/Views/Shared/CustomError.cshtml";
        private const string ViewDataErrorMessageKey = "ErrorMessage";

        private readonly IModelMetadataProvider modelMetadataProvider;

        public ExceptionFilter(
        IModelMetadataProvider modelMetadataProvider)
        {
            this.modelMetadataProvider = modelMetadataProvider;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception.GetType() != typeof(ServiceException))
            {
                return;
            }

            context.Result = new ViewResult
            {
                ViewName = CustomErrorViewPath,
                ViewData = new ViewDataDictionary(
                    this.modelMetadataProvider,
                    context.ModelState)
                {
                    [ViewDataErrorMessageKey] = context.Exception.Message,
                },
            };
        }
    }
}