(*
Inspired by blog post: http://fsharpforfunandprofit.com/posts/enterprise-tic-tac-toe-2/
*)

module TicTacToe

open TicTacToeDomain

let private horizontalPositions = [| Left; HCenter; Right |]
let private verticalPositions = [| Top; VCenter; Bottom |]

let internal allPositions = 
    [| for hp in horizontalPositions do
           for vp in verticalPositions do
               yield { horizontal = hp
                       vertical = vp } |]

type private Player = 
    | PlayerX
    | PlayerO

let private applyMove board pos player = 
    let newCellState = 
        match player with
        | PlayerX -> PlayedByX
        | PlayerO -> PlayedByO
    
    let mapCell oldCell = 
        match oldCell with
        | c when c.position = pos -> 
            match c.state with
            | Empty -> { oldCell with state = newCellState }
            | _ -> failwith "Cell is not empty."
        | _ -> oldCell
    
    board |> Seq.map mapCell

let private linesToCheck = 
    let cp h v = 
        { horizontal = h
          vertical = v }
    
    let diagonalLine1 = 
        [| cp Left Top
           cp HCenter VCenter
           cp Right Bottom |]
    
    let diagonalLine2 = 
        [| cp Left Bottom
           cp HCenter VCenter
           cp Right Top |]
    
    let horLines = 
        verticalPositions |> Seq.map (fun v -> 
                                 horizontalPositions
                                 |> Seq.map (fun h -> cp h v)
                                 |> Seq.toArray)
    
    let vertLines = 
        horizontalPositions |> Seq.map (fun h -> 
                                   verticalPositions
                                   |> Seq.map (fun v -> cp h v)
                                   |> Seq.toArray)
    
    [ yield diagonalLine1
      yield diagonalLine2
      yield! horLines
      yield! vertLines ]

let private checkGameWon board = 
    let map = 
        board
        |> Seq.map (fun c -> (c.position, c))
        |> Map.ofSeq
    
    let checkLineWon (line : CellPosition array) = 
        let firstPos = line.[0]
        let firstCell = map.[firstPos]
        match firstCell.state with
        | Empty -> false
        | _ -> line |> Array.forall (fun p -> map.[p].state = firstCell.state)
    
    let checkLineTie (line : CellPosition array) = line
                                                   |> Array.exists (fun p -> map.[p].state = PlayedByX)
                                                   && line |> Array.exists (fun p -> map.[p].state = PlayedByO)
    linesToCheck |> Seq.exists checkLineWon, linesToCheck |> Seq.forall checkLineTie

let startGame() = 
    let board = 
        allPositions |> Seq.map (fun p -> 
                            { position = p
                              state = Empty })
    { board = board
      status = MoveRequiredByX }

let makeMove gameState position = 
    let player = 
        match gameState.status with
        | MoveRequiredByX -> PlayerX
        | MoveRequiredByO -> PlayerO
        | _ -> failwith "Invalid game state. No move required."
    
    let newBoard = applyMove gameState.board position player
    let won, tie = checkGameWon newBoard
    
    let newStatus = 
        match won, tie with
        | true, _ -> 
            match gameState.status with
            | MoveRequiredByX -> WonByX
            | MoveRequiredByO -> WonByO
            | _ -> failwith "Invalid game state for move."
        | _, true -> Tie
        | _ -> 
            match gameState.status with
            | MoveRequiredByX -> MoveRequiredByO
            | MoveRequiredByO -> MoveRequiredByX
            | _ -> failwith "Invalid game state for move."
    { board = newBoard
      status = newStatus }

let positionByIndex idx = 
    match idx with
    | _ when idx < 0 || idx > 8 -> failwith "Invalid cell index."
    | i -> allPositions.[i]
