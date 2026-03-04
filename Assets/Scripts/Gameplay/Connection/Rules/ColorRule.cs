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
    public bool CanConnect(IDotPresenter fromDot, IDotPresenter toDot, IConnectionModel connection, IBoardPresenter board)
    {
        var toColorable = toDot.Dot.GetModel<ColorableModel>();
        var connectionColor = connection.CurrentColor;

        if (!CheckConnectionMatch(connectionColor, toColorable))
        {
            return false;
        }
       
        return true;
    }

    private bool CheckConnectionMatch(DotColor connectionColor, ColorableModel toDot)
    {
        // If the connection color is blank, we can connect to any dot
        if (connectionColor.IsBlank())
        {
            return true;
        }
        // Get the real (or surrogate) color of the dot we are trying to connect to
        var toColor = toDot.GetComparableColor(connectionColor);
         // Match is true if the color we are trying to connect to is the same as the connection color
        if (connectionColor == toColor)
        {
            return true;
        }
         return false;
       
    }
}