using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris
{
    public static class FallingBlock
    {
        public static Color[] defColors = new Color[]
        {
            Color.Cyan,
            Color.Blue,
            Color.Orange,
            Color.Yellow,
            Color.Lime,
            Color.Purple,
            Color.Red
        };

        public static Vector2 block1 = new Vector2(20, 20);
        public static Vector2 block2 = new Vector2(20, 20);
        public static Vector2 block3 = new Vector2(20, 20);
        public static Vector2 block4 = new Vector2(20, 20);

        public static Color blockColor = Color.NavajoWhite;

        private static Vector2 _origin;
        public static Random rnd = new Random();

        public static int nextBlock = 0;
        public static int currentBlock;
        public static int heldBlock;
        public static bool canHold = true;

        public static void NewBlock()
        {
            int type = nextBlock;
            while (nextBlock == type)
            {
                nextBlock = rnd.Next(1, 7);
            }

            currentBlock = type;
            canHold = true;
        
            switch (type)
            {
                case 1: //line | light blue
                    blockColor = Color.Cyan;
                    block1 = new Vector2(3, 1);
                    block2 = new Vector2(4, 1);
                    block3 = new Vector2(5, 1);
                    block4 = new Vector2(6, 1);
                    _origin = new Vector2(4.5f, 1.5f);
                    break;
                case 2: //reverse-L | blue
                    blockColor = Color.Blue;
                    block1 = new Vector2(3, 0);
                    block2 = new Vector2(3, 1);
                    block3 = new Vector2(4, 1);
                    block4 = new Vector2(5, 1);
                    _origin = new Vector2(4, 1);
                    break;
                case 3: //L | orange
                    blockColor = Color.Orange;
                    block1 = new Vector2(3, 1);
                    block2 = new Vector2(4, 1);
                    block3 = new Vector2(5, 1);
                    block4 = new Vector2(5, 0);
                    _origin = new Vector2(4, 1);
                    break;
                case 4://square | yellow
                    blockColor = Color.Yellow;
                    block1 = new Vector2(4, 0);
                    block2 = new Vector2(4, 1);
                    block3 = new Vector2(5, 0);
                    block4 = new Vector2(5, 1);
                    _origin = new Vector2(4.5f, 0.5f);
                    break;
                case 5://snake upwards | green
                    blockColor = Color.Lime;
                    block1 = new Vector2(3, 1);
                    block2 = new Vector2(4, 1);
                    block3 = new Vector2(4, 0);
                    block4 = new Vector2(5, 0);
                    _origin = new Vector2(4, 1);
                    break;
                case 6://T | purple
                    blockColor = Color.Magenta;
                    block1 = new Vector2(3, 1);
                    block2 = new Vector2(4, 0);
                    block3 = new Vector2(4, 1);
                    block4 = new Vector2(5, 1);
                    _origin = new Vector2(4, 1);
                    break;
                case 7://snake downwards | red
                    blockColor = Color.Red;
                    block1 = new Vector2(3, 0);
                    block2 = new Vector2(4, 0);
                    block3 = new Vector2(4, 1);
                    block4 = new Vector2(5, 1);
                    _origin = new Vector2(4, 1);
                    break;
                default://this shouldn't happen
                    System.Diagnostics.Debug.WriteLine("ERROR: INVALID NEWBLOCK NUMBER");
                    break;
            }
        }

        public static void PushDown()
        {
            block1.Y += 1;
            block2.Y += 1;
            block3.Y += 1;
            block4.Y += 1;
            _origin.Y += 1;
        }

        public static void PushRight()
        {
            block1.X += 1;
            block2.X += 1;
            block3.X += 1;
            block4.X += 1;
            _origin.X += 1;
        }

        public static void PushLeft()
        {
            block1.X -= 1;
            block2.X -= 1;
            block3.X -= 1;
            block4.X -= 1;
            _origin.X -= 1;
        }

        public static void Drop(int y)
        {
            int lowY = (int)Math.Max(block1.Y, Math.Max(block2.Y, Math.Max(block3.Y, block4.Y)));

            int dropBy = y - lowY;

            block1.Y += dropBy;
            block2.Y += dropBy;
            block3.Y += dropBy;
            block4.Y += dropBy;
            _origin.Y += dropBy;

        }

        public static void Jump()
        {
            block1.Y -= 1;
            block2.Y -= 1;
            block3.Y -= 1;
            block4.Y -= 1;
        }

        public static void Rotate(int degree)
        {
            float radians = (float)(Math.PI / 180 * degree);

            block1 = Vector2.Transform(block1 - _origin, Matrix.CreateRotationZ(radians)) + _origin;
            block2 = Vector2.Transform(block2 - _origin, Matrix.CreateRotationZ(radians)) + _origin;
            block3 = Vector2.Transform(block3 - _origin, Matrix.CreateRotationZ(radians)) + _origin;
            block4 = Vector2.Transform(block4 - _origin, Matrix.CreateRotationZ(radians)) + _origin;
            FixLR();

        }

        public static void FixLR()
        {
            while ((int)Math.Round(block1.X) > 9
                || (int)Math.Round(block2.X) > 9
                || (int)Math.Round(block3.X) > 9
                || (int)Math.Round(block4.X) > 9)
            {
                PushLeft();
            }

            while ((int)Math.Round(block1.X) < 0
                || (int)Math.Round(block2.X) < 0
                || (int)Math.Round(block3.X) < 0
                || (int)Math.Round(block4.X) < 0)
            {
                PushRight();
            }
        }
        public static void FixY()
        {
            while ((int)Math.Round(block1.Y) > 17
                || (int)Math.Round(block2.Y) > 17
                || (int)Math.Round(block3.Y) > 17
                || (int)Math.Round(block4.Y) > 17)
            {
                Jump();
            }
        }

        public static void HoldBlock()
        {
            if (canHold)
            {
                if (heldBlock == 0)
                {
                    heldBlock = currentBlock;
                    NewBlock();
                }
                else
                {
                    int nextBlockSave = nextBlock;
                    nextBlock = heldBlock;
                    heldBlock = currentBlock;
                    NewBlock();
                    nextBlock = nextBlockSave;
                }
                canHold = false;
            }
        }

    }
}
