﻿namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    using JetBrains.Annotations;

    /// <summary>
    /// Unary operations supported by the <see cref="UnaryOperationConverter"/>
    /// </summary>
    public enum UnaryOperation
    {
        /// <summary>
        /// The negation operation; either a boolean or atithmetic negation.
        /// </summary>
        Negation
    }

    /// <summary>
    /// Applies the <see cref="Operation"/> on the value.<para/>
    /// Either a boolean or an arithetic operation for built in types, or the negation defined by the objects negation operator overload.<para/>
    /// </summary>
    /// <returns>
    /// If the conversions succeed, the result of the operation is returned. If any error occurs, the result is <see cref="DependencyProperty.UnsetValue"/>.
    /// </returns>
    public class UnaryOperationConverter : ValueConverter
    {
        [NotNull, ItemNotNull]
        private static readonly string[] _operationMethodNames = { "op_UnaryNegation" };

        /// <summary>
        /// The default negation converter.
        /// </summary>
        [NotNull]
        public static readonly IValueConverter Negation = new UnaryOperationConverter();

        /// <summary>
        /// Gets or sets the operation performed by this converter.<para/>
        /// Even though this converter supports only one operation, the property is present to have the same look and feel like the <see cref="BinaryOperationConverter"/>.
        /// </summary>
        public UnaryOperation Operation
        {
            get;
            set;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        [ContractVerification(false)]
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            while (true)
            {
                var valueType = value.GetType();

                switch (Type.GetTypeCode(valueType))
                {
                    case TypeCode.Boolean:
                        return !(bool)value;

                    case TypeCode.SByte:
                        return -(sbyte)value;
                    case TypeCode.Int16:
                        return -(short)value;
                    case TypeCode.Int32:
                        return -(int)value;
                    case TypeCode.Int64:
                        return -(long)value;
                    case TypeCode.Single:
                        return -(float)value;
                    case TypeCode.Double:
                        return -(double)value;
                    case TypeCode.Decimal:
                        return -(decimal)value;

                    case TypeCode.Object:
                        return ApplyOperation(value, valueType);

                    case TypeCode.String:
                        if ((targetType != null) && (targetType != typeof(string)))
                        {
                            value = ChangeType((string)value, targetType);
                            if (value == null)
                                return null;
                            continue;
                        }
                        break;
                }

                throw new ArgumentOutOfRangeException(nameof(value), "Unsupported type");
            }
        }

        [CanBeNull]
        private static object ApplyOperation([CanBeNull] object value, [NotNull] Type valueType)
        {
            Contract.Requires(valueType != null);

            var methods = valueType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            // ReSharper disable PossibleNullReferenceException
            return methods
                .Where(m => _operationMethodNames.Contains(m.Name))
                .Select(m => new { Method = m, Parameters = m.GetParameters() })
                .Where(m => m.Parameters.Length == 1)
                .Where(m => m.Parameters[0].ParameterType == valueType)
                .Select(m => m.Method.Invoke(null, new[] { value }))
                .FirstOrDefault(v => v != null);
            // ReSharper restore PossibleNullReferenceException
        }

        [CanBeNull]
        private static object ChangeType([CanBeNull] string value, [NotNull] Type targetType)
        {
            Contract.Requires(targetType != null);

            var typeConverter = TypeDescriptor.GetConverter(targetType);
            return typeConverter.ConvertFromInvariantString(value);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">ConvertBack is not supported by this converter.</exception>
        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
