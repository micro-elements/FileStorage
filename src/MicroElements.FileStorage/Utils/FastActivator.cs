﻿// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace MicroElements.FileStorage.Utils
{
    /// <summary>
    /// Source: https://blogs.msdn.microsoft.com/seteplia/2017/02/01/dissecting-the-new-constraint-in-c-a-perfect-example-of-a-leaky-abstraction/
    /// </summary>
    public static class FastActivator
    {
        public static T CreateInstance<T>() where T : new()
        {
            return FastActivatorImpl<T>.Create();
        }

        private static class FastActivatorImpl<T> where T : new()
        {
            public static readonly Func<T> Create = DynamicModuleLambdaCompiler.GenerateFactory<T>();
        }
    }

    public static class DynamicModuleLambdaCompiler
    {
        public static Func<T> GenerateFactory<T>() where T : new()
        {
            Expression<Func<T>> expr = () => new T();
            NewExpression newExpr = (NewExpression)expr.Body;

            var method = new DynamicMethod(
                name: "lambda",
                returnType: newExpr.Type,
                parameterTypes: new Type[0],
                m: typeof(DynamicModuleLambdaCompiler).Module,
                skipVisibility: true);

            ILGenerator ilGen = method.GetILGenerator();
            // Constructor for value types could be null
            if (newExpr.Constructor != null)
            {
                ilGen.Emit(OpCodes.Newobj, newExpr.Constructor);
            }
            else
            {
                LocalBuilder temp = ilGen.DeclareLocal(newExpr.Type);
                ilGen.Emit(OpCodes.Ldloca, temp);
                ilGen.Emit(OpCodes.Initobj, newExpr.Type);
                ilGen.Emit(OpCodes.Ldloc, temp);
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T>)method.CreateDelegate(typeof(Func<T>));
        }
    }
}
