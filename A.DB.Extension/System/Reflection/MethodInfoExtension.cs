namespace System.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;

    public static class MethodInfoExtension
    {
        private static Dictionary<int, FastInvokeHandler> executeDelegatePool = new Dictionary<int, FastInvokeHandler>();

        private static FastInvokeHandler CreateMethodInvoker(this MethodInfo methodInfo)
        {
            int num;
            DynamicMethod method = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            ILGenerator iLGenerator = method.GetILGenerator();
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] typeArray = new Type[parameters.Length];
            for (num = 0; num < typeArray.Length; num++)
            {
                if (parameters[num].ParameterType.IsByRef)
                {
                    typeArray[num] = parameters[num].ParameterType.GetElementType();
                }
                else
                {
                    typeArray[num] = parameters[num].ParameterType;
                }
            }
            LocalBuilder[] builderArray = new LocalBuilder[typeArray.Length];
            for (num = 0; num < typeArray.Length; num++)
            {
                builderArray[num] = iLGenerator.DeclareLocal(typeArray[num], true);
            }
            for (num = 0; num < typeArray.Length; num++)
            {
                iLGenerator.Emit(OpCodes.Ldarg_1);
                EmitFastInt(iLGenerator, num);
                iLGenerator.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(iLGenerator, typeArray[num]);
                iLGenerator.Emit(OpCodes.Stloc, builderArray[num]);
            }
            if (!methodInfo.IsStatic)
            {
                iLGenerator.Emit(OpCodes.Ldarg_0);
            }
            for (num = 0; num < typeArray.Length; num++)
            {
                if (parameters[num].ParameterType.IsByRef)
                {
                    iLGenerator.Emit(OpCodes.Ldloca_S, builderArray[num]);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Ldloc, builderArray[num]);
                }
            }
            if (methodInfo.IsStatic)
            {
                iLGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            }
            else
            {
                iLGenerator.EmitCall(OpCodes.Callvirt, methodInfo, null);
            }
            if (methodInfo.ReturnType == typeof(void))
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                EmitBoxIfNeeded(iLGenerator, methodInfo.ReturnType);
            }
            for (num = 0; num < typeArray.Length; num++)
            {
                if (parameters[num].ParameterType.IsByRef)
                {
                    iLGenerator.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(iLGenerator, num);
                    iLGenerator.Emit(OpCodes.Ldloc, builderArray[num]);
                    if (builderArray[num].LocalType.IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Box, builderArray[num].LocalType);
                    }
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }
            iLGenerator.Emit(OpCodes.Ret);
            return (FastInvokeHandler) method.CreateDelegate(typeof(FastInvokeHandler));
        }

        private static void EmitBoxIfNeeded(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    break;

                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;

                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    break;

                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    break;

                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    break;

                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    break;

                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    break;

                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    break;

                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    break;

                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    break;

                default:
                    if ((value > -129) && (value < 0x80))
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }

        public static object FastInvoke(this MethodInfo methodInfo, object invoker, params object[] parameters) => 
            GetExecuteDelegate(methodInfo)(invoker, (parameters == null) ? new object[1] : parameters)

        private static FastInvokeHandler GetExecuteDelegate(MethodInfo methodInfo)
        {
            FastInvokeHandler handler = null;
            int hashCode = methodInfo.GetHashCode();
            if (!executeDelegatePool.TryGetValue(hashCode, out handler))
            {
                lock (executeDelegatePool)
                {
                    if (!executeDelegatePool.TryGetValue(hashCode, out handler))
                    {
                        handler = methodInfo.CreateMethodInvoker();
                        executeDelegatePool.Add(hashCode, handler);
                    }
                }
            }
            return handler;
        }
    }
}

