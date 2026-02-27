using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if a connection is valid based on the color of the dots and the connection.
/// </summary>
public class ColorRule : IDotConnectionRule
{
    /// <summary>
    /// Checks if the connection is valid based on the color of the dots and the connection.
    /// </summary>
    /// <param name="fromDot">The dot that is being connected</param>
    /// <param name="toDot">The dot to connect to</param>
    /// <param name="connection">The connection model</param>
    /// <param name="board">The board presenter object</param>
    /// <returns>Wether or not the two dots can form a valid connection</returns>
    public bool CanConnect(IDotPresenter fromDot, IDotPresenter toDot, ConnectionModel connection, IBoardPresenter board)
    {
        var toColorable = toDot.Dot.GetComponent<ColorableModel>();
        var fromColorable = fromDot.Dot.GetComponent<ColorableModel>();
        var connectionColor = connection.CurrentColor;

        if (!CheckConnectionMatch(connectionColor, fromColorable, toColorable))
        {
            return false;
        }
        if (!CheckDotColorMatch(fromColorable, toColorable))
        {
            return false;
        }
        return true;
    }

    private bool CheckConnectionMatch(DotColor connectionColor, ColorableModel fromDot, ColorableModel toDot)
    {
        if (connectionColor.IsBlank())
        {
            return true;
        }
        if (toDot.Equals(connectionColor) && fromDot.Equals(connectionColor))
        {
            return true;
        }
        return false;
    }

    private bool CheckDotColorMatch(ColorableModel fromDot, ColorableModel toDot)
    {
        var fromColor = fromDot.GetComparableColor(toDot.Color);
        var toColor = toDot.GetComparableColor(fromDot.Color);
        if (fromColor == toColor)
        {

            return true;
        }
        return false;
    }
}