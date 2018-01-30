﻿// /*******************************************************************************
//  * Copyright 2012-2018 Esri
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *  http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *   Unless required by applicable law or agreed to in writing, software
//  *   distributed under the License is distributed on an "AS IS" BASIS,
//  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *   See the License for the specific language governing permissions and
//  *   limitations under the License.
//  ******************************************************************************/

#if !__IOS__ && !__ANDROID__

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Esri.ArcGISRuntime.Toolkit.Internal
{
    /// <summary>
    /// Provides utility extension methods related to data-bindings
    /// </summary>
    static internal class BindingExtensions
    {
        /// <summary>
        /// Creates a copy of the specified binding
        /// </summary>
        /// <param name="source">The binding to copy</param>
        /// <param name="newConverterParameter">The object to use as a converter parameter</param>
        /// <returns>The copy of the binding</returns>
        public static Binding Clone(this Binding source, object newConverterParameter = null)
        {
            var copy = new Binding
            {
                Converter = source.Converter,
                // Can't change the ConverterParameter after instantation on UWP, so set it here
                ConverterParameter = newConverterParameter == null ? source.ConverterParameter : newConverterParameter,
                FallbackValue = source.FallbackValue,
                Mode = source.Mode,
                Path = source.Path,
                TargetNullValue = source.TargetNullValue,
                UpdateSourceTrigger = source.UpdateSourceTrigger,
#if NETFX_CORE
                ConverterLanguage = source.ConverterLanguage
#else
                AsyncState = source.AsyncState,
                BindingGroupName = source.BindingGroupName,
                BindsDirectlyToSource = source.BindsDirectlyToSource,
                ConverterCulture = source.ConverterCulture,
                IsAsync = source.IsAsync,
                NotifyOnSourceUpdated = source.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = source.NotifyOnTargetUpdated,
                NotifyOnValidationError = source.NotifyOnValidationError,
                StringFormat = source.StringFormat,
                UpdateSourceExceptionFilter = source.UpdateSourceExceptionFilter,
                ValidatesOnDataErrors = source.ValidatesOnDataErrors,
                ValidatesOnExceptions = source.ValidatesOnExceptions,
                XPath = source.XPath,
#endif
            };
            if (source.Source != null)
            {
                copy.Source = source.Source;
            }
            else if (!string.IsNullOrEmpty(source.ElementName))
            {
                copy.ElementName = source.ElementName;
            }
            else if (source.RelativeSource != null)
            {
                copy.RelativeSource = source.RelativeSource;
            }

            return copy;
        }

        /// <summary>
        /// Applies the specified format string to the binding of the specified property on the specified element.  Also initializes a fallback string format from the
        /// original string format if one is not already referenced in the corresponding parameter.
        /// </summary>
        /// <param name="bindingTarget">The element for which to update the property</param>
        /// <param name="targetProperty">The dependency property to update</param>
        /// <param name="stringFormat">The new format string</param>
        /// <param name="fallbackFormat">The fallback format string.  Used if the new format string is null or empty.</param>
        public static void UpdateStringFormat(this FrameworkElement bindingTarget, DependencyProperty targetProperty, string stringFormat, ref string fallbackFormat)
        {
            if (bindingTarget == null)
                return;

            // Get the binding for the target property
            var binding = bindingTarget.GetBindingExpression(targetProperty)?.ParentBinding;
            if (binding == null)
                return; // Or should we create a new binding in this case?

            // If the fallback hasn't been populated already, store the current string format as specified by the binding
#if NETFX_CORE
            if (fallbackFormat == null)
                fallbackFormat = binding.Converter is StringFormatConverter ? binding.ConverterParameter as string : null;
#else
            if (fallbackFormat == null)
                fallbackFormat = binding.StringFormat;
#endif
            // Create a new binding to apply the format string.  Necessary because bindings that are already in use cannot be updated,
            // but we want to preserve how users may have setup the binding otherwise
            var newStringFormat = !string.IsNullOrEmpty(stringFormat) ? stringFormat : fallbackFormat;
            var newBinding = binding.Clone(newStringFormat);
            // If the format string is null or empty, use the fall back format.  Otherwise, apply the new format.
            bindingTarget.SetBinding(targetProperty, newBinding);
        }
    }
}

#endif