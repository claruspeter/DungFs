open System
open DungFs
open DungFs.Play


let view model =
  Console.Clear()
  printfn "==============================================================="
  printfn "                    Welcome to DungeonFs!"
  printfn "==============================================================="
  printfn ""
  printfn "Player"
  printfn "---------------------------------------------------------------"
  printfn " Health: %d%%" model.player.health
  printfn " Wealth: %d gold" model.player.gold
  printfn ""
  printfn "Room"
  printfn "---------------------------------------------------------------"
  match model.here.inhabitant with 
  | None -> printfn " No-one is in the room"
  | Some monster -> 
    printfn " Monster (%A)" (if monster.isDead then "dead" else monster.health.ToString())
  printfn " Gold: %d gold" model.here.gold
  printfn " Exits: %s" model.here.exitString
  printfn ""
  printfn "==============================================================="
  printfn "%s" model.message
  printfn "==============================================================="

  printfn "   [Q]uit - [P]ickup Gold - Move %s - [A]ttack" (model.here.exitString |> Seq.map (sprintf "[%c]") |> fun x -> String.Join("", x))

  model

let update (dung:Dungeon) (ch:char) : Dungeon =
  dung 
  |>  match ch with 
      | 'p' -> pickupGold
      | 'a' -> attack
      | _ -> id


let gameLoop (initialModel:Dungeon) =
  view initialModel |> ignore
  (fun _ -> Console.ReadKey(true).KeyChar |> Char.ToLowerInvariant )
  |> Seq.initInfinite
  |> Seq.takeWhile (fun x -> x <> 'q')
  |> Seq.fold (fun model x -> update model x |> view ) initialModel


// Main //
enterDungeon(standardDice)
|> gameLoop
|> ignore



