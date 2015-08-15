using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTech;
using System.Linq;
using static TicTacToeDomain;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var gameState0 = TicTacToe.startGame();
            var gameState1 = TicTacToe.makeMove(gameState0, new CellPosition(HorizontalPosition.Left, VerticalPosition.Top));
            var gameState1Ser = TicTacToeGameStateSerializer.serialize(gameState1);

            //gameState.board.First().position.horizontal.
        }
    }
}
