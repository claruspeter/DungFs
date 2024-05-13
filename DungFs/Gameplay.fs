
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

let private defend dungeon =
  let damage = dungeon.dice.roll(20)
  match dungeon.here.inhabitant, dungeon.player.isDead, dungeon.player.health <= damage  with 
  | None, _, _ -> dungeon
  | Some monster, _, _  when monster.isDead -> dungeon
  | Some monster, true, _ -> dungeon |> Builder.msg $"The {monster.name} wails on your dead body"
  | Some monster, false, false ->
    {
      dungeon with 
        player = {dungeon.player with health = dungeon.player.health - damage}
    }
    |> Builder.msg  $"{monster.name} attacks and causes you {damage} damage"
  | Some monster, false, true ->
    {
        dungeon with 
          player = {dungeon.player with health = 0; gold = 0}
          gameOver = true
          here = { 
            dungeon.here with 
              gold = dungeon.here.gold + dungeon.player.gold
          }
    }
    |> Builder.msg $"{monster.name} kills you.  Your mouldy bones will lie in this room forever"

let private preemptAttack dungeon =
  if dungeon.here.inhabitant.IsSome && dungeon.dice.roll(20) > 10 then 
    dungeon |> defend
  else
    dungeon

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
      }
      |> Builder.msg "You lay down and quietly die"
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
      }
      |> Builder.msg $"You pick up {dungeon.here.gold} gold"

  }


let attack =
  {
    name = "Attack"
    command = 'a'
    go = fun (dungeon: Dungeon) ->
      match dungeon.here.inhabitant with 
      | None -> dungeon |> Builder.msg "There is no one in the room"
      | Some monster -> 
        let damage = dungeon.dice.roll(20)
        match monster.isDead, monster.health <= damage  with 
        | true, _ -> dungeon |> Builder.msg "You attack a dead body"
        | false, false ->
          {
              dungeon with 
                here = { dungeon.here with inhabitant = monster |> takeDamage damage |> Some }
          } 
          |> Builder.msg  $"You attack and cause {damage} damage"
          |> defend
        | false, true ->
          {
              dungeon with 
                here = { 
                  dungeon.here with 
                    inhabitant = monster |> takeDamage damage |> dropGold |> Some 
                    gold = dungeon.here.gold + monster.gold
                }
          }
          |> Builder.msg  $"You attack and kill the creature"
  }

let private move direction dungeon =
  match dungeon.here.exits |> List.contains direction with 
  | false -> dungeon |> Builder.msg  $"Unable to move ${direction}"
  | true -> 
    let newRoom = Rooms.generateRoomTo dungeon.dice direction
    {
      dungeon with 
        here = newRoom
    }
    |> Builder.msg $"You go {direction} into the {newRoom.name}"
    |> preemptAttack

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
  match dungeon.player.isDead with 
  | true -> seq []
  | false ->
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
    messages = ["You walk into the Entrance. Choose what to do next wisely"]
    gameOver = false
  }