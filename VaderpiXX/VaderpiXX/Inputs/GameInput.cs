using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace VaderpiXX.Inputs
{
    class GameInput
    {
        Dictionary<string, Input> inputs = new Dictionary<string, Input>();

        public GameInput()
        {
        }

        public Input MyInput(string action)
        {
            // Add the action, if it doesn't already exist
            if (!inputs.ContainsKey(action))
            {
                inputs.Add(action, new Input());
            }

            return inputs[action];
        }

        public void BeginUpdate()
        {
            Input.BeginUpdate();
        }

        public void EndUpdate()
        {
            Input.EndUpdate();
        }

        public bool IsConnected(PlayerIndex player)
        {
            // If there was never a gamepad connected, fake the return value
            if (!Input.GamepadConnectionState[player])
            {
                return true;
            }

            return Input.IsConnected(player);
        }

        public bool IsPressed(string action, Rectangle currentObjectLocation)
        {
            if (!inputs.ContainsKey(action))
            {
                return false;
            }

            return inputs[action].IsPressed(PlayerIndex.One, currentObjectLocation);
        }

        public bool IsPressed(string action)
        {
            if (!inputs.ContainsKey(action))
            {
                return false;
            }

            return inputs[action].IsPressed(PlayerIndex.One);
        }

        public bool IsPressed(string action, PlayerIndex player)
        {
            if (!inputs.ContainsKey(action))
            {
                return false;
            }

            return inputs[action].IsPressed(player);
        }

        public bool IsPressed(string action, PlayerIndex? player)
        {
            if (player == null)
            {
                PlayerIndex controllingPlayer;
                return IsPressed(action, player, out controllingPlayer);
            }

            return IsPressed(action, player.Value);
        }

        public bool IsPressed(string action, PlayerIndex? player, out PlayerIndex controllingPlayer)
        {
            if (!inputs.ContainsKey(action))
            {
                controllingPlayer = PlayerIndex.One;
                return false;
            }

            if (player == null)
            {
                if (IsPressed(action, PlayerIndex.One))
                {
                    controllingPlayer = PlayerIndex.One;
                    return true;
                }

                if (IsPressed(action, PlayerIndex.Two))
                {
                    controllingPlayer = PlayerIndex.Two;
                    return true;
                }

                if (IsPressed(action, PlayerIndex.Three))
                {
                    controllingPlayer = PlayerIndex.Three;
                    return true;
                }

                if (IsPressed(action, PlayerIndex.Four))
                {
                    controllingPlayer = PlayerIndex.Four;
                    return true;
                }

                controllingPlayer = PlayerIndex.One;
                return true;
            }

            controllingPlayer = (PlayerIndex)player;
            return IsPressed(action, controllingPlayer);
        }

        public void AddGamepadInput(string action, Buttons button, bool isReleasedPreviously)
        {
            MyInput(action).AddGamepadInput(button,
                                            isReleasedPreviously);
        }

        public void AddKeyboardInput(string action, Keys key, bool isReleasedPreviously)
        {
            MyInput(action).AddKeyboardInput(key,
                                             isReleasedPreviously);
        }

        public void AddTouchTapInput(string action, Rectangle touchArea, bool isReleasedPreviously)
        {
            MyInput(action).AddTouchTapInput(touchArea,
                                             isReleasedPreviously);
        }

        public void AddTouchSlideInput(string action, Input.Direction direction, float slideDistance)
        {
            MyInput(action).AddTouchSlideInput(direction,
                                               slideDistance);
        }

        public void AddTouchGestureInput(string action, GestureType gesture, Rectangle area)
        {
            MyInput(action).AddTouchGestureInput(gesture,
                                                 area);
        }

        public void AddAccelerometerInput(string action, Input.Direction direction, float tiltThreshold)
        {
            MyInput(action).AddAccelerometerInput(direction,
                                                  tiltThreshold);
        }

        public Vector2 CurrentGesturePosition(string action)
        {
            return MyInput(action).CurrentGesturePosition();
        }

        public Vector2 CurrentGestureDelta(string action)
        {
            return MyInput(action).CurrentGestureDelta();
        }

        public Vector2 CurrentGesturePosition2(string action)
        {
            return MyInput(action).CurrentGesturePosition2();
        }

        public Vector2 CurrentGestureDelta2(string action)
        {
            return MyInput(action).CurrentGestureDelta2();
        }

        public Point CurrentTouchPoint(string action)
        {
            Vector2? currentPosition = MyInput(action).CurrentTouchPosition();

            if (currentPosition == null)
            {
                return new Point(-1, -1);
            }

            return new Point((int)currentPosition.Value.X,
                             (int)currentPosition.Value.Y);
        }

        public Vector2 CurrentTouchPosition(string action)
        {
            Vector2? currentPosition = MyInput(action).CurrentTouchPosition();

            if (currentPosition == null)
            {
                return new Vector2(-1, -1);
            }

            return currentPosition.Value; // ??? does it work?
        }

        public float CurrentGestureScaleChange(string action)
        {
            // if no Pinch esture is activated, return zero
            if (!MyInput(action).PinchGestureAvailable)
            {
                return 0.0f;
            }

            // Get the current and previous position of the fingers
            Vector2 currentPositionFingerOne = CurrentGesturePosition(action);

            Vector2 previousPositionFingerOne = CurrentGesturePosition(action)
                                                - CurrentGestureDelta(action);

            Vector2 currentPositionFingerTwo = CurrentGesturePosition2(action);

            Vector2 previousPositionFingerTwo = CurrentGesturePosition2(action)
                                                - CurrentGestureDelta2(action);

            //Figure out the distance between current and previous position
            float currentDistance = Vector2.Distance(currentPositionFingerOne,
                                                     currentPositionFingerTwo);

            float previousDistance = Vector2.Distance(previousPositionFingerOne,
                                                      previousPositionFingerTwo);

            // Calculate the diff between both and use it to alter the scale
            float scaleChange = (currentDistance - previousDistance) * 0.01f;

            return scaleChange;
        }

        public Vector3 CurrentAccelerometerReading(string action)
        {
            return MyInput(action).CurrentAccelerometerReading;
        }
    }
}
