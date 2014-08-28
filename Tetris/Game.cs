using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using SFML.Graphics;
using SFML.Window;

namespace Tetris
{
    internal class Game
    {
        private const int WindowWidth = (GridWidth * BlockWidth) + GridOffsetLeft + (8 * BlockWidth);
        private const int WindowHeight = (GridHeight * BlockHeigth) + GridOffsetTop;
        
        private const int GridOffsetLeft = BlockWidth * 2;
        private const int GridOffsetTop = 0;

        private const int GridWidth = 10;
        private const int GridHeight = 16;

        private const int BlockWidth = 32;
        private const int BlockHeigth = 32;

        private readonly RenderWindow _window;
        private readonly Grid<int> _grid;

        private readonly Sprite[] _blockSprites;
        private readonly Sprite[] _layoutSprites;
        private readonly Sprite[] _numberSprites;
        private readonly Dictionary<char,Sprite> _letterSprites;

        private Tetromino _tetromino;
        private Tetromino _nextTetromino;

        private int _level;
        private int _score;
        private int _lines;

        private float _secondsSinceMoveDown;

        private bool _isMovingLeft;
        private bool _isMovingRight;
        private bool _isMovingDown;
        private bool _isRotating;

        public Game()
        {
            _window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), "бумага Тетрис");
            _window.Closed += (s, e) => _window.Close();
            _window.KeyPressed += WindowOnKeyPressed;

            _window.SetFramerateLimit(60);

            var icon = new Image("Assets/Icon.png");
            _window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);

            _grid = new Grid<int>(GridWidth, GridHeight);

            _tetromino = Tetromino.NewRandom();
            _nextTetromino = Tetromino.NewRandom();

            _blockSprites = new[]
                {
                    new Sprite(new Texture("Assets/Blocks/Box_32x32.png")),
                    new Sprite(new Texture("Assets/Blocks/I_32x32.png")),
                    new Sprite(new Texture("Assets/Blocks/J_32x32.png")), 
                    new Sprite(new Texture("Assets/Blocks/L_32x32.png")), 
                    new Sprite(new Texture("Assets/Blocks/O_32x32.png")), 
                    new Sprite(new Texture("Assets/Blocks/S_32x32.png")), 
                    new Sprite(new Texture("Assets/Blocks/T_32x32.png")), 
                    new Sprite(new Texture("Assets/Blocks/Z_32x32.png")) 
                };

            _numberSprites = new[]
                {
                    new Sprite(new Texture("Assets/Numbers/0_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/1_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/2_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/3_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/4_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/5_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/6_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/7_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/8_32x32.png")),
                    new Sprite(new Texture("Assets/Numbers/9_32x32.png"))
                };

            _layoutSprites = new[]
                {
                    new Sprite(new Texture("Assets/Layout/Black_32x32.png")),
                    new Sprite(new Texture("Assets/Layout/Bricks_32x32.png"))
                };

            _letterSprites = new Dictionary<char, Sprite>
                {
                    { 'S', new Sprite(new Texture("Assets/Letters/S_32x32.png")) },
                    { 'C', new Sprite(new Texture("Assets/Letters/C_32x32.png")) },
                    { 'O', new Sprite(new Texture("Assets/Letters/O_32x32.png")) },
                    { 'R', new Sprite(new Texture("Assets/Letters/R_32x32.png")) },
                    { 'E', new Sprite(new Texture("Assets/Letters/E_32x32.png")) },
                    { 'L', new Sprite(new Texture("Assets/Letters/L_32x32.png")) },
                    { 'I', new Sprite(new Texture("Assets/Letters/I_32x32.png")) },
                    { 'N', new Sprite(new Texture("Assets/Letters/N_32x32.png")) },
                    { 'V', new Sprite(new Texture("Assets/Letters/V_32x32.png")) },
                };
        }

        public void Run()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (_window.IsOpen())
            {
                TimeSpan elapsed = stopwatch.Elapsed;
                stopwatch.Restart();

                _window.DispatchEvents();

                Update((float)elapsed.TotalSeconds);

                Draw();
            }
        }

        private void WindowOnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Code == Keyboard.Key.Left)
            {
                _isMovingLeft = true;
            }

            if (keyEventArgs.Code == Keyboard.Key.Right)
            {
                _isMovingRight = true;
            }

            if (keyEventArgs.Code == Keyboard.Key.Down)
            {
                _isMovingDown = true;
            }

            if (keyEventArgs.Code == Keyboard.Key.Up)
            {
                _isRotating = true;
            }
        }

        private void Update(float secondsElapsed)
        {
            // Handle movement
            var movement = new Vector2i();
            if (_isMovingLeft)
            {
                _isMovingLeft = false;
                movement.X -= 1;
            }

            if (_isMovingRight)
            {
                _isMovingRight = false;
                movement.X += 1;
            }

            if (_isMovingDown)
            {
                _isMovingDown = false;
                movement.Y += 1;
            }

            if (_isRotating)
            {
                _isRotating = false;
                _tetromino.PotentialRotation++;
            }

            _tetromino.PotentialTopLeft += movement;

            // Handle falling down
            _secondsSinceMoveDown += secondsElapsed;
            if (_secondsSinceMoveDown >= 0.5f)
            {
                _secondsSinceMoveDown -= 0.5f;

                _tetromino.PotentialTopLeft = _tetromino.TopLeft + new Vector2i(0, 1);
            }

            // Check rotation
            bool allowRotation = true;
            for (int x = 0; x < _tetromino.PotentialWidth; x++)
            {
                for (int y = 0; y < _tetromino.PotentialHeight; y++)
                {
                    if (_tetromino.PotentialBlocks[x, y] != 0)
                    {
                        if (_tetromino.PotentialTopLeft.X + x < 0 || _tetromino.PotentialTopLeft.Y + y < 0
                            || _tetromino.PotentialTopLeft.X + x >= GridWidth || _tetromino.PotentialTopLeft.Y + y >= GridHeight
                            || _grid[_tetromino.PotentialTopLeft.X + x, _tetromino.PotentialTopLeft.Y + y] != 0)
                        {
                            allowRotation = false;
                        }
                    }
                }
            }

            if (allowRotation)
            {
                _tetromino.Rotation = _tetromino.PotentialRotation;
            }
            else
            {
                Console.WriteLine("Prevent rotation");
                _tetromino.PotentialRotation = _tetromino.Rotation;
            }

            bool tetrominoHasLanded = false;
            for (int x = 0; x < _tetromino.Width; x++)
            {
                for (int y = 0; y < _tetromino.Height; y++)
                {
                    if (_tetromino.Blocks[x, y] != 0)
                    {
                        if (_tetromino.PotentialTopLeft.Y + y >= GridHeight ||
                            _grid[_tetromino.TopLeft.X + x, _tetromino.PotentialTopLeft.Y + y] != 0)
                        {
                            Console.WriteLine("landed");
                            tetrominoHasLanded = true;
                        }

                        if (!tetrominoHasLanded)
                        {
                            if (_tetromino.PotentialTopLeft.X + x < 0 || _tetromino.PotentialTopLeft.X + x >= GridWidth
                                || _grid[_tetromino.PotentialTopLeft.X + x, _tetromino.PotentialTopLeft.Y + y] != 0)
                            {
                                Console.WriteLine("Prevent horizontal move");

                                // Reset X
                                _tetromino.PotentialTopLeft = new Vector2i(
                                    _tetromino.TopLeft.X, _tetromino.PotentialTopLeft.Y);
                            }
                        }
                    }
                }
            }

            if (tetrominoHasLanded)
            {
                // Copy tetromino blocks to grid
                for (int x = 0; x < _tetromino.Width; x++)
                {
                    for (int y = 0; y < _tetromino.Height; y++)
                    {
                        if (_tetromino.Blocks[x, y] != 0)
                        {
                            _grid[_tetromino.TopLeft.X + x, _tetromino.TopLeft.Y + y] = _tetromino.Blocks[x, y];
                        }
                    }
                }

                // Check for completed rows
                int completedRowCount = 0;
                for (int y = 0; y < GridHeight; y++)
                {
                    bool rowIsComplete = true;

                    for (int x = 0; x < GridWidth; x++)
                    {
                        rowIsComplete &= _grid[x, y] != 0;
                    }

                    if (rowIsComplete)
                    {
                        completedRowCount++;

                        Console.WriteLine("row completed: {0}", y);

                        // Move each row down
                        for (int yy = y; yy >= 0; yy--)
                        {
                            for (int xx = 0; xx < GridWidth; xx++)
                            {
                                _grid[xx, yy] = yy - 1 >= 0 ? _grid[xx, yy - 1] : 0;
                            }
                        }
                    }
                }

                if (completedRowCount > 0)
                {
                    _lines += completedRowCount;

                    // Calculate score
                    int pointFactor;
                    switch (completedRowCount)
                    {
                        case 1:
                            pointFactor = 40;
                            break;
                        case 2:
                            pointFactor = 100;
                            break;
                        case 3:
                            pointFactor = 300;
                            break;
                        case 4:
                            pointFactor = 1200;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _score += pointFactor * (_level + 1);

                    // Check level progression
                    
                }

                _tetromino = _nextTetromino;
                _nextTetromino = Tetromino.NewRandom();
            }
            else
            {
                _tetromino.TopLeft = _tetromino.PotentialTopLeft;
            }
        }

        private void Draw()
        {
            _window.Clear();

            // Fill background with black layout block
            for (int x = 0; x < WindowWidth / BlockWidth; x++)
            {
                for (int y = 0; y < WindowHeight / BlockHeigth; y++)
                {
                    Sprite layoutSprite = _layoutSprites[0];
                    layoutSprite.Position = new Vector2f(x * BlockWidth, y * BlockHeigth);

                    _window.Draw(layoutSprite);
                }
            }

            // Draw two brick lines at the side of the grid
            for (int y = 0; y < WindowHeight / BlockHeigth; y++)
            {
                Sprite brickSprite = _layoutSprites[1];

                brickSprite.Position = new Vector2f(BlockWidth, y * BlockHeigth);
                _window.Draw(brickSprite);

                brickSprite.Position = new Vector2f(GridOffsetLeft + (GridWidth * BlockWidth), y * BlockHeigth);
                _window.Draw(brickSprite);
            }

            // Draw the controls at the right side of the screen
            var controlPosition = new Vector2f(GridOffsetLeft + (GridWidth * BlockWidth) + (BlockWidth * 2), 0);

            DrawText("SCORE", controlPosition + new Vector2f(0, BlockHeigth * 1));
            DrawNumber(_score, controlPosition + new Vector2f(0, BlockHeigth * 2));

            DrawText("LEVEL", controlPosition + new Vector2f(0, BlockHeigth * 4));
            DrawNumber(_level, controlPosition + new Vector2f(0, BlockHeigth * 5));

            DrawText("LINES", controlPosition + new Vector2f(0, BlockHeigth * 7));
            DrawNumber(_lines, controlPosition + new Vector2f(0, BlockHeigth * 8));

            // Draw a box that contains the next tetromino
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Sprite emptySprite = _blockSprites[0];
                    emptySprite.Position = controlPosition + new Vector2f((BlockWidth * x), (BlockHeigth * 10) + (BlockHeigth * y));

                    _window.Draw(emptySprite);
                }
            }

            // Draw the next tetromino
            for (int x = 0; x < _nextTetromino.Width; x++)
            {
                for (int y = 0; y < _nextTetromino.Height; y++)
                {
                    Sprite blockSprite = _blockSprites[_nextTetromino.Blocks[x, y]];
                    blockSprite.Position = controlPosition + new Vector2f((BlockWidth * 1) + (x * BlockWidth), (BlockHeigth * 11) + (y * BlockHeigth));

                    _window.Draw(blockSprite);
                }
            }

            // Draw the grid
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    Sprite blockSprite = _blockSprites[_grid[x, y]];
                    blockSprite.Position = new Vector2f(
                        (x * BlockWidth) + GridOffsetLeft, (y * BlockHeigth) + GridOffsetTop);

                    _window.Draw(blockSprite);
                }
            }

            // Draw the active tetromino
            for (int x = 0; x < _tetromino.Width; x++)
            {
                for (int y = 0; y < _tetromino.Height; y++)
                {
                    int code = _tetromino.Blocks[x, y];
                    if (code != 0)
                    {
                        Sprite blockSprite = _blockSprites[code];
                        blockSprite.Position = new Vector2f(
                            ((_tetromino.TopLeft.X + x) * BlockWidth) + GridOffsetLeft, ((_tetromino.TopLeft.Y + y) * BlockHeigth) + GridOffsetTop);

                        _window.Draw(blockSprite);
                    }
                }
            }

            _window.Display();
        }

        private void DrawNumber(int number, Vector2f position)
        {
            const int FieldSize = 5;

            string str = number.ToString(CultureInfo.InvariantCulture);
            if (str.Length < FieldSize)
            {
                for (int i = 0; i < FieldSize - str.Length; i++)
                {
                    Sprite emptySprite = _blockSprites[0];
                    emptySprite.Position = position;
                    _window.Draw(emptySprite);

                    position += new Vector2f(BlockWidth, 0);
                }
            }

            foreach (char ch in str)
            {
                int code = int.Parse(ch.ToString(CultureInfo.InvariantCulture));
                Sprite numberSprite = _numberSprites[code];
                numberSprite.Position = position;
                _window.Draw(numberSprite);

                position += new Vector2f(BlockWidth, 0);
            }
        }

        private void DrawText(string text, Vector2f position)
        {
            foreach (char ch in text)
            {
                Sprite letterSprite = _letterSprites[ch];
                letterSprite.Position = position;
                _window.Draw(letterSprite);

                position += new Vector2f(BlockWidth, 0);
            }
        }
    }
}
