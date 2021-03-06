﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDefinition.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Describes a property to be presented in a column (or row).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools.Wpf.ItemsGrid
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using PropertyTools.DataAnnotations;

    /// <summary>
    /// Describes a property to be presented in a column (or row).
    /// </summary>
    public class PropertyDefinition
    {
        /// <summary>
        /// Gets or sets the descriptor.
        /// </summary>
        /// <value>The descriptor.</value>
        public PropertyDescriptor Descriptor { get; set; }
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header { get; set; }
        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; }
        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public IValueConverter Converter { get; set; }
        /// <summary>
        /// Gets or sets the converter parameter.
        /// </summary>
        /// <value>The converter parameter.</value>
        public object ConverterParameter { get; set; }
        /// <summary>
        /// Gets or sets the converter culture.
        /// </summary>
        /// <value>The converter culture.</value>
        public CultureInfo ConverterCulture { get; set; }
        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the column width.
        /// </summary>
        /// <value>The width.</value>
        public GridLength Width { get; set; }

        /// <summary>
        /// Gets or sets the max length (for TextBox).
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the property name of an items source (for ComboBox).
        /// </summary>
        /// <value>The items source property.</value>
        public string ItemsSourceProperty { get; set; }

        /// <summary>
        /// Gets or sets the items source (for ComboBox).
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable (for ComboBox).
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDefinition" /> class.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        public PropertyDefinition(PropertyDescriptor propertyDescriptor)
        {
            this.Descriptor = propertyDescriptor;
            this.Header = Descriptor.Name;
            this.Width = GridLength.Auto;
            this.MaxLength = int.MaxValue;

            var ispa = GetAttribute<ItemsSourcePropertyAttribute>();
            if (ispa != null)
            {
                ItemsSourceProperty = ispa.PropertyName;
            }

            if (Descriptor.PropertyType.Is(typeof(Enum)))
            {
                SetEnumItemsSource();
            }
        }

        /// <summary>
        /// Sets the enum items source.
        /// </summary>
        protected virtual void SetEnumItemsSource()
        {
            ItemsSource = Enum.GetValues(Descriptor.PropertyType);
        }

        /// <summary>
        /// Gets the first attribute of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of attribute.</typeparam>
        /// <returns>
        /// The attribute, or <c>null</c>.
        /// </returns>
        public T GetAttribute<T>() where T : Attribute
        {
            var type = typeof(T);
            foreach (var a in this.Descriptor.Attributes)
            {
                if (type.IsAssignableFrom(a.GetType()))
                {
                    return a as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the binding.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <returns>
        /// 
        /// </returns>
        public Binding CreateBinding(UpdateSourceTrigger trigger = UpdateSourceTrigger.Default)
        {
            var bindingMode = this.Descriptor.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            var formatString = this.FormatString;
            if (formatString != null && !formatString.StartsWith("{"))
            {
                formatString = "{0:" + formatString + "}";
            }

            var binding = new Binding(this.Descriptor.Name)
                {
                    Mode = bindingMode,
                    Converter = this.Converter,
                    ConverterParameter = this.ConverterParameter,
                    StringFormat = formatString,
                    UpdateSourceTrigger = trigger,
                    ValidatesOnDataErrors = true,
                    ValidatesOnExceptions = true
                };
            if (this.ConverterCulture != null)
            {
                binding.ConverterCulture = this.ConverterCulture;
            }

            return binding;
        }

        /// <summary>
        /// Creates the one way binding.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public Binding CreateOneWayBinding()
        {
            var b = this.CreateBinding();
            b.Mode = BindingMode.OneWay;
            return b;
        }
    }
}