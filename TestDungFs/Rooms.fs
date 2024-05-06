module Tests.Rooms

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs

let room = Room.empty [North ; West]

[<Fact>]
let ``A room may have exits`` () =
  room.exits.Length |> should equal 2

[<Fact>]
let ``A room may have gold in it`` () =
  let richRoom = {room with gold = 24}
  richRoom.gold |> should equal 24

[<Fact>]
let ``A room may have people in it`` () =
  let crowdedRoom = {room with inhabitant = Some Person.empty}
  crowdedRoom.inhabitant |> shouldNotEqual None