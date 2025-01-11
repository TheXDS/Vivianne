/*
IInValueConverter.cs

This file is part of Morgan's CLR Advanced Runtime (MCART)

Author(s):
     César Andrés Morgan <xds_xps_ivx@hotmail.com>

Released under the MIT License (MIT)
Copyright © 2011 - 2024 César Andrés Morgan

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Globalization;
using System.Windows.Data;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Vivianne.ValueConverters.Base;

/// <summary>
/// Defines a contract for a value converter that converts values from a specified input type.
/// </summary>
/// <typeparam name="TIn">The type of the input value accepted by the converter.</typeparam>
public interface IInValueConverter<TIn> : IValueConverter
{
    /// <summary>
    /// Gets the source type of the input value accepted by the converter.
    /// </summary>
    Type SourceType => typeof(TIn);

    /// <summary>
    /// Gets the target type of the value produced by the converter.
    /// </summary>
    Type TargetType { get; }

    /// <inheritdoc/>
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        return Convert(value is TIn i ? i : (TIn)typeof(TIn).Default()!, targetType, parameter, culture);
    }

    /// <summary>
    /// Converts a value of type <typeparamref name="TIn"/> to the specified target type.
    /// </summary>
    /// <param name="value">The input value to convert.</param>
    /// <param name="targetType">The type to convert the value to.</param>
    /// <param name="parameter">An optional parameter to use in the conversion.</param>
    /// <param name="culture">The culture information to use in the conversion.</param>
    /// <returns>
    /// The converted value of the target type or null if the conversion fails.
    /// </returns>
    object? Convert(TIn value, Type targetType, object? parameter, CultureInfo? culture);
}

