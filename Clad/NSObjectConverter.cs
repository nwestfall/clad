﻿using System;
using Foundation;
using System.Globalization;
using System.Collections.Generic;
using CoreGraphics;

namespace Clad
{
    /// <summary>
    /// NS object converter is a helper class that helps to convert NSObjects into
    /// C# objects
    /// </summary>
    public static class NSObjectConverter
    {
        #region Static Methods
        /// <summary>
        /// Converts to an object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="nsO">Ns o.</param>
        /// <param name="targetType">Target type.</param>
        public static Object ToObject(this NSObject nsO, Type targetType)
        {
            if (nsO is NSString)
            {
                return nsO.ToString();
            }

            if (nsO is NSDate)
            {
                var nsDate = (NSDate)nsO;
                return DateTime.SpecifyKind((DateTime)nsDate, DateTimeKind.Unspecified);
            }

            if (nsO is NSDecimalNumber)
            {
                return decimal.Parse(nsO.ToString(), CultureInfo.InvariantCulture);
            }

            if (nsO is NSNumber)
            {
                var x = (NSNumber)nsO;

                switch (Type.GetTypeCode(targetType))
                {
                    case TypeCode.Boolean:
                        return x.BoolValue;
                    case TypeCode.Char:
                        return Convert.ToChar(x.ByteValue);
                    case TypeCode.SByte:
                        return x.SByteValue;
                    case TypeCode.Byte:
                        return x.ByteValue;
                    case TypeCode.Int16:
                        return x.Int16Value;
                    case TypeCode.UInt16:
                        return x.UInt16Value;
                    case TypeCode.Int32:
                        return x.Int32Value;
                    case TypeCode.UInt32:
                        return x.UInt32Value;
                    case TypeCode.Int64:
                        return x.Int64Value;
                    case TypeCode.UInt64:
                        return x.UInt64Value;
                    case TypeCode.Single:
                        return x.FloatValue;
                    case TypeCode.Double:
                        return x.DoubleValue;
                }
            }

            if (nsO is NSValue)
            {
                var v = (NSValue)nsO;

                if (targetType == typeof(IntPtr))
                {
                    return v.PointerValue;
                }

                if (targetType == typeof(CGSize))
                {
                    return v.SizeFValue;
                }

                if (targetType == typeof(CGRect))
                {
                    return v.RectangleFValue;
                }

                if (targetType == typeof(CGPoint))
                {
                    return v.PointFValue;
                }
            }

            return nsO;
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="nsO">Ns o.</param>
        public static string ToString(this NSObject nsO)
        {
            return (string)ToObject(nsO, typeof(string));
        }

        /// <summary>
        /// Convert to date time
        /// </summary>
        /// <returns>The date time.</returns>
        /// <param name="nsO">Ns o.</param>
        public static DateTime ToDateTime(this NSObject nsO)
        {
            return (DateTime)ToObject(nsO, typeof(DateTime));
        }

        /// <summary>
        /// Convert to decimal number
        /// </summary>
        /// <returns>The decimal.</returns>
        /// <param name="nsO">Ns o.</param>
        public static decimal ToDecimal(this NSObject nsO)
        {
            return (decimal)ToObject(nsO, typeof(decimal));
        }

        /// <summary>
        /// Convert to boolean
        /// </summary>
        /// <returns><c>true</c>, if bool was toed, <c>false</c> otherwise.</returns>
        /// <param name="nsO">Ns o.</param>
        public static bool ToBool(this NSObject nsO)
        {
            return (bool)ToObject(nsO, typeof(bool));
        }

        /// <summary>
        /// Convert to character
        /// </summary>
        /// <returns>The char.</returns>
        /// <param name="nsO">Ns o.</param>
        public static char ToChar(this NSObject nsO)
        {
            return (char)ToObject(nsO, typeof(char));
        }

        /// <summary>
        /// Convert to integer
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="nsO">Ns o.</param>
        public static int ToInt(this NSObject nsO)
        {
            return (int)ToObject(nsO, typeof(int));
        }

        /// <summary>
        /// Convert to float
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="nsO">Ns o.</param>
        public static float ToFloat(this NSObject nsO)
        {
            return (float)ToObject(nsO, typeof(float));
        }

        /// <summary>
        /// Converts to double
        /// </summary>
        /// <returns>The double.</returns>
        /// <param name="nsO">Ns o.</param>
        public static double ToDouble(this NSObject nsO)
        {
            return (double)ToObject(nsO, typeof(double));
        }

        /// <summary>
        /// Converts a List<string> to a NSString array
        /// </summary>
        /// <returns>The NSString array.</returns>
        /// <param name="list">List.</param>
        public static NSString[] ToNSStringArray(this List<string> list)
        {
            var nsArray = new NSString[list.Count];
            for (var i = 0; i < list.Count; i++)
                nsArray[i] = new NSString(list[i]);
            return nsArray;
        }
        #endregion
    }
}
