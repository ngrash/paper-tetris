using System;

using SFML.Window;

namespace Tetris
{
    internal class Tetromino
    {
        #region Tetrominos

        #region I

        private static readonly Tetromino I = new Tetromino
        {
            Rotations =
            new[]
                    {
                        new Grid<int>(new[,] 
                        {
                            { 1 },
                            { 1 },
                            { 1 },
                            { 1 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 1, 1, 1, 1 }
                            }),
                        new Grid<int>(new[,] 
                        {
                            { 1 },
                            { 1 },
                            { 1 },
                            { 1 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 1, 1, 1, 1 }
                            })
                    }
        };

        #endregion

        #region J

        private static readonly Tetromino J = new Tetromino
        {
            Rotations =
            new[]
                    {
                        new Grid<int>(new[,] 
                        {
                            { 0, 2 },
                            { 0, 2 },
                            { 2, 2 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 2, 0, 0 },
                              { 2, 2, 2 }
                            }),
                        new Grid<int>(new[,] 
                        {
                            { 2, 2 },
                            { 2, 0 },
                            { 2, 0 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 2, 2, 2 },
                              { 0, 0, 2 }
                            })
                    }
        };

        #endregion

        #region L

        private static readonly Tetromino L = new Tetromino
        {
            Rotations =
            new[]
                    {
                        new Grid<int>(new[,] 
                        {
                            { 3, 0 },
                            { 3, 0 },
                            { 3, 3 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 3, 3, 3 },
                              { 3, 0, 0 }
                            }),
                        new Grid<int>(new[,] 
                        {
                            { 3, 3 },
                            { 0, 3 },
                            { 0, 3 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 0, 0, 3 },
                              { 3, 3, 3 }
                            })
                    }
        };

        #endregion

        #region O

        private static readonly Tetromino O = new Tetromino
            {
                Rotations =
                    new[]
                        {
                            new Grid<int>(new[,]
                                {
                                    { 4, 4 },
                                    { 4, 4 }
                                }), 
                            new Grid<int>(new[,]
                                {
                                    { 4, 4 }, 
                                    { 4, 4 }
                                }),
                            new Grid<int>(new[,]
                                {
                                    { 4, 4 },
                                    { 4, 4 }
                                }), 
                            new Grid<int>(new[,]
                                {
                                    { 4, 4 }, 
                                    { 4, 4 }
                                })
                        }
            };

        #endregion

        #region S

        private static readonly Tetromino S = new Tetromino
        {
            Rotations =
            new[]
                    {
                        new Grid<int>(new[,] 
                        {
                            { 0, 5, 5 },
                            { 5, 5, 0 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 5, 0 },
                              { 5, 5 },
                              { 0, 5 }
                            }),
                        new Grid<int>(new[,] 
                        {
                            { 0, 5, 5 },
                            { 5, 5, 0 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 5, 0 },
                              { 5, 5 },
                              { 0, 5 }
                            })
                    }
        };

        #endregion

        #region T

        private static readonly Tetromino T = new Tetromino
        {
            Rotations =
            new[]
                    {
                        new Grid<int>(new[,] 
                        {
                            { 0, 6, 0 },
                            { 6, 6, 6 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 6, 0 },
                              { 6, 6 },
                              { 6, 0 }
                            }),
                        new Grid<int>(new[,] 
                        {
                            { 6, 6, 6 },
                            { 0, 6, 0 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 0, 6 },
                              { 6, 6 },
                              { 0, 6 }
                            })
                    }
        };

        #endregion

        #region Z

        private static readonly Tetromino Z = new Tetromino
        {
            Rotations =
            new[]
                    {
                        new Grid<int>(new[,] 
                        {
                            { 7, 7, 0 },
                            { 0, 7, 7 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 0, 7 },
                              { 7, 7 },
                              { 7, 0 }
                            }),
                        new Grid<int>(new[,] 
                        {
                            { 7, 7, 0 },
                            { 0, 7, 7 }
                        }),
                        new Grid<int>(new[,]
                            {
                              { 0, 7 },
                              { 7, 7 },
                              { 7, 0 }
                            })
                    }
        };

        #endregion

        #endregion

        private static readonly Random Random = new Random();

        private int _rotation;

        private int _potentialRotation;

        public Vector2i TopLeft { get; set; }

        public Vector2i PotentialTopLeft { get; set; }

        public Grid<int> Blocks
        {
            get
            {
                return Rotations[Rotation];
            }
        }

        public Grid<int> PotentialBlocks
        {
            get
            {
                return Rotations[PotentialRotation];
            }
        }

        public int Width
        {
            get
            {
                return Blocks.Width;
            }
        }

        public int Height
        {
            get
            {
                return Blocks.Height;
            }
        }

        public int PotentialWidth
        {
            get
            {
                return PotentialBlocks.Width;
            }
        }

        public int PotentialHeight
        {
            get
            {
                return PotentialBlocks.Height;
            }
        }

        public int Rotation
        {
            get
            {
                return _rotation;
            }

            set
            {
                _rotation = LimitRotation(value);
            }
        }

        public int PotentialRotation
        {
            get
            {
                return _potentialRotation;
            }

            set
            {
                _potentialRotation = LimitRotation(value);
            }
        }

        private Grid<int>[] Rotations { get; set; }

        public static Tetromino NewRandom()
        {
            Tetromino prototype;
            int next = Random.Next(7);
            switch (next)
            {
                case 0:
                    prototype = O;
                    break;
                case 1:
                    prototype = J;
                    break;
                case 2:
                    prototype = L;
                    break;
                case 3:
                    prototype = T;
                    break;
                case 4:
                    prototype = I;
                    break;
                case 5:
                    prototype = Z;
                    break;
                case 6:
                    prototype = S;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Tetromino tetromino = NewRandomRotation(prototype);
            return tetromino;
        }

        private static Tetromino New(Tetromino prototype)
        {
            return new Tetromino { Rotations = prototype.Rotations };
        }

        private static Tetromino NewRandomRotation(Tetromino prototype)
        {
            Tetromino tetromino = New(prototype);
            tetromino.Rotation = Random.Next(tetromino.Rotations.Length);
            return tetromino;
        }

        private int LimitRotation(int value)
        {
            return value < Rotations.Length ? value : 0;
        }
    }
}
