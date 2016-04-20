namespace AOPIOC.Wcf
{
    using EgoalTech.Commons.Logger;
    using EgoalTech.DB;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    public class EGOperationBehavior : IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            IOperationInvoker baseInvoker = dispatchOperation.Invoker;
            dispatchOperation.Invoker = new EGOperationInvoker(baseInvoker, operationDescription);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        private class EGOperationInvoker : IOperationInvoker
        {
            private IOperationInvoker invoker;
            private OperationDescription operationDescription;

            public EGOperationInvoker(IOperationInvoker baseInvoker, OperationDescription operationDescription)
            {
                this.invoker = baseInvoker;
                this.operationDescription = operationDescription;
            }

            public object[] AllocateInputs()
            {
                return this.invoker.AllocateInputs();
            }

            private List<OperationAttribute> GetOperationAttributes()
            {
                List<OperationAttribute> list = new List<OperationAttribute>();
                object[] customAttributes = this.operationDescription.SyncMethod.GetCustomAttributes(typeof(OperationAttribute), true);
                if (customAttributes != null)
                {
                    foreach (object obj2 in customAttributes)
                    {
                        OperationAttribute item = obj2 as OperationAttribute;
                        item.OperationDescription = this.operationDescription;
                        list.Add(item);
                    }
                }
                return (from t in list
                    orderby t.Priority
                    select t).ToList<OperationAttribute>();
            }

            public object Invoke(object instance, object[] inputs, out object[] outputs)
            {
                object result = null;
                List<OperationAttribute> operationAttributes = null;
                int num;
                object obj3;
                try
                {
                    OperationAttribute current;
                    operationAttributes = this.GetOperationAttributes();
                    using (List<OperationAttribute>.Enumerator enumerator = operationAttributes.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            current.BeforeInvoke(instance, inputs);
                        }
                    }
                    result = this.invoker.Invoke(instance, inputs, out outputs);
                    if (operationAttributes.Count > 0)
                    {
                        for (num = operationAttributes.Count - 1; num >= 0; num--)
                        {
                            current = operationAttributes[num];
                            current.AfterInvoke(instance, inputs, result);
                        }
                    }
                    obj3 = result;
                }
                catch (Exception exception)
                {
                    this.TryLog(exception);
                    try
                    {
                        if ((operationAttributes != null) && (operationAttributes.Count > 0))
                        {
                            for (num = operationAttributes.Count - 1; num >= 0; num--)
                            {
                                operationAttributes[num].OnInvokeError(instance, inputs, result);
                            }
                        }
                    }
                    catch
                    {
                    }
                    IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
                    IExceptionFormatter formatter = null;
                    if (objectContainer.IsRegistered(typeof(IExceptionFormatter)))
                    {
                        formatter = objectContainer.Resolve<IExceptionFormatter>();
                    }
                    if (formatter == null)
                    {
                        formatter = new ExceptionFormatter();
                        objectContainer.RegisterInstance<IExceptionFormatter>(formatter);
                    }
                    throw formatter.Format(exception);
                }
                finally
                {
                    try
                    {
                        WcfContext.Current.FindAll<DbObjectOperator>().ForEach(delegate (DbObjectOperator op) {
                            try
                            {
                                op.DbOperator.Close();
                            }
                            catch
                            {
                            }
                        });
                    }
                    catch
                    {
                    }
                }
                return obj3;
            }

            public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
            {
                return this.invoker.InvokeBegin(instance, inputs, callback, state);
            }

            public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
            {
                return this.invoker.InvokeEnd(instance, out outputs, result);
            }

            private void TryLog(Exception ex)
            {
                try
                {
                    IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
                    if (!objectContainer.IsRegistered(typeof(ILogger)))
                    {
                        objectContainer.RegisterType<ILogger, WcfLogger>(new object[0]);
                    }
                    objectContainer.Resolve<ILogger>().Error(ex, null);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("WCF log exception error:");
                    Debug.WriteLine(exception.Message);
                }
            }

            public bool IsSynchronous
            {
                get
                {
                    return ((this.invoker == null) || this.invoker.IsSynchronous);
                }
            }
        }
    }
}

