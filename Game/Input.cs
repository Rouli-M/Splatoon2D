﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Splatoon2D
{
    static class Input
    {
        public static float movement_direction; // float from -1.0 to 1.0
        public static bool GamepadUsed = false;
        public static int aim_direction;
        public static bool Jump, Shoot, Squid;
        public static float Angle = 0f; // between zero and 2 * Math.PI
        public static KeyboardState ks, old_ks;
        public static MouseState ms, old_ms;

        public static void Update(Player player)
        {
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            ks = Keyboard.GetState();
            ms = Mouse.GetState();

            if (old_ks == null) old_ks = ks;
            if (old_ms == null) old_ms = ms;
            
            // If there a controller attached, handle it
            if (capabilities.IsConnected)
            {
                GamepadUsed = true;
                GamePadState gp = GamePad.GetState(PlayerIndex.One);
                Jump = gp.IsButtonDown(Buttons.A);
                Shoot = gp.Triggers.Right > 0.5;
                Squid = gp.Triggers.Left > 0.5;
                movement_direction = gp.ThumbSticks.Left.X;

                if (gp.ThumbSticks.Right.X != 0 || gp.ThumbSticks.Right.Y != 0)
                {
                    Angle = (float)Math.Atan2(gp.ThumbSticks.Right.Y, gp.ThumbSticks.Right.X);
                    //aim_direction = Math.Sign(gp.ThumbSticks.Right.X);
                }
                else if (gp.ThumbSticks.Left.X != 0)
                {
                    //aim_direction = Math.Sign(gp.ThumbSticks.Right.X);
                    if (gp.ThumbSticks.Left.X > 0) Angle = 0;
                    else Angle = (float)Math.PI;
                }
            }
            else
            {
                GamepadUsed = false;
                Jump = ks.IsKeyDown(Keys.Z) || ks.IsKeyDown(Keys.W) || ks.IsKeyDown(Keys.Up);
                Shoot = ms.LeftButton == ButtonState.Pressed;
                Squid = ks.IsKeyDown(Keys.LeftShift);

                if (LeftPressed(ks) && RightPressed(ks) || !LeftPressed(ks) && !RightPressed(ks)) movement_direction = 0;
                else if (LeftPressed(ks)) movement_direction = -1;
                else if (RightPressed(ks)) movement_direction = 1;

                Vector2 ScreenMousePosition = ms.Position.ToVector2() * 1 / Camera.Zoom + Camera.TopLeftCameraPosition; // dark magic stuff

                Vector2 PlayerToMouse = ScreenMousePosition - player.FeetPosition - player.GetArmRelativePoint();
                Angle = (float)Math.Atan2(-PlayerToMouse.Y, PlayerToMouse.X);
            }

            Angle = (float)(Angle % (Math.PI * 2));
            aim_direction = Math.Sign((float)Math.Cos(Angle));
        }

        private static bool LeftPressed(KeyboardState ks)
        {
            return (ks.IsKeyDown(Keys.Q) || ks.IsKeyDown(Keys.A) || ks.IsKeyDown(Keys.Left));
        }

        private static bool RightPressed(KeyboardState ks)
        {
            return (ks.IsKeyDown(Keys.D) || ks.IsKeyDown(Keys.Right));
        }
    }
}
