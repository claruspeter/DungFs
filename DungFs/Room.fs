namespace DungFs
open System


type Direction = North | East | South | West

type Room ={
  name: string
  exits: Direction list
  gold: int
  inhabitant: Person option
} with 
  static member empty exits = {Room.name = "Empty room"; exits = exits; gold=0; inhabitant = None}
  member this.exitString = this.exits |> List.map (fun x -> x.ToString()) |> fun x -> String.Join(",", x)

module Rooms =
  let withInhabitant person room =
    {
      room with inhabitant = Some person
    }

  let opposite =
    function 
    | North -> South
    | East -> West
    | West -> East
    | South -> North

  let rec private shuffle (dice:Dice) deck = 
    match deck with 
    | [] -> []
    | [a] -> [a]
    | _ ->
        let randomPosition = dice.roll(deck.Length) - 1
        let cardAtPosition = deck[randomPosition]
        let rest = deck |> List.removeAt randomPosition
        [cardAtPosition] @ (shuffle dice rest)

  let generateRoomTo (dice:Dice) direction =
    {
      Room.empty [] with 
        name = Data.rooms[dice.roll(Data.rooms.Length) - 1]
        gold = if dice.roll(20) > 10 then dice.roll(20) else 0
        exits = 
          [North; East; West; South]
          |> List.filter (fun x -> x <> (opposite direction))
          |> shuffle dice
          |> fun dirs -> dirs |> List.take (dice.roll(dirs.Length))
        inhabitant = if dice.roll(20) > 5 then Data.people[dice.roll(Data.people.Length) - 1]() |> Some else None

    }