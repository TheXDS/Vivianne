/*
IOneWayValueConverter.cs

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
/// Defines a contract for a one-way value converter that converts values from a specified input type to a specified output type.
/// </summary>
/// <typeparam name="TIn">The type of the input value accepted by the converter.</typeparam>
/// <typeparam name="TOut">The type of the output value produced by the converter.</typeparam>
public interface IOneWayValueConverter<TIn, TOut> : IInValueConverter<TIn>, IOutValueConverter<TOut>
{
    /// <inheritdoc/>
    Type IInValueConverter<TIn>.TargetType => typeof(TOut);

    /// <inheritdoc/>
    object? IInValueConverter<TIn>.Convert(TIn value, Type targetType, object? parameter, CultureInfo? culture)
    {
        return Convert(value, parameter, culture);
    }

    /// <inheritdoc/>
    TOut IOutValueConverter<TOut>.Convert(object? value, object? parameter, CultureInfo? culture)
    {
        return Convert(value is TIn i ? i : (TIn)typeof(TIn).Default()!, parameter, culture);
    }

    /// <inheritdoc/>
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        return Convert(value is TIn i ? i : (TIn)typeof(TIn).Default()!, parameter, culture);
    }

    /// <summary>
    /// Converts a value of type <typeparamref name="TIn"/> to the specified output type.
    /// </summary>
    /// <param name="value">The input value to convert.</param>
    /// <param name="parameter">An optional parameter to use in the conversion.</param>
    /// <param name="culture">The culture information to use in the conversion.</param>
    /// <returns>
    /// The converted value of type <typeparamref name="TOut"/> or null if the conversion fails.
    /// </returns>
    TOut Convert(TIn value, object? parameter, CultureInfo? culture);

    /// <inheritdoc/>
    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        throw new InvalidOperationException();
    }
}
