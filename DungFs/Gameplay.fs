
module DungFs.Play
open System
open DungFs


let private R = Random()

let increaseGold amount (person:Person) =
  { person with gold = person.gold + amount }

let dropGold (person:Person) =
  { person with gold = 0 }

let takeDamage amount (person:Person) =
  { person with health = Math.Max(0, person.health - amount) }

///<summary>A possible activity that a player can do</summary>
type Activity = {
  name: string
  command: char
  go: Dungeon -> Dungeon
} with
  member this.label = 
    let replacement = $"[{this.command}]".ToUpperInvariant()
    match this.name.ToLowerInvariant().IndexOf this.command with
    | -1 -> $"{this.name} {replacement}"
    | i -> this.name.Remove(i,1).Insert(i,replacement)

let quit = 
  {
    name="Quit"
    command = 'q'
    go = fun dungeon ->
      {
        dungeon with 
          gameOver = true
          message = "You lay down and quietly die"
      }
  }

let pickupGold = 
  { 
    name="Pickup Gold"
    command = 'p'
    go = fun dungeon ->
      {
        dungeon with 
          player = dungeon.player |> increaseGold dungeon.here.gold
          here = {dungeon.here with gold = 0}
          message = $"You pick up {dungeon.here.gold} gold"
      }
  }


let attack =
  {
    name = "Attack"
    command = 'a'
    go = fun (dungeon: Dungeon) ->
      match dungeon.here.inhabitant with 
      | None -> {dungeon with message = "There is no one in the room"}
      | Some monster -> 
        let damage = dungeon.dice.roll(20)
        match monster.isDead, monster.health <= damage  with 
        | true, _ -> {dungeon with message="You attack a dead body"}
        | false, false ->
          {
              dungeon with 
                here = { dungeon.here with inhabitant = monster |> takeDamage damage |> Some }
                message = $"You attack and cause {damage} damage"
          }
        | false, true ->
          {
              dungeon with 
                here = { 
                  dungeon.here with 
                    inhabitant = monster |> takeDamage damage |> dropGold |> Some 
                    gold = dungeon.here.gold + monster.gold
                }
                message = $"You attack and kill the creature"
          }
  }

let private move direction dungeon =
  match dungeon.here.exits |> List.contains direction with 
  | false -> {dungeon with message = $"Unable to move ${direction}"}
  | true -> 
    let newRoom = Rooms.generateRoomTo dungeon.dice direction
    {
      dungeon with 
        here = newRoom
        message = $"You go {direction} into {newRoom.name}"
    }

let goNorth = 
  {
    name = "Go North"
    command = 'n'
    go = move North
  }
let goEast = 
  {
    name = "Go East"
    command = 'e'
    go = move East
  }
let goSouth = 
  {
    name = "Go South"
    command = 's'
    go = move South
  }  
let goWest = 
  {
    name = "Go West"
    command = 'w'
    go = move West
  }

let availableActivities dungeon =
  seq{
    yield quit
    if dungeon.here.gold > 0 then yield pickupGold
    if dungeon.here.inhabitant.IsSome then yield attack
    if dungeon.here.exits |> Seq.contains North then yield goNorth
    if dungeon.here.exits |> Seq.contains East then yield goEast
    if dungeon.here.exits |> Seq.contains West then yield goWest
    if dungeon.here.exits |> Seq.contains South then yield goSouth
  }

let standardDice = {rollFunction = fun size -> R.Next(size) + 1}

let enterDungeon dice =
  {
    player = Person.empty
    here = {name = "the Entrance"; exits = [ North ]; gold = 3; inhabitant = None}
    dice = dice
    message = "You walk into the Entrance. Choose what to do next wisely"
    gameOver = false
  }