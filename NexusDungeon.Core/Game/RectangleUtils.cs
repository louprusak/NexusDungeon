using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusDungeon.Core.Game
{
    public static class RectangleUtils
    {
        /// <summary>
        /// Obtient la position du centre du bord inférieur du rectangle.
        /// </summary>
        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
    }
}
