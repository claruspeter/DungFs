module Tests.Activities

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs
open DungFs.Play

let fixedDice _ = 5
let dungeon = Play.enterDungeon fixedDice

[<Fact>]
let ``Player may always quit`` () =
  dungeon
  |> availableActivities
  |> Seq.map (fun x -> x.name)
  |> should contain quit.name

[<Fact>]
let ``Player may pickup gold if it is present`` () =
  dungeon
  |> availableActivities
  |> Seq.map (fun x -> x.name)
  |> should contain pickupGold.name

[<Fact>]
let ``Player may not pickup gold if there isn't any`` () =
  dungeon
  |> pickupGold.go
  |> availableActivities
  |> Seq.map (fun x -> x.name)
  |> shouldNotContain pickupGold.name

[<Fact>]
let ``Player may move towards existing exits`` () =
  dungeon
  |> availableActivities
  |> Seq.map (fun x -> x.name)
  |> should contain goNorth.name

[<Fact>]
let ``Player may not move in a direction that doesn't have an exit`` () =
  let activities =
    dungeon
    |> availableActivities
    |> Seq.map (fun x -> x.name)
  activities |> shouldNotContain goEast.name
  activities |> shouldNotContain goWest.name
  activities |> shouldNotContain goSouth.name