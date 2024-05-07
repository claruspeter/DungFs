namespace DungFs
open System.Collections.Generic

// =======================================================================
//   Model - View - Update
//   ---------------------
//   A standard pattern for taking inputs (keys from the keyboard)
//   and a starting model (our dungeon) and updating it key-by-key
//   until we decide that we are "done".
// =======================================================================

type MVU<'model, 'input> = {
  inputStream: 'input seq
  isDone: 'model -> bool
  update: 'model -> 'input -> 'model
  view: 'model -> 'model
} with 
  member this.start (initialModel:'model) =
    this.view initialModel |> ignore
    this.inputStream
    |> Seq.scan (fun model x -> this.update model x |> this.view ) initialModel
    |> Seq.skipWhile (this.isDone >> not)
    |> Seq.head
