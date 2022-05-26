using ArBreakout.Common;
using UnityEngine;

namespace ArBreakout.GamePhysics
{
    public static class BreakoutPhysics
    {
        public const float LevelDimY = 19.0f;

        private const float MaxVelocity = 2.0f;

        /*
         * Stores information about the collision. Such as the surface normal and the depth of the penetration.
         */
        public struct Contact
        {
            public float Separation;
            public Vector3 Normal;
            public Vector3 Point;
        }

        public static Contact ExtractContactPoint(Collision other)
        {
            var result = new Contact();
            var contactCount = other.contactCount;
            var contactPoints = new ContactPoint[contactCount];

            other.GetContacts(contactPoints);
            foreach (var contact in contactPoints)
            {
                var absSeparation = Mathf.Abs(contact.separation);
                if (absSeparation > result.Separation)
                {
                    result.Separation = absSeparation;
                    result.Normal = contact.normal;
                    result.Point = contact.point;
                }
            }

            return result;
        }

        public static Vector3 CalculateVelocityDelta(Vector3 acceleration, float maxVelocity = MaxVelocity)
        {
            return Vector3.ClampMagnitude(acceleration * GameTime.FixedDelta, maxVelocity);
        }

        /*
         * Determines delta movement based on the equation of motion.
         */
        public static Vector3 CalculateMovementDelta(Vector3 acceleration, Vector3 velocity)
        {
            return (0.5f * acceleration * Mathf.Pow(GameTime.FixedDelta, 2.0f) + velocity * GameTime.FixedDelta);
        }
    }
}