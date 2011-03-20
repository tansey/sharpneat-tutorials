/* ***************************************************************************
 * This file is part of the NashCoding tutorial on SharpNEAT 2.
 * 
 * Copyright 2010, Wesley Tansey (wes@nashcoding.com)
 * 
 * Both SharpNEAT and this tutorial are free software: you can redistribute
 * it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the 
 * License, or (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */
using TicTacToeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestTicTacToe
{
    /// <summary>
    /// This is a test class for TicTacToeGameTest and is intended
    /// to contain all TicTacToeGameTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TicTacToeGameTest
    {


        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// A test for GetBoardFromString
        ///</summary>
        [TestMethod()]
        public void GetBoardFromStringTest()
        {
            string boardString = @" | | 
                                    | | 
                                    | | ";
            SquareTypes[,] expected = new SquareTypes[3,3];
            SquareTypes[,] actual;
            actual = TicTacToeGame.GetBoardFromString(boardString);
            AssertEqualBoards(expected, actual);

            boardString = @"X| |O
                             |O| 
                            X| |X";
            expected[0, 0] = SquareTypes.X;
            expected[1, 0] = SquareTypes.N;
            expected[2, 0] = SquareTypes.O;
            expected[0, 1] = SquareTypes.N;
            expected[1, 1] = SquareTypes.O;
            expected[2, 1] = SquareTypes.N;
            expected[0, 2] = SquareTypes.X;
            expected[1, 2] = SquareTypes.N;
            expected[2, 2] = SquareTypes.X;
            actual = TicTacToeGame.GetBoardFromString(boardString);
            AssertEqualBoards(expected, actual);
        }

        private void AssertEqualBoards(SquareTypes[,] expected, SquareTypes[,] actual)
        {
            Assert.AreEqual(expected[0, 0], actual[0, 0]);
            Assert.AreEqual(expected[1, 0], actual[1, 0]);
            Assert.AreEqual(expected[2, 0], actual[2, 0]);
            Assert.AreEqual(expected[0, 1], actual[0, 1]);
            Assert.AreEqual(expected[1, 1], actual[1, 1]);
            Assert.AreEqual(expected[2, 1], actual[2, 1]);
            Assert.AreEqual(expected[0, 2], actual[0, 2]);
            Assert.AreEqual(expected[1, 2], actual[1, 2]);
            Assert.AreEqual(expected[2, 2], actual[2, 2]);
        }

        /// <summary>
        /// A test for GetWinner
        ///</summary>
        [TestMethod()]
        public void GetWinnerTest()
        {
            SquareTypes[,] board = 
            TicTacToeGame.GetBoardFromString(@" | | 
                                                | | 
                                                | | ");
            SquareTypes expected = SquareTypes.N;
            SquareTypes actual;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Diagnol left to right
            board =
            TicTacToeGame.GetBoardFromString(@"X|O|O
                                               O|X|O
                                               O|O|X");
            expected = SquareTypes.X;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Diagnol right to left
            board =
            TicTacToeGame.GetBoardFromString(@"O|O|X
                                               O|X| 
                                               X|O|X");
            expected = SquareTypes.X;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Horizontal bottom
            board =
            TicTacToeGame.GetBoardFromString(@"X| |X
                                               X|X|O
                                               O|O|O");
            expected = SquareTypes.O;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Horizontal middle
            board =
            TicTacToeGame.GetBoardFromString(@"X|O|O
                                               X|X|X
                                               O|O| ");
            expected = SquareTypes.X;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Horizontal top
            board =
            TicTacToeGame.GetBoardFromString(@"O|O|O
                                               X|X|O
                                                |O|X");
            expected = SquareTypes.O;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Vertical right
            board =
            TicTacToeGame.GetBoardFromString(@"X|O|O
                                               O|X|O
                                               X|O|O");
            expected = SquareTypes.O;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Vertical middle
            board =
            TicTacToeGame.GetBoardFromString(@"X|O| 
                                                |O|X
                                               X|O| ");
            expected = SquareTypes.O;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //Vertical left
            board =
            TicTacToeGame.GetBoardFromString(@"X|O| 
                                               X| |O
                                               X|O| ");
            expected = SquareTypes.X;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);


            //No winner
            board =
            TicTacToeGame.GetBoardFromString(@"X|O| 
                                                | |O
                                               X|O| ");
            expected = SquareTypes.N;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //No winner
            board =
            TicTacToeGame.GetBoardFromString(@"X|O|O
                                                |X|O
                                               X|O| ");
            expected = SquareTypes.N;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //No winner
            board =
            TicTacToeGame.GetBoardFromString(@"X|X|
                                               O|X|O
                                               X|O|O ");
            expected = SquareTypes.N;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);

            //No winner
            board =
            TicTacToeGame.GetBoardFromString(@"X|X|O
                                               O|X|X
                                               X|O|O");
            expected = SquareTypes.N;
            actual = TicTacToeGame.GetWinner(board);
            Assert.AreEqual(expected, actual);



        }
    }
}
