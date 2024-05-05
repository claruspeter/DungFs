namespace DungFs
open System

type Person = {
  health: int
  gold: int
} with 
  static member empty = { health = 100; gold = 0}
  member this.isDead = this.health <= 0

type Direction = N | E | S | W

type Room ={
  exits: Direction list
  gold: int
  inhabitant: Person option
} with 
  static member empty exits = {Room.exits = exits; gold=0; inhabitant = None}
  member this.exitString = this.exits |> List.map (fun x -> x.ToString()) |> fun x -> String.Join(",", x)

type Dungeon = {
  player: Person
  here: Room
  message: string
  dice: int -> int
}

module Builder = 
  let withInhabitant person dungeon =
    {
      dungeon with 
        here = {
          dungeon.here with inhabitant = Some person
        }
    }


module Play =
  let private R = Random()
  
  let increaseGold amount (person:Person) =
    { person with gold = person.gold + amount }

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
            message = "You lay down and quietly wait for death to take you"
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
          let damage = dungeon.dice(20)
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
                      inhabitant = monster |> takeDamage damage |> Some 
                      gold = dungeon.here.gold + monster.gold
                  }
                  message = $"You attack and kill the creature"
            }
    }
  
  let goNorth = 
    {
      name = "Go North"
      command = 'n'
      go = id
    }
  let goEast = 
    {
      name = "Go East"
      command = 'e'
      go = id
    }
  let goSouth = 
    {
      name = "Go South"
      command = 's'
      go = id
    }  
  let goWest = 
    {
      name = "Go West"
      command = 'w'
      go = id
    }

  let availableActivities dungeon =
    seq{
      yield quit
      if dungeon.here.gold > 0 then yield pickupGold
      if dungeon.here.exits |> Seq.contains N then yield goNorth
      if dungeon.here.exits |> Seq.contains E then yield goEast
      if dungeon.here.exits |> Seq.contains W then yield goWest
      if dungeon.here.exits |> Seq.contains S then yield goSouth
    }

  let standardDice size = R.Next(size) + 1

  let enterDungeon dice =
    {
      player = Person.empty
      here = {exits = [ N ]; gold = 3; inhabitant = None}
      dice = dice
      message = "You enter a new room. Choose what to do next wisely"
    }