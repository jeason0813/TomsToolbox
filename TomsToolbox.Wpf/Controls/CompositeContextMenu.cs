﻿namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition;

    /// <inheritdoc />
    /// <summary>
    /// A markup extension to create a <see cref="T:TomsToolbox.Wpf.Controls.CompositeContextMenu" /> in XAML.
    /// </summary>
    [MarkupExtensionReturnType(typeof(ContextMenu))]
    public class CompositeContextMenuExtension : MarkupExtension
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Controls.CompositeContextMenuExtension" /> class.
        /// </summary>
        public CompositeContextMenuExtension()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Controls.CompositeContextMenuExtension" /> class.
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        public CompositeContextMenuExtension([CanBeNull] string regionId)
        {
            RegionId = regionId;
        }

        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        [CanBeNull]
        public string RegionId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a binding to seed the <see cref="IVisualCompositionBehavior.CompositionContext"/>.
        /// </summary>
        [CanBeNull]
        public Binding CompositionContextBinding
        {
            get;
            set;
        }

        /// <inheritdoc />
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var compositeContextMenu = new CompositeContextMenu()
            {
                RegionId = RegionId
            };

            if (CompositionContextBinding != null)
            {
                BindingOperations.SetBinding(compositeContextMenu, CompositeContextMenu.CompositionContextProperty, CompositionContextBinding);
            }

            return compositeContextMenu;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// A context menu that uses the composition framework to build it's content 
    /// dynamically by collecting all exported <see cref="T:TomsToolbox.Wpf.Composition.CommandSourceFactory" /> objects 
    /// with the matching region.
    /// </summary>
    public class CompositeContextMenu : ContextMenu
    {
        static CompositeContextMenu()
        {
            // ReSharper disable once PossibleNullReferenceException
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompositeContextMenu), new FrameworkPropertyMetadata(typeof(CompositeContextMenu)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Controls.CompositeContextMenu" /> class.
        /// </summary>
        public CompositeContextMenu()
        {
            var compositionBehavior = new ItemsControlCompositionBehavior();
            BindingOperations.SetBinding(compositionBehavior, ItemsControlCompositionBehavior.RegionIdProperty, new Binding() { Source = this, Path = new PropertyPath(RegionIdProperty) });

            var behaviors = Interaction.GetBehaviors(this);
            Contract.Assume(behaviors != null);
            behaviors.Add(compositionBehavior);
        }

        /// <summary>
        /// Gets or sets the region identifier for which to collect all exported <see cref="CommandSourceFactory"/> objects.
        /// </summary>
        [CanBeNull]
        public string RegionId
        {
            get => (string)GetValue(RegionIdProperty);
            set => SetValue(RegionIdProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="RegionId"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty RegionIdProperty =
            DependencyProperty.Register("RegionId", typeof(string), typeof(CompositeContextMenu));


        /// <summary>
        /// Gets or sets the composition context.
        /// </summary>
        [CanBeNull]
        public object CompositionContext
        {
            get => GetValue(CompositionContextProperty);
            set => SetValue(CompositionContextProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="CompositionContext"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty CompositionContextProperty =
            DependencyProperty.Register("CompositionContext", typeof (object), typeof (CompositeContextMenu));
    }
}
