
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Entities.Base;
using Microsoft.IdentityModel.Tokens;
using Common.Resources;

namespace EstateAgentApi.Services.Base
{
    public class ServiceBase<Tclass>
    {
        protected readonly ILogger<Tclass> _logger;
         public ServiceBase(
            ILogger<Tclass> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        protected virtual ServiceResult HandleException(Exception ex)
        {
            try
            {
                if (ex is DbUpdateException)
                {
                    return HandleDbUpdateException(ex);
                }
                else if (ex is APIException napEx)
                {
                    return HandleAPIException(napEx);
                }
                else
                {
                    return InternalServerError(ErrorCodeEnum.None,Resource.GeneralErrorTryAgain, null);
                }
            }
            catch (Exception generalEx)
            {
                _logger.LogError(generalEx, null);

                throw; //Todo : درسته ایا یا باید همینجا هندل بشه
            }
        }

        protected virtual ServiceResult Ok(object data)
        {
            if (data == null)
            {
                return Ok();
            }

            return new ServiceResult(data, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, null, null));
        }

        protected virtual ServiceResult Ok()
        {
            return new ServiceResult(null, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, null, null));
        }

        protected virtual ServiceResult BadRequest(ErrorCodeEnum errorCode, string errorMessage, List<FieldErrorItem> errors)
        {
            return new ServiceResult(null, new ApiResult(HttpStatusCode.BadRequest, errorCode, errorMessage, errors));
        }

        protected virtual ServiceResult NotFound(ErrorCodeEnum errorCode, string errorMessage, List<FieldErrorItem> errors)
        {
            return new ServiceResult(null, new ApiResult(HttpStatusCode.NotFound, errorCode, errorMessage, errors));
        }

        protected virtual ServiceResult InternalServerError(ErrorCodeEnum errorCode, string errorMessage, List<FieldErrorItem> errors)
        {
            return new ServiceResult(null, new ApiResult(HttpStatusCode.InternalServerError, errorCode, errorMessage, errors));
        }
        protected virtual void ValidateModel(object model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
                throw new APIException(HttpStatusCode.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, validationResults.AsReadOnly());
        }

        private ServiceResult HandleDbUpdateException(Exception ex)
        {
            var sqlEx = ex.InnerException as SqlException;
            if (sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627))  /// unique constraint violation
            {
                //TODO: ErrorCode اضافه و مشخص شود
                return BadRequest(ErrorCodeEnum.None, "به دلیل نقض یونیک بودن بعضی از فیلدها امکان اضافه شدن و یا اپدیت وجود ندارد ", null);
            }
            else if (sqlEx != null && (sqlEx.Number == 547))  /// Foreign key constraint violation
            {
                return BadRequest(ErrorCodeEnum.None, "به دلیل نقض محدودیت کلید خارجی امکان درج یا آپدیت وجود ندارد", null);
            }
            else
            {
                return InternalServerError(ErrorCodeEnum.None, "", null);
            }
        }

        private ServiceResult HandleAPIException(APIException napEx)
        {
            if (napEx is null)
                throw new ArgumentNullException(nameof(napEx));

            if (napEx.ValidationResults.IsNullOrEmpty())
                return BadRequest(ErrorCodeEnum.None, "خطای اعتبارسنجی فیلدها", null);

            var validationResults = new List<FieldErrorItem>();
            napEx.ValidationResults
                .ToList()
                .ForEach(v =>
                    validationResults
                    .Add(new FieldErrorItem(v.MemberNames.FirstOrDefault() ?? string.Empty, new List<string>() { v.ErrorMessage ?? string.Empty })));

            return BadRequest(ErrorCodeEnum.None, "خطای اعتبارسنجی فیلدها", validationResults);
        }

        //protected virtual ApiResultService HandleException(Exception ex)
        //{
        //    try
        //    {
        //        if (ex is DbUpdateException)
        //        {
        //            return HandleDbUpdateException(ex);
        //        }
        //        else if (ex is NAPException napEx)
        //        {
        //            return HandleNAPException(napEx);
        //        }
        //        else
        //        {
        //            return InternalServerError(ErrorCodeEnum.None, ValidationResource.GeneralErrorTryAgain, null);
        //        }
        //    }
        //    catch (Exception generalEx)
        //    {
        //        _logger.LogError(generalEx, null);

        //        throw; //Todo : درسته ایا یا باید همینجا هندل بشه
        //    }
        //}

        //private ApiResultService HandleDbUpdateException(Exception ex)
        //{
        //    var sqlEx = ex.InnerException as SqlException;
        //    if (sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627))  /// unique constraint violation
        //    {
        //        //TODO: ErrorCode اضافه و مشخص شود
        //        return BadRequest(ErrorCodeEnum.None, "به دلیل نقض یونیک بودن بعضی از فیلدها امکان اضافه شدن و یا اپدیت وجود ندارد ", null);
        //    }
        //    else if (sqlEx != null && (sqlEx.Number == 547))  /// Foreign key constraint violation
        //    {
        //        return BadRequest(ErrorCodeEnum.None, "به دلیل نقض محدودیت کلید خارجی امکان درج یا آپدیت وجود ندارد", null);
        //    }
        //    else
        //    {
        //        return InternalServerError(ErrorCodeEnum.None, "", null);
        //    }
        //}

        //private ApiResultService HandleNAPException(NAPException napEx)
        //{
        //    if (napEx is null)
        //        throw new ArgumentNullException(nameof(napEx));

        //    if (napEx.ValidationResults.IsNullOrEmpty())
        //        return BadRequest(ErrorCodeEnum.None, "خطای اعتبارسنجی فیلدها", null);

        //    var validationResults = new List<FieldErrorItem>();
        //    napEx.ValidationResults
        //        .ToList()
        //        .ForEach(v =>
        //            validationResults
        //            .Add(new FieldErrorItem(v.MemberNames.FirstOrDefault() ?? string.Empty, new List<string>() { v.ErrorMessage ?? string.Empty })));

        //    return BadRequest(ErrorCodeEnum.None, "خطای اعتبارسنجی فیلدها", validationResults);
        //}
    }
}
