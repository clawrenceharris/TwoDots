using System;


public interface IColorableModel : IEquatable<DotColor>
{
    DotColor Color { get; set; }

    /// <summary>
    /// Determines the resulting color to use in color equivalency validation rules.
    /// For dots with a fixed color, this returns their own color.
    /// For wildcard/blank dots, this may return the provided color, acting as a color surrogate.
    /// This method answers the question: "If I connect with you, what color should be considered for comparison?"
    /// </summary>
    /// <param name="other">The color of the other dot being evaluated for equivalency.</param>
    /// <returns>
    /// The color to use for equivalency checks—either the internal color or the given color, depending on the implementing dot type.
    /// </returns>
    DotColor GetComparableColor(DotColor other);
}