using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicTacTech;
using System.Linq;
using static TicTacToeDomain;

namespace Tests
{
    public static class GameStateExt
    {
        public static GameState Move(this GameState gameState, HorizontalPosition hp, VerticalPosition vp)
        {
            return TicTacToe.makeMove(gameState, new CellPosition(hp, vp));
        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var gameState = TicTacToe.startGame();
            gameState = gameState
                .Move(HorizontalPosition.Left, VerticalPosition.Top)
                .Move(HorizontalPosition.Left, VerticalPosition.VCenter)
                .Move(HorizontalPosition.HCenter, VerticalPosition.Top)
                .Move(HorizontalPosition.HCenter, VerticalPosition.VCenter)
                .Move(HorizontalPosition.Right, VerticalPosition.Top);

            Assert.IsTrue(gameState.status.IsWonByX);

            //gameState.board.First().position.horizontal.
        }
    }
}
