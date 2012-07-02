using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;

namespace VaderpiXX.Inputs
{
    class GestureDefinition
    {
        public GestureType Type;
        public Rectangle CollisionArea;
        public GestureSample Gesture;
        public Vector2 Delta;
        public Vector2 Delta2;
        public Vector2 Position;
        public Vector2 Position2;

        public GestureDefinition(GestureType gestureType, Rectangle gestureArea)
        {
            Gesture = new GestureSample(gestureType,
                                        new TimeSpan(0),
                                        Vector2.Zero,
                                        Vector2.Zero,
                                        Vector2.Zero,
                                        Vector2.Zero);
            Type = gestureType;
            CollisionArea = gestureArea;
        }

        public GestureDefinition(GestureSample gesture)
        {
            Gesture = gesture;
            Type = gesture.GestureType;
            CollisionArea = new Rectangle((int)gesture.Position.X,
                                          (int)gesture.Position.Y,
                                          5,
                                          5);
            Delta = gesture.Delta;
            Delta2 = gesture.Delta2;
            Position = gesture.Position;
            Position2 = gesture.Position2;
        }
    }
}
