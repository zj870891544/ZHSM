using UnityEngine;
using ZHSM.cfg;

namespace ZHSM
{
    public static class LubanUtility
    {
        public static Vector3 ParsePosition(Position position)
        {
            return new Vector3(position.X, position.Y, position.Z);
        }

        public static Quaternion ParseRotation(Rotation rotation)
        {
            return new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
        }
    }
}