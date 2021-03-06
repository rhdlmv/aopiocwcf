﻿namespace A.Validation
{
    using System;
    

    public static class ObjectExtension
    {
        public static T Validate<T>(this T obj, IRuleContext ruleContext, string action) where T: class
        {
            ruleContext.Validate<T>(obj, action);
            return obj;
        }
    }
}

