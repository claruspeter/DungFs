module Tests.Gameplay

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs

let dungeon = enterDungeon()

[<Fact>]
let ``Game starts with healthy person`` () =
  dungeon.player.health |> should equal 100

[<Fact>]
let ``Game starts with broke person`` () =
  dungeon.player.gold |> should equal 0

[<Fact>]
let ``Game starts with a valid room`` () =
  dungeon.here.exits.IsEmpty |> should equal false

[<Fact>]
let ``Player can pick up gold from room`` () =
  let updated = dungeon |> pickupGold
  updated.player.gold |> should equal 3
  updated.here.gold |> should equal 0