using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class PlayerManager
    {
        public Sprite powerUp;
        public Sprite playerSprite;
        private float playerSpeed = 160.0f;
        private Rectangle playerAreaLimit;

        public long PlayerScore = 0;
        public int LivesRemaining = 5;
        public bool Destroyed = false;

        private Vector2 gunOffset = new Vector2(25, 10);
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.1f;
        public int playerRadius = 15;
        public ShotManager PlayerShotManager;

        public double shieldTimer = 0;
        public double shieldTimerMax = 10.0;   // Max shield time is 10 seconds
        private Random rand = new Random(System.Environment.TickCount);

        public bool ShieldsUp = false;

        public PlayerManager(
            Texture2D texture,  
            Rectangle initialFrame,
            int frameCount,
            Rectangle screenBounds)
        {
            playerSprite = new Sprite(
                new Vector2(500, 500),
                texture,
                initialFrame,
                Vector2.Zero);

            powerUp = new Sprite(
                new Vector2(400, 0),
                texture,
                new Rectangle(202,54,286,106 ),
                new Vector2(0, 40));

            PlayerShotManager = new ShotManager(
                texture,
                new Rectangle(0, 300, 5, 5),
                4,
                2,
                250f,
                screenBounds);

            playerAreaLimit =
                new Rectangle(
                    0,
                    screenBounds.Height / 2,
                    screenBounds.Width,
                    screenBounds.Height / 2);

            for (int x = 1; x < frameCount; x++)
            {
                playerSprite.AddFrame(
                    new Rectangle(
                        initialFrame.X + (initialFrame.Width * x),
                        initialFrame.Y,
                        initialFrame.Width,
                        initialFrame.Height));
            }
            playerSprite.CollisionRadius = playerRadius;
        }

        private void FireShot()
        {
            if (shotTimer >= minShotTimer)
            {
                PlayerShotManager.FireShot(
                    playerSprite.Location + gunOffset,
                    new Vector2(0, -1),
                    true);
                shotTimer = 0.0f;
            }
        }

        private void HandleKeyboardInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                playerSprite.Velocity += new Vector2(0, -1);
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                playerSprite.Velocity += new Vector2(0, 1);
            }

            if (keyState.IsKeyDown(Keys.Left))
            {
                playerSprite.Velocity += new Vector2(-1, 0);
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                playerSprite.Velocity += new Vector2(1, 0);
            }

            if (keyState.IsKeyDown(Keys.Space))
            {
                FireShot();
            }
        }

        private void HandleGamepadInput(GamePadState gamePadState)
        {
            playerSprite.Velocity +=
                new Vector2(
                    gamePadState.ThumbSticks.Left.X,
                    -gamePadState.ThumbSticks.Left.Y);

            if (gamePadState.Buttons.A == ButtonState.Pressed)
            {
                FireShot();
            }
        }

        private void imposeMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X)
                location.X = playerAreaLimit.X;

            if (location.X >
                (playerAreaLimit.Right - playerSprite.Source.Width))
                location.X =
                    (playerAreaLimit.Right - playerSprite.Source.Width);

            if (location.Y < playerAreaLimit.Y)
                location.Y = playerAreaLimit.Y;

            if (location.Y >
                (playerAreaLimit.Bottom - playerSprite.Source.Height))
                location.Y =
                    (playerAreaLimit.Bottom - playerSprite.Source.Height);

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            PlayerShotManager.Update(gameTime);
            powerUp.Update(gameTime);

            if (!Destroyed)
            {
                if (ShieldsUp)
                {
                    shieldTimer += gameTime.ElapsedGameTime.TotalSeconds;

                    if (shieldTimer >= 10)
                    {
                        ShieldsUp = false;
                        playerSprite.CollisionRadius = playerRadius;
                        shieldTimer = 0;
                    }
                }
                playerSprite.Velocity = Vector2.Zero;

                shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                HandleKeyboardInput(Keyboard.GetState());
                HandleGamepadInput(GamePad.GetState(PlayerIndex.One));

                playerSprite.Velocity.Normalize();
                playerSprite.Velocity *= playerSpeed;

                playerSprite.Update(gameTime);

                if (playerSprite.IsBoxColliding(powerUp.BoundingBoxRect))
                {
                    ShieldsUp = true;
                    playerSprite.CollisionRadius = playerRadius * 3;
                    powerUp.Location = new Vector2(rand.Next(40, 720), -400);
                }

                imposeMovementLimits();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);
            powerUp.Draw(spriteBatch);

            if (!Destroyed)
            {
                playerSprite.Draw(spriteBatch);
            }
        }

    }
}
