open System
open DungFs
open DungFs.Play
open System.Collections.Generic


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
  printfn "Room - %s" model.here.name
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
  model 
    |> availableActivities
    |> Seq.map (fun a -> a.label)
    |> fun x -> String.Join(" - ", x)
    |> printfn "   %s"
  model

let update (dung:Dungeon) (ch:char) : Dungeon =
  dung 
  |> availableActivities
  |> Seq.tryFind (fun x -> x.command = ch)
  |>  function
      | None -> dung
      | Some activity -> dung |> activity.go

let takeWhileInclusive predicate (s:seq<_>) = 
  let rec loop (en:IEnumerator<_>) = 
    seq {
      if en.MoveNext() then
        yield en.Current
      if predicate en.Current then
        yield! loop en 
    }
  seq { 
    use en = s.GetEnumerator()
    yield! loop en
  }



let gameLoop (initialModel:Dungeon) =
  view initialModel |> ignore
  (fun _ -> Console.ReadKey(true).KeyChar |> Char.ToLowerInvariant )
  |> Seq.initInfinite
  |> takeWhileInclusive (fun x -> x <> 'q')
  |> Seq.fold (fun model x -> update model x |> view ) initialModel

// Main //
enterDungeon(standardDice)
|> gameLoop
|> ignore

