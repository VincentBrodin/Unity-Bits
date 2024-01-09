using UnityEngine;

namespace Tools
{
    public static class Formations
    {
        private static Vector3[] _positions; 
        public static Vector3[] GetFormationPositions(Vector3 center, int unitCount, float spacing)
        {
            var sideLength = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
            _positions = new Vector3[unitCount];

            var index = 0;
            for (var i = 0; i < sideLength; i++)
            {
                for (var j = 0; j < sideLength; j++)
                {
                    if (index >= unitCount)
                        break;

                    var offsetX = (i - (float)sideLength / 2) * spacing;
                    var offsetZ = (j - (float)sideLength / 2) * spacing;
                    _positions[index] = new Vector3(center.x + offsetX, center.y, center.z + offsetZ);
                    index++;
                }
            }

            return _positions;
        }
    }
}
