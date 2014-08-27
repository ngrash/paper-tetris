using System;
using System.Diagnostics;

using SFML.Graphics;
using SFML.Window;

namespace Tetris
{
    internal class Game
    {
        private const int WindowWidth = GridWidth * BlockWidth;
        private const int WindowHeight = GridHeight * BlockHeigth;

        private const int GridWidth = 10;
        private const int GridHeight = 16;

        private const int BlockWidth = 32;
        private const int BlockHeigth = 32;

        private readonly RenderWindow _window;
        private readonly Grid<int> _grid;

        private Tetromino _tetromino;

        private float _secondsSinceMoveDown;

        private bool _isMovingLeft;
        private bool _isMovingRight;
        private bool _isMovingDown;
        private bool _isRotating;

        public Game()
        {
            _window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), "Tetris");
            _window.Closed += (s, e) => _window.Close();
            _window.KeyPressed += WindowOnKeyPressed;

            _window.SetFramerateLimit(60);

            _grid = new Grid<int>(GridWidth, GridHeight);

            _tetromino = Tetromino.NewRandom();
            _tetromino.PotentialTopLeft = new Vector2i(3, 0);
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
                for (int y = 0; y < GridHeight; y++)
                {
                    bool rowIsComplete = true;

                    for (int x = 0; x < GridWidth; x++)
                    {
                        rowIsComplete &= _grid[x, y] != 0;
                    }

                    if (rowIsComplete)
                    {
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

                _tetromino = Tetromino.NewRandom();
            }
            else
            {
                _tetromino.TopLeft = _tetromino.PotentialTopLeft;
            }
        }

        private void Draw()
        {
            _window.Clear();

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    if (_grid[x, y] == 0)
                    {
                        _window.Draw(
                            new RectangleShape(new Vector2f(BlockWidth, BlockHeigth))
                                {
                                    FillColor = (x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1) ? new Color(10, 10, 10) : new Color(30, 30, 30),
                                    Position = new Vector2f(x * BlockWidth, y * BlockHeigth)
                                });
                    }
                    else
                    {
                        _window.Draw(
                            new RectangleShape(new Vector2f(BlockWidth, BlockHeigth))
                                {
                                    FillColor = new Color(200, 30, 30),
                                    Position = new Vector2f(x * BlockWidth, y * BlockHeigth)
                                });
                    }
                }
            }

            for (int x = 0; x < _tetromino.Width; x++)
            {
                for (int y = 0; y < _tetromino.Height; y++)
                {
                    if (_tetromino.Blocks[x, y] != 0)
                    {
                        _window.Draw(
                            new RectangleShape(new Vector2f(BlockWidth, BlockHeigth))
                                {
                                    FillColor = new Color(200, 30, 30),
                                    Position =
                                        new Vector2f(
                                            (_tetromino.TopLeft.X + x) * BlockWidth,
                                            (_tetromino.TopLeft.Y + y) * BlockHeigth)
                                });
                    }
                }
            }

            _window.Display();
        }
    }
}
