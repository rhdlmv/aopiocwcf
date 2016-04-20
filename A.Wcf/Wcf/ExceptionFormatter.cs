namespace AOPIOC.Wcf
{
    using EgoalTech.Commons.Exception;
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    public class ExceptionFormatter : IExceptionFormatter
    {
        public Exception Format(Exception exception)
        {
            Exception exception2 = exception;
            if (exception is WebFaultException<ExceptionDetail>)
            {
                return exception2;
            }
            if (!(exception is GenericException))
            {
                exception2 = new InternalSystemException(exception);
            }
            return new WebFaultException<ExceptionDetail>(new ExceptionDetail(exception2), HttpStatusCode.BadRequest);
        }
    }
}

