open System
open Browser
open DungFs
open DungFs.Play

type Dispatcher = Dungeon -> Activity -> Dungeon 

let private actionClicked (dispatcher: Dispatcher) model (cmd:Activity) (evt: Types.MouseEvent): unit=
  printfn "click: %A" cmd.name
  dispatcher model cmd |> ignore
  ()

let private viewActions (dispatcher: Dispatcher) model =
  let actions = document.getElementById("activities")
  actions.innerHTML <- ""
  model
  |> availableActivities
  |> Seq.map (fun x -> 
      let btn = document.createElement("button")
      btn.innerText <- x.name
      btn.onclick <- actionClicked dispatcher model x
      btn
  )
  |> Seq.map (actions.appendChild)
  |> fun x -> x |> Seq.length |> printfn "%d actions available"
  model

let private viewMessages model = 
  let messages = document.getElementById("messages")
  model.messages
  |> Seq.map (fun m -> $"<li>{m}</li>")
  |> fun x -> String.Join("\n", x)
  |> fun x -> messages.innerHTML <- x
  model

let private viewData domId data =
  document.getElementById(domId).innerHTML <- data.ToString()

let private viewPlayer model = 
  viewData "player_health" model.player.health
  viewData "player_wealth" model.player.gold
  model

let private viewRoom model = 
  match model.here.inhabitant with 
    | None -> " No-one is in the room"
    | Some monster -> 
      (if monster.isDead then "dead" else monster.health.ToString())
      |> sprintf " %s (%s)" monster.name 
  |> viewData "room_inhabitant"
  viewData "room_name" model.here.name
  viewData "room_wealth" model.here.gold
  viewData "room_exits" model.here.exitString
  model

let private view (dispatcher: Dispatcher) model =
  model 
  |> viewPlayer
  |> viewRoom
  |> viewMessages
  |> viewActions dispatcher
  
let rec update (dung:Dungeon) (activity:Activity) : Dungeon =
  dung
  |> activity.go
  |> view update

Play.standardDice
|> Play.enterDungeon
|> view update
|> ignore