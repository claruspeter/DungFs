
module DungFs.Console
open System.Collections.Generic
open System
open DungFs
open DungFs.Play

let private view model =
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
  printfn "Room - %s" model.here.name
  printfn "---------------------------------------------------------------"
  match model.here.inhabitant with 
  | None -> printfn " No-one is in the room"
  | Some monster -> 
    printfn " %s (%s)" monster.name (if monster.isDead then "dead" else monster.health.ToString())
  printfn " Gold: %d gold" model.here.gold
  printfn " Exits: %s" model.here.exitString
  printfn ""
  printfn "==============================================================="
  printfn "%s" model.message
  printfn "==============================================================="
  model 
    |> availableActivities
    |> Seq.map (fun a -> a.label)
    |> fun x -> String.Join(" - ", x)
    |> printfn "   %s"
  model

let private update (dung:Dungeon) (ch:char) : Dungeon =
  dung 
  |> availableActivities
  |> Seq.tryFind (fun x -> x.command = ch)
  |>  function
      | None -> dung
      | Some activity -> dung |> activity.go

let gameLoop =
  {
    inputStream = (fun _ -> Console.ReadKey(true).KeyChar |> Char.ToLowerInvariant ) |> Seq.initInfinite
    isDone = fun dung -> dung.gameOver
    update = update
    view = view
  }