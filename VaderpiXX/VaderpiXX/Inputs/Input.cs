using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;

namespace VaderpiXX.Inputs
{
    class Input
    {
        Dictionary<Keys, bool> keyboardDefinedInputs = new Dictionary<Keys, bool>();
        Dictionary<Buttons, bool> gamepadDefinedInputs = new Dictionary<Buttons, bool>();
        Dictionary<Rectangle, bool> touchTapDefinedInputs = new Dictionary<Rectangle, bool>();
        Dictionary<Direction, float> touchSlideDefinedInputs = new Dictionary<Direction, float>();
        Dictionary<int, GestureDefinition> gestureDefinedInputs = new Dictionary<int, GestureDefinition>();
        Dictionary<Direction, float> accelerometerDefinedInputs = new Dictionary<Direction, float>();

        public static Dictionary<PlayerIndex, GamePadState> CurrentGamePadState = new Dictionary<PlayerIndex, GamePadState>();
        public static Dictionary<PlayerIndex, GamePadState> PreviousGamePadState = new Dictionary<PlayerIndex, GamePadState>();
        public static KeyboardState CurrentKeyboardState;
        public static KeyboardState PreviousKeyboardState;
        public static TouchCollection CurrentTouchLocationState;
        public static TouchCollection PreviousTouchLocationState;
        public static Dictionary<PlayerIndex, bool> GamepadConnectionState = new Dictionary<PlayerIndex, bool>();

        private static List<GestureDefinition> detectedGestures = new List<GestureDefinition>();
        private static Accelerometer accelerometerSensor;
        private static Vector3 currentAccelerometerReading;

        public enum Direction{
            Up,
            Down,
            Left,
            Right
        }

        public bool PinchGestureAvailable = false;
        private static bool isAccelerometerStarted = false;

        GestureDefinition currentGestureDefinition;

        public Input()
        {
            if (CurrentGamePadState.Count == 0)
            {
                CurrentGamePadState.Add(PlayerIndex.One, GamePad.GetState(PlayerIndex.One));
                CurrentGamePadState.Add(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two));
                CurrentGamePadState.Add(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three));
                CurrentGamePadState.Add(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four));

                PreviousGamePadState.Add(PlayerIndex.One, GamePad.GetState(PlayerIndex.One));
                PreviousGamePadState.Add(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two));
                PreviousGamePadState.Add(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three));
                PreviousGamePadState.Add(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four));

                GamepadConnectionState.Add(PlayerIndex.One, CurrentGamePadState[PlayerIndex.One].IsConnected);
                GamepadConnectionState.Add(PlayerIndex.Two, CurrentGamePadState[PlayerIndex.Two].IsConnected);
                GamepadConnectionState.Add(PlayerIndex.Three, CurrentGamePadState[PlayerIndex.Three].IsConnected);
                GamepadConnectionState.Add(PlayerIndex.Four, CurrentGamePadState[PlayerIndex.Four].IsConnected);
            }

            if (accelerometerSensor == null)
            {
                accelerometerSensor = new Accelerometer();
                accelerometerSensor.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(AccelerometerReadingChanged);
            }
        }

        public static void BeginUpdate()
        {
            CurrentGamePadState[PlayerIndex.One] = GamePad.GetState(PlayerIndex.One);
            CurrentGamePadState[PlayerIndex.Two] = GamePad.GetState(PlayerIndex.Two);
            CurrentGamePadState[PlayerIndex.Three] = GamePad.GetState(PlayerIndex.Three);
            CurrentGamePadState[PlayerIndex.Four] = GamePad.GetState(PlayerIndex.Four);

            CurrentKeyboardState = Keyboard.GetState(PlayerIndex.One);
            CurrentTouchLocationState = TouchPanel.GetState();

            detectedGestures.Clear();

            if (TouchPanel.EnabledGestures != GestureType.None)
            {
                while (TouchPanel.IsGestureAvailable)
                {
                    GestureSample gesture = TouchPanel.ReadGesture();
                    detectedGestures.Add(new GestureDefinition(gesture));
                }
            }
        }

        public static void EndUpdate()
        {
            PreviousGamePadState[PlayerIndex.One] = CurrentGamePadState[PlayerIndex.One];
            PreviousGamePadState[PlayerIndex.Two] = CurrentGamePadState[PlayerIndex.Two];
            PreviousGamePadState[PlayerIndex.Three] = CurrentGamePadState[PlayerIndex.Three];
            PreviousGamePadState[PlayerIndex.Four] = CurrentGamePadState[PlayerIndex.Four];

            PreviousKeyboardState = CurrentKeyboardState;
            PreviousTouchLocationState = CurrentTouchLocationState;
        }

        private void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            currentAccelerometerReading.X = (float)e.X;
            currentAccelerometerReading.Y = (float)e.Y;
            currentAccelerometerReading.Z = (float)e.Z;
        }

        public void AddGamepadInput(Buttons button, bool isReleasedPreviously)
        {
            if (gamepadDefinedInputs.ContainsKey(button))
            {
                gamepadDefinedInputs[button] = isReleasedPreviously;
                return;
            }
            gamepadDefinedInputs.Add(button, isReleasedPreviously);
        }

        public void AddKeyboardInput(Keys key, bool isReleasedPreviously)
        {
            if (keyboardDefinedInputs.ContainsKey(key))
            {
                keyboardDefinedInputs[key] = isReleasedPreviously;
                return;
            }
            keyboardDefinedInputs.Add(key, isReleasedPreviously);
        }

        public void AddTouchTapInput(Rectangle touchArea, bool isReleasedPreviously)
        {
            if (touchTapDefinedInputs.ContainsKey(touchArea))
            {
                touchTapDefinedInputs[touchArea] = isReleasedPreviously;
                return;
            }
            touchTapDefinedInputs.Add(touchArea, isReleasedPreviously);
        }

        public void AddTouchSlideInput(Direction direction, float slideDistance)
        {
            if (touchSlideDefinedInputs.ContainsKey(direction))
            {
                touchSlideDefinedInputs[direction] = slideDistance;
                return;
            }
            touchSlideDefinedInputs.Add(direction, slideDistance);
        }

        public void AddTouchGestureInput(GestureType gestureType, Rectangle touchArea)
        {
            TouchPanel.EnabledGestures = gestureType | TouchPanel.EnabledGestures;

            gestureDefinedInputs.Add(gestureDefinedInputs.Count, new GestureDefinition(gestureType, touchArea));

            if (gestureType == GestureType.Pinch)
            {
                PinchGestureAvailable = true;
            }
        }

        public void AddAccelerometerInput(Direction direction, float tiltThreshold)
        {
            if (!isAccelerometerStarted)
            {
                try
                {
                    accelerometerSensor.Start();
                    isAccelerometerStarted = true;
                }
                catch (AccelerometerFailedException)
                {
                    isAccelerometerStarted = false;
                }
            }

            accelerometerDefinedInputs.Add(direction, tiltThreshold);
        }

        public void RemoveAccelerometerInput()
        {
            if (isAccelerometerStarted)
            {
                try
                {
                    accelerometerSensor.Stop();
                    isAccelerometerStarted = false;
                }
                catch (AccelerometerFailedException)
                {
                    // Sensor couldn't be stopped
                }
            }

            accelerometerDefinedInputs.Clear();
        }

        public static bool IsConnected(PlayerIndex player)
        {
            return CurrentGamePadState[player].IsConnected;
        }

        public bool IsPressed(PlayerIndex player)
        {
            return IsPressed(player, null);
        }

        public bool IsPressed(PlayerIndex player, Rectangle? currentObjectLocation)
        {
            if (IsKeyboardInputPressed())
            {
                return true;
            }

            if (IsGamepadInputPressed(player))
            {
                return true;
            }

            if (IsTouchTapInputPressed())
            {
                return true;
            }

            if (IsTouchSlideInputPressed())
            {
                return true;
            }

            if (IsTouchGestureInputPressed(currentObjectLocation))
            {
                return true;
            }

            if (IsAccelerometerInputPressed())
            {
                return true;
            }

            return false;
        }

        private bool IsKeyboardInputPressed()
        {
            foreach (Keys key in keyboardDefinedInputs.Keys)
            {
                if (keyboardDefinedInputs[key]
                    && CurrentKeyboardState.IsKeyDown(key)
                    && !PreviousKeyboardState.IsKeyDown(key))
                {
                    return true;
                }
                else if (!keyboardDefinedInputs[key]
                         && CurrentKeyboardState.IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsGamepadInputPressed(PlayerIndex player)
        {
            foreach (Buttons button in gamepadDefinedInputs.Keys)
            {
                if (gamepadDefinedInputs[button]
                    && CurrentGamePadState[player].IsButtonDown(button)
                    && !PreviousGamePadState[player].IsButtonDown(button))
                {
                    return true;
                }
                else if (!gamepadDefinedInputs[button]
                         && CurrentGamePadState[player].IsButtonDown(button))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTouchTapInputPressed()
        {
            foreach (Rectangle touchArea in touchTapDefinedInputs.Keys)
            {
                if (touchTapDefinedInputs[touchArea]
                    && touchArea.Intersects(CurrentTouchRectangle)
                    && PreviousTouchPosition() == null)
                {
                    return true;
                }
                else if (!touchTapDefinedInputs[touchArea]
                         && touchArea.Intersects(CurrentTouchRectangle))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsTouchSlideInputPressed()
        {
            foreach (Direction slideDirection in touchSlideDefinedInputs.Keys)
            {
                if (CurrentTouchPosition() != null
                    && PreviousTouchPosition() != null)
                {
                    switch (slideDirection)
                    {
                        case Direction.Up:
                            if (CurrentTouchPosition().Value.Y
                                + touchSlideDefinedInputs[slideDirection] < PreviousTouchPosition().Value.Y)
                            {
                                return true;
                            }
                            break;
                        case Direction.Down:
                            if (CurrentTouchPosition().Value.Y
                                - touchSlideDefinedInputs[slideDirection] > PreviousTouchPosition().Value.Y)
                            {
                                return true;
                            }
                            break;
                        case Direction.Left:
                            if (CurrentTouchPosition().Value.X
                                + touchSlideDefinedInputs[slideDirection] < PreviousTouchPosition().Value.X)
                            {
                                return true;
                            }
                            break;
                        case Direction.Right:
                            if (CurrentTouchPosition().Value.X
                                - touchSlideDefinedInputs[slideDirection] > PreviousTouchPosition().Value.X)
                            {
                                return true;
                            }
                            break;
                    }
                }        
            }

            return false;
        }

        private bool IsTouchGestureInputPressed(Rectangle? newDetectionLocation)
        {
            // Clear the current gesture eacht to that there is always the most recent stored
            currentGestureDefinition = null;

            // If no gestures were detected, just exit
            if (detectedGestures.Count == 0)
            {
                return false;
            }

            // Check to see if any of the gestures have been fired
            foreach (GestureDefinition userDefinedGesture in gestureDefinedInputs.Values)
            {
                foreach (GestureDefinition detectedGesture in detectedGestures)
                {
                    if (detectedGesture.Type == userDefinedGesture.Type)
                    {
                        // If a rectangle area to check against has been bassed in, use that one.
                        // Otherwise, use the one the Input was originally set up with
                        Rectangle areaToCheck = userDefinedGesture.CollisionArea;

                        if (newDetectionLocation != null)
                        {
                            areaToCheck = newDetectionLocation.Value;
                        }

                        // If the gesture detected was made in the area where user was interested in Input,
                        // then a gesture input is considered detected
                        if (detectedGesture.CollisionArea.Intersects(areaToCheck))
                        {
                            if (currentGestureDefinition == null)
                            {
                                currentGestureDefinition = new GestureDefinition(detectedGesture.Gesture);
                            }
                            else
                            {
                                // Gestures like freeDrag or Flick are registered many times in a single update frame.
                                // Because we have only one variable stored, add all additional gesture values. So we
                                // have a composite of all the gesture information in currentGesture
                                currentGestureDefinition.Delta += detectedGesture.Delta;
                                currentGestureDefinition.Delta2 += detectedGesture.Delta2;
                                currentGestureDefinition.Position += detectedGesture.Position;
                                currentGestureDefinition.Position2 += detectedGesture.Position2;
                            }
                        }
                    }
                }
            }

            if (currentGestureDefinition != null)
            {
                return true;
            }

            return false;
        }

        private bool IsAccelerometerInputPressed()
        {
            foreach (KeyValuePair<Direction, float> input in accelerometerDefinedInputs)
            {
                switch (input.Key)
                {
                    case Direction.Up:
                        if (Math.Abs(currentAccelerometerReading.Y) > input.Value
                            && currentAccelerometerReading.Y < 0)
                        {
                            return true;
                        }
                        break;
                    case Direction.Down:
                        if (Math.Abs(currentAccelerometerReading.Y) > input.Value
                            && currentAccelerometerReading.Y > 0)
                        {
                            return true;
                        }
                        break;
                    case Direction.Left:
                        if (Math.Abs(currentAccelerometerReading.X) > input.Value
                            && currentAccelerometerReading.X < 0)
                        {
                            return true;
                        }
                        break;
                    case Direction.Right:
                        if (Math.Abs(currentAccelerometerReading.X) > input.Value
                            && currentAccelerometerReading.X > 0)
                        {
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        public Vector2 CurrentGesturePosition()
        {
            if (currentGestureDefinition == null)
            {
                return Vector2.Zero;
            }

            return currentGestureDefinition.Position;
        }

        public Vector2 CurrentGesturePosition2()
        {
            if (currentGestureDefinition == null)
            {
                return Vector2.Zero;
            }

            return currentGestureDefinition.Position2;
        }

        public Vector2 CurrentGestureDelta()
        {
            if (currentGestureDefinition == null)
            {
                return Vector2.Zero;
            }

            return currentGestureDefinition.Delta;
        }

        public Vector2 CurrentGestureDelta2()
        {
            if (currentGestureDefinition == null)
            {
                return Vector2.Zero;
            }

            return currentGestureDefinition.Delta2;
        }

        // Get the touch point for the current location. This doesn't use any
        // of the gesture information, but the actual touch point on the screen
        public Vector2? CurrentTouchPosition()
        {
            foreach (TouchLocation location in CurrentTouchLocationState)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                    case TouchLocationState.Moved:
                        return location.Position;
                }
            }

            return null;
        }

        public Vector2? PreviousTouchPosition()
        {
            foreach (TouchLocation location in PreviousTouchLocationState)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                    case TouchLocationState.Moved:
                        return location.Position;
                }
            }

            return null;
        }

        private Rectangle CurrentTouchRectangle
        {
            get
            {
                Vector2? touchPosition = CurrentTouchPosition();

                if (touchPosition == null)
                {
                    return Rectangle.Empty;
                }

                return new Rectangle((int)touchPosition.Value.X - 5,
                                     (int)touchPosition.Value.Y - 5,
                                     10,
                                     10);
            }
        }

        public Vector3 CurrentAccelerometerReading
        {
            get
            {
                return currentAccelerometerReading;
            }
        }
    }
}
