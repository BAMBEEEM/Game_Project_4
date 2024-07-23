using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.Misc
{
    /// <summary>
    /// A static class that helps with storing and unifying camera settings between all sprites.
    /// </summary>
    public static class CameraSettings
    {
        public static Matrix transform = Matrix.Identity;

        public static Matrix WaveShakeEffect(float _shakeTime)
        {
            float shaketime = _shakeTime / 220; //RandomHelper.NextFloat(80, 240)
            Matrix shakeTranslation = Matrix.CreateTranslation(8 * MathF.Sin(shaketime), 15 * MathF.Cos(shaketime), 0);
            return shakeTranslation;
        }

        public static Matrix DrownShakeEffect(float _shakeTime, bool isDrowning)
        {

            float shaketime = _shakeTime / 140;
            if (!isDrowning) return Matrix.Identity;
            else
            {
                Matrix shakeTranslation = Matrix.CreateTranslation(20 * MathF.Sin(shaketime), 3 * MathF.Cos(shaketime), 0);
                return shakeTranslation;
            }

        }







    }
}
