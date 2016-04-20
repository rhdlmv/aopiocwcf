namespace EgoalTech.Commons
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class AsyncHandler<T>
    {
        private Exception error;
        private ManualResetEvent evt;
        private LambdaExpression expression;
        private MethodCallExpression methodCallExpression;
        private T obj;
        private object result;
        private Thread thread;

        public AsyncHandler(T obj)
        {
            this.evt = new ManualResetEvent(false);
            this.obj = obj;
        }

        private MethodCallExpression GetMethodCallExpression(LambdaExpression expression)
        {
            MethodCallExpression body = expression.Body as MethodCallExpression;
            if ((body == null) && expression.Body.NodeType.Equals(ExpressionType.Convert))
            {
                UnaryExpression expression3 = expression.Body as UnaryExpression;
                body = expression3.Operand as MethodCallExpression;
            }
            return body;
        }

        public void Invoke(Expression<Action<T>> expression)
        {
            this.result = null;
            lock (((AsyncHandler<T>) this))
            {
                MethodCallExpression methodCallExpression = this.GetMethodCallExpression(expression);
                if (methodCallExpression == null)
                {
                    throw new Exception("Cannot pass in a non method expression");
                }
                this.expression = expression;
                this.methodCallExpression = methodCallExpression;
                this.thread = new Thread(new ThreadStart(this.Start));
                this.thread.Start();
            }
        }

        public void Invoke(Expression<Func<T, object>> expression)
        {
            this.result = null;
            lock (((AsyncHandler<T>) this))
            {
                MethodCallExpression methodCallExpression = this.GetMethodCallExpression(expression);
                if (methodCallExpression == null)
                {
                    throw new Exception("Cannot pass in a non method expression");
                }
                this.expression = expression;
                this.methodCallExpression = methodCallExpression;
                this.thread = new Thread(new ThreadStart(this.Start));
                this.thread.Start();
            }
        }

        private void Start()
        {
            try
            {
                MethodInfo method = this.methodCallExpression.Method;
                List<object> list = new List<object>();
                foreach (Expression expression in this.methodCallExpression.Arguments)
                {
                    object item = Expression.Lambda(expression, this.expression.Parameters).Compile().DynamicInvoke(new object[1]);
                    list.Add(item);
                }
                this.result = method.Invoke(this.obj, list.ToArray());
            }
            catch (Exception exception)
            {
                this.error = exception;
            }
            this.evt.Set();
        }

        public object WaitForComplete(int timeout = 0x15f90)
        {
            lock (((AsyncHandler<T>) this))
            {
                bool flag = this.evt.WaitOne(timeout);
                this.evt.Reset();
                if (!flag)
                {
                    try
                    {
                        if (this.thread != null)
                        {
                            this.thread.Abort();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    this.error = new Exception("Invoke Timeout !", this.error);
                }
                this.thread = null;
                this.expression = null;
                this.methodCallExpression = null;
                if (this.error != null)
                {
                    Exception error = this.error;
                    this.error = null;
                    throw new Exception(error.Message, error);
                }
            }
            return this.result;
        }

        public TResult WaitForComplete<TResult>(int timeout = 0x15f90)
        {
            object obj2 = this.WaitForComplete(timeout);
            if (obj2 != null)
            {
                return (TResult) obj2;
            }
            return default(TResult);
        }
    }
}

