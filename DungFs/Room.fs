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

  let generateRoomTo direction =
    {
      Room.empty [] with 
        name = "an Empty Room"
    }