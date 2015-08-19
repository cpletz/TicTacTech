module TicTacToeGameStateSerializer

open System
open System.Runtime.Serialization
open TicTacToeDomain
open TicTacToe

[<DataContract>]
type SerGameState() = 
    
    [<DataMember>]
    member val board : string = null with get, set
    
    [<DataMember>]
    member val moveRequiredBy : string = null with get, set
    
    [<DataMember>]
    member val wonBy : string = null with get, set

let deserialize (serGameState : SerGameState) = 
    let deserializeCellState s = 
        match s with
        | 'E' -> Empty
        | 'X' -> PlayedByX
        | 'O' -> PlayedByO
        | _ -> failwith "Unknown CellState"
    
    let deserializeBoard s = 
        s
        |> Seq.zip allPositions
        |> Seq.map (fun (x, y) -> 
               { position = x
                 state = deserializeCellState y })
    
    let deserializeGameStatus moverequiredBy wonBy = 
        match moverequiredBy with
        | "X" -> MoveRequiredByX
        | "O" -> MoveRequiredByO
        | _ -> 
            match wonBy with
            | "X" -> WonByX
            | "O" -> WonByO
            | _ -> Tie
    
    let cells = deserializeBoard serGameState.board
    let status = deserializeGameStatus serGameState.moveRequiredBy serGameState.wonBy
    { board = cells
      status = status }

let serialize gameState = 
    let serCellState cellState = 
        match cellState with
        | PlayedByX -> 'X'
        | PlayedByO -> 'O'
        | Empty -> 'E'
    
    let serBoard board = 
        board
        |> Seq.map (fun c -> serCellState c.state)
        |> Seq.toArray
        |> String
    
    let serGameStatus status = 
        match status with
        | MoveRequiredByX -> ("X", null)
        | MoveRequiredByO -> ("O", null)
        | WonByX -> (null, "X")
        | WonByO -> (null, "O")
        | Tie -> (null, null)
    
    let (move, won) = serGameStatus gameState.status
    let sgs = SerGameState()
    sgs.board <- serBoard gameState.board
    sgs.moveRequiredBy <- move
    sgs.wonBy <- won
    sgs

