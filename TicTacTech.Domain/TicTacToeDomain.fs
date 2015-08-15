(*
Inspired by blog post: http://fsharpforfunandprofit.com/posts/enterprise-tic-tac-toe-2/
*)

module TicTacToeDomain

type HorizontalPosition = 
    | Left
    | HCenter
    | Right

type VerticalPosition = 
    | Top
    | VCenter
    | Bottom

type CellPosition = 
    { horizontal : HorizontalPosition
      vertical : VerticalPosition }

type CellState = 
    | PlayedByX
    | PlayedByO
    | Empty

type Cell = 
    { position : CellPosition
      state : CellState }

type GameStatus = 
    | MoveRequiredByX
    | MoveRequiredByO
    | WonByX
    | WonByO
    | Tie

type GameState = 
    { board : Cell seq
      status : GameStatus }

