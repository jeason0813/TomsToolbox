﻿namespace TomsToolbox.Wpf.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// The logical operation performed by the <see cref="LogicalMultiValueConverter" />
    /// </summary>
    public enum LogicalOperation
    {
        /// <summary>
        /// The logical AND operation; returns true if all items are true.
        /// </summary>
        And,
        /// <summary>
        /// The logical OR operation; returns true if any item is true.
        /// </summary>
        Or,
    }

    /// <summary>
    /// A <see cref="IMultiValueConverter" /> that performs a logical operation on all items.
    /// </summary>
    /// <remarks>
    /// All items must be convertible to boolean.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Use the same term as in IMultiValueConverter")]
    public class LogicalMultiValueConverter : IMultiValueConverter
    {
        private static readonly Func<IEnumerable<bool>, bool> _andOperationMethod = items => items.All(item => item);
        private static readonly Func<IEnumerable<bool>, bool> _orOperationMethod = items => items.Any(item => item);

        private LogicalOperation _operation;
        private Func<IEnumerable<bool>, bool> _operationMethod = _andOperationMethod;

        /// <summary>
        /// The default logical AND converter. 
        /// </summary>
        public static readonly IMultiValueConverter And = new LogicalMultiValueConverter { Operation = LogicalOperation.And };
        /// <summary>
        /// The default logical OR converter. 
        /// </summary>
        public static readonly IMultiValueConverter Or = new LogicalMultiValueConverter { Operation = LogicalOperation.Or };

        /// <summary>
        /// Gets or sets the operation to be performed on all items.
        /// </summary>
        public LogicalOperation Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;

                switch (value)
                {
                    case LogicalOperation.And:
                        _operationMethod = _andOperationMethod;
                        break;

                    case LogicalOperation.Or:
                        _operationMethod = _orOperationMethod;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("value", value, null);
                }
            }
        }

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return false;

            try
            {
                return _operationMethod(values.Select(v => System.Convert.ToBoolean(v, CultureInfo.InvariantCulture)));
            }
            catch (SystemException)
            {
            }

            return false;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_operationMethod != null);
        }
    }
}