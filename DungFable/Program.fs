open System
open Browser
open DungFs

let private viewMessages model = 
  let messages = document.getElementById("messages")
  model.messages
  |> Seq.map (fun m -> $"<li>{m}</li>")
  |> fun x -> String.Join("\n", x)
  |> fun x -> messages.innerHTML <- x
  model

let private view model =
  model 
  |> viewMessages
  
let update (dung:Dungeon) (ch:char) : Dungeon =
  dung

let gameLoop =
  {
    inputStream = []
    isDone = fun dung -> dung.gameOver
    update = update
    view = view
  }

Play.standardDice
|> Play.enterDungeon
|> gameLoop.start
|> ignore