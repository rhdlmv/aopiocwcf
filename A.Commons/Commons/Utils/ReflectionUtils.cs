namespace EgoalTech.Commons.Utils
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;

    public class ReflectionUtils
    {
        private static void FindMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes, out MethodInfo methodInfo, out ParameterInfo[] parameters)
        {
            methodInfo = null;
            parameters = null;
            if (null == parameterTypes)
            {
                methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                methodInfo = methodInfo.MakeGenericMethod(typeArguments);
                parameters = methodInfo.GetParameters();
            }
            else
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo info in methods)
                {
                    if (!(info.Name == methodName))
                    {
                        continue;
                    }
                    MethodInfo info2 = info.MakeGenericMethod(typeArguments);
                    parameters = info2.GetParameters();
                    if (parameters.Length == parameterTypes.Length)
                    {
                        bool flag = true;
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i].ParameterType != parameterTypes[i])
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            methodInfo = info2;
                            break;
                        }
                    }
                }
                if (null == methodInfo)
                {
                    throw new InvalidOperationException("Method not found");
                }
            }
        }

        public static GenericInvoker GenericMethodInvokerMethod(Type type, string methodName, Type[] typeArguments)
        {
            return GenericMethodInvokerMethod(type, methodName, typeArguments, null);
        }

        public static GenericInvoker GenericMethodInvokerMethod(Type type, string methodName, Type[] typeArguments, Type[] parameterTypes)
        {
            MethodInfo info;
            ParameterInfo[] infoArray;
            FindMethod(type, methodName, typeArguments, parameterTypes, out info, out infoArray);
            DynamicMethod method = new DynamicMethod(string.Format("__MethodInvoker_{0}_ON_{1}", info.Name, info.DeclaringType.Name), typeof(object), new Type[] { typeof(object), typeof(object[]) }, info.DeclaringType);
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.DeclareLocal(typeof(object));
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, info.DeclaringType);
            for (int i = 0; i < infoArray.Length; i++)
            {
                iLGenerator.Emit(OpCodes.Ldarg_1);
                iLGenerator.Emit(OpCodes.Ldc_I4, i);
                iLGenerator.Emit(OpCodes.Ldelem_Ref);
                Type parameterType = infoArray[i].ParameterType;
                if (parameterType.IsClass)
                {
                    iLGenerator.Emit(OpCodes.Castclass, parameterType);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Unbox_Any, parameterType);
                }
            }
            iLGenerator.EmitCall(OpCodes.Callvirt, info, null);
            if (info.ReturnType == typeof(void))
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else if (info.ReturnType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Box, info.ReturnType);
            }
            iLGenerator.Emit(OpCodes.Stloc_0);
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ret);
            return (GenericInvoker) method.CreateDelegate(typeof(GenericInvoker));
        }

        public static PropertyInfo[] GetPropertyInfoWithAttribute(Type objectType, Type attributeType)
        {
            return (from prop in objectType.GetProperties()
                where Attribute.IsDefined(prop, attributeType)
                select prop).ToArray<PropertyInfo>();
        }
    }
}

