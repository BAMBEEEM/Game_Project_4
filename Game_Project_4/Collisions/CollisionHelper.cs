using Game_Project_4.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.Collisions
{
    public static class CollisionHelper
    {

        /// <summary>
        /// Detects a collision between two BoundingRectangles
        /// </summary>
        /// <param name="a">The first BoundingRectangle</param>
        /// <param name="b">The second BoundingRectangle</param>
        /// <returns>true fir collision, false otherwise</returns>
        public static bool Collides(BoundingRectangle a, BoundingRectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right ||
                     a.Top > b.Bottom || a.Bottom < b.Top);
        }

        /// <summary>
        /// Calculates the closeness between two BoundingRectangles
        /// </summary>
        /// <param name="a">The first position</param>
        /// <param name="b">The second position</param>
        /// <returns>the closest point between two vector positions</returns>
        public static float Closeness(Vector2 a, Vector2 b)
        {


            return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }



    }
}
