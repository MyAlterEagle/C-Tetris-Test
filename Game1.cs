using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Tetris
{
    public class Game1 : Game
    {
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _backgroundTexture;
        private Vector2 _backgroundPosition;

        private Texture2D _cover;
        private Texture2D[] _smallBlocks = new Texture2D[7];

        private Texture2D _blockTexture;

        private bool[,] _blockArray = new bool[10, 18];
        private Color[,] _colorArray = new Color[10, 18];

        private float _timer;
        private int _lines = 0;
        private int _score = 0;

        private Color _heldColor;
        
        private bool _running;
        private bool _blocksToBreak;
        private bool _firstTry = true;

        private KeyboardState _currentKey;
        private KeyboardState _previousKey;

        private SpriteFont _font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 843;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Window.Title = "Tetris";
            _backgroundPosition = new Vector2(0, 0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("843x900background");
            _blockTexture = Content.Load<Texture2D>("50x50block");
            _cover = Content.Load<Texture2D>("83x16cover");
            for (int i = 0; i < 7; i++)
            {
                _smallBlocks[i] = Content.Load<Texture2D>("50x50block" +(i+1));
            }                                                                                       
            _font = Content.Load<SpriteFont>("font");
        }

        protected override void Update(GameTime gameTime)
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();
            if (_currentKey.IsKeyDown(Keys.R) && _previousKey.IsKeyUp(Keys.R))
            {
                Restart();
            }
            if (!_running)
            {
                return;
            }

            _timer +=                                                                                                                                                                                                                                                                                                                                                                                                                                                                         (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_blocksToBreak)
            {
                if (_timer > 0.1)
                {
                    for (int y = 0; y < 18; y++)
                    {
                        bool push = false;
                        for (int x = 0; x < 10; x++)
                        {
                            if (_colorArray[x, y] == Color.DarkGray)
                            {
                                _blockArray[x, y] = false;
                                _colorArray[x, y] = Color.Black;
                                push = true;
                            }
                        }
                        if (push)
                        {
                            _score += _lines * 20;
                            _lines++;
                            for (int yP = y; yP >= 0; yP--)
                            {
                                for (int xP = 0; xP < 10; xP++)
                                {
                                    try
                                    {
                                        _blockArray[xP, yP] = _blockArray[xP, yP - 1];
                                        _colorArray[xP, yP] = _colorArray[xP, yP - 1];
                                    }
                                    catch
                                    {

                                    }

                                }
                            }
                        }
                    }
                    _blocksToBreak = false;
                    FallingBlock.NewBlock();
                }
                else
                {
                    return;
                }
            }

            try
            {
                blockUnstucker();
                blockPlacer();
            }
            catch
            {
                _firstTry = false;
                _running = false;
            }
            try
            {
                runControls();
            }
            catch
            {
            }

            float difficulty = 2.5f - _lines / 5;
            if (difficulty < 0.25f)
            {
                difficulty = 0.5f;
            }

            if (_timer > difficulty)
            {
                FallingBlock.PushDown();
                _timer = 0;
            }
            

            base.Update(gameTime);
        }

        private void blockPlacer()
        {
            if ((int)Math.Max(FallingBlock.block1.Y, Math.Max(FallingBlock.block2.Y, Math.Max(FallingBlock.block3.Y, FallingBlock.block4.Y))) == 17
                            || _blockArray[(int)Math.Round(FallingBlock.block1.X), (int)Math.Round(FallingBlock.block1.Y) + 1]
                            || _blockArray[(int)Math.Round(FallingBlock.block2.X), (int)Math.Round(FallingBlock.block2.Y) + 1]
                            || _blockArray[(int)Math.Round(FallingBlock.block3.X), (int)Math.Round(FallingBlock.block3.Y) + 1]
                            || _blockArray[(int)Math.Round(FallingBlock.block4.X), (int)Math.Round(FallingBlock.block4.Y) + 1])
            {
                Color darkColor = new Color(FallingBlock.blockColor.R - 60, FallingBlock.blockColor.G - 60, FallingBlock.blockColor.B - 60);

                _blockArray[(int)Math.Round(FallingBlock.block1.X), (int)Math.Round(FallingBlock.block1.Y)] = true;
                _blockArray[(int)Math.Round(FallingBlock.block2.X), (int)Math.Round(FallingBlock.block2.Y)] = true;
                _blockArray[(int)Math.Round(FallingBlock.block3.X), (int)Math.Round(FallingBlock.block3.Y)] = true;
                _blockArray[(int)Math.Round(FallingBlock.block4.X), (int)Math.Round(FallingBlock.block4.Y)] = true;
                _colorArray[(int)Math.Round(FallingBlock.block1.X), (int)Math.Round(FallingBlock.block1.Y)] = darkColor;
                _colorArray[(int)Math.Round(FallingBlock.block2.X), (int)Math.Round(FallingBlock.block2.Y)] = darkColor;
                _colorArray[(int)Math.Round(FallingBlock.block3.X), (int)Math.Round(FallingBlock.block3.Y)] = darkColor;
                _colorArray[(int)Math.Round(FallingBlock.block4.X), (int)Math.Round(FallingBlock.block4.Y)] = darkColor;

                bool anyLine = false;
                for (int y = 0; y < 18; y++)
                {
                    bool line = true;
                    for (int x = 0; x < 10; x++)
                    {
                        if (!_blockArray[x, y])
                        {
                            line = false;
                            break;
                        }
                    }
                    if (line)
                    {
                        anyLine = true;
                        for (int x = 0; x < 10; x++)
                        {
                            _colorArray[x, y] = Color.DarkGray;
                        }
                        _timer = 0;
                        _blocksToBreak = true;
                    }
                }
                if (!anyLine)
                {
                    FallingBlock.NewBlock();
                }
                
            }

            
        }

        private void blockUnstucker()
        {
            while (_blockArray[(int)Math.Round(FallingBlock.block1.X), (int)Math.Round(FallingBlock.block1.Y)]
                || _blockArray[(int)Math.Round(FallingBlock.block2.X), (int)Math.Round(FallingBlock.block2.Y)]
                || _blockArray[(int)Math.Round(FallingBlock.block3.X), (int)Math.Round(FallingBlock.block3.Y)]
                || _blockArray[(int)Math.Round(FallingBlock.block4.X), (int)Math.Round(FallingBlock.block4.Y)])
            {
                FallingBlock.Jump();
            }
        }

        private void Restart()
        {
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    _blockArray[x, y] = false;
                }
            }
            _lines = 0;
            _score = 0;
            _running = true;
            FallingBlock.heldBlock = 0;
            FallingBlock.nextBlock = FallingBlock.rnd.Next(1, 7);
            FallingBlock.NewBlock();
        }

        private void runControls()
        {
            if (_currentKey.IsKeyDown(Keys.Right) && _previousKey.IsKeyUp(Keys.Right)
            && !_blockArray[(int)Math.Round(FallingBlock.block1.X) + 1, (int)Math.Round(FallingBlock.block1.Y)]
            && !_blockArray[(int)Math.Round(FallingBlock.block2.X) + 1, (int)Math.Round(FallingBlock.block2.Y)]
            && !_blockArray[(int)Math.Round(FallingBlock.block3.X) + 1, (int)Math.Round(FallingBlock.block3.Y)]
            && !_blockArray[(int)Math.Round(FallingBlock.block4.X) + 1, (int)Math.Round(FallingBlock.block4.Y)])
            {
                FallingBlock.PushRight();
                FallingBlock.FixLR();
            }
            else if (_currentKey.IsKeyDown(Keys.Left) && _previousKey.IsKeyUp(Keys.Left)
            && !_blockArray[(int)Math.Round(FallingBlock.block1.X) - 1, (int)Math.Round(FallingBlock.block1.Y)]
            && !_blockArray[(int)Math.Round(FallingBlock.block2.X) - 1, (int)Math.Round(FallingBlock.block2.Y)]
            && !_blockArray[(int)Math.Round(FallingBlock.block3.X) - 1, (int)Math.Round(FallingBlock.block3.Y)]
            && !_blockArray[(int)Math.Round(FallingBlock.block4.X) - 1, (int)Math.Round(FallingBlock.block4.Y)])
            {
                FallingBlock.PushLeft();
                FallingBlock.FixLR();
            }

            if (_currentKey.IsKeyDown(Keys.Up) && _previousKey.IsKeyUp(Keys.Up))
            {
                FallingBlock.Rotate(90);
                FallingBlock.FixY();
            }
            if (_currentKey.IsKeyDown(Keys.Z) && _previousKey.IsKeyUp(Keys.Z))
            {
                FallingBlock.Rotate(-90);
                FallingBlock.FixY();
            }
            if (_currentKey.IsKeyDown(Keys.Down) && _previousKey.IsKeyUp(Keys.Down))
            {
                FallingBlock.PushDown();
                _score += 10;
            }
            if (_currentKey.IsKeyDown(Keys.Space) && _previousKey.IsKeyUp(Keys.Space))
            {
                int lowPoint = (int)Math.Round(Math.Max(FallingBlock.block1.Y, Math.Max(FallingBlock.block2.Y, Math.Max(FallingBlock.block3.Y, FallingBlock.block4.Y))));
                for (int i = lowPoint; i < 18; i++)
                {
                    if (i == 17)
                    {
                        _score += 20*(17-lowPoint);
                        FallingBlock.Drop(17);
                    }
                    else if (_blockArray[(int)Math.Round(FallingBlock.block1.X), i + 1 - (lowPoint-(int)FallingBlock.block1.Y)]
                        || _blockArray[(int)Math.Round(FallingBlock.block2.X), i + 1 - (lowPoint - (int)FallingBlock.block2.Y)]
                        || _blockArray[(int)Math.Round(FallingBlock.block3.X), i + 1 - (lowPoint - (int)FallingBlock.block3.Y)]
                        || _blockArray[(int)Math.Round(FallingBlock.block4.X), i + 1 - (lowPoint - (int)FallingBlock.block4.Y)])
                    {
                        FallingBlock.Drop(i);
                        _score += 20 * (i - lowPoint);
                        break;
                    }

                }
                FallingBlock.FixY();
            }
            if (_currentKey.IsKeyDown(Keys.C) && _previousKey.IsKeyUp(Keys.C))
            {
                _heldColor = FallingBlock.blockColor;
                FallingBlock.HoldBlock();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();
            _spriteBatch.Draw(_backgroundTexture, _backgroundPosition, Color.White);

            if (_running || _firstTry)
            {
                _spriteBatch.Draw(_cover, new Vector2(521, 356), Color.White);
            }

            if (FallingBlock.heldBlock != 0)
            {
                _spriteBatch.Draw(_smallBlocks[FallingBlock.heldBlock-1], new Vector2(716, 66), FallingBlock.defColors[FallingBlock.heldBlock-1]);
            }
            if (FallingBlock.nextBlock != 0)
            {
                _spriteBatch.Draw(_smallBlocks[FallingBlock.nextBlock - 1], new Vector2(548, 66), FallingBlock.defColors[FallingBlock.nextBlock - 1]);
            }

            _spriteBatch.DrawString(_font, _score.ToString(), new Vector2(573, 119), Color.Black);
            _spriteBatch.DrawString(_font, _lines.ToString(), new Vector2(573, 139), Color.Black);

            _spriteBatch.Draw(_blockTexture, Vector2.Multiply(FallingBlock.block1, 50), FallingBlock.blockColor);
            _spriteBatch.Draw(_blockTexture, Vector2.Multiply(FallingBlock.block2, 50), FallingBlock.blockColor);
            _spriteBatch.Draw(_blockTexture, Vector2.Multiply(FallingBlock.block3, 50), FallingBlock.blockColor);
            _spriteBatch.Draw(_blockTexture, Vector2.Multiply(FallingBlock.block4, 50), FallingBlock.blockColor);

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 18; y++)
                {
                    if (_blockArray[x, y])
                    {
                        _spriteBatch.Draw(_blockTexture, new Vector2(x * 50, y * 50), _colorArray[x, y]);
                    }
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
