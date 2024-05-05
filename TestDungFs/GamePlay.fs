module Tests.Gameplay

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs

let fixedDice _ = 5
let dungeon = Play.enterDungeon fixedDice

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
let ``Game starts with a 'entered room' message'`` () =
  dungeon.message |> should startWith "You enter a new room"


[<Fact>]
let ``Player can pick up gold from room`` () =
  let updated = dungeon |> Play.pickupGold.go
  updated.player.gold |> should equal 3
  updated.here.gold |> should equal 0

[<Fact>]
let ``When player picks up gold the message says how much`` () =
  let updated = dungeon |> Play.pickupGold.go
  updated.message |> should equal "You pick up 3 gold"

[<Fact>]
let ``Player can attack person in room`` () =
  let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 20}
  let updated = roomWithMonster |> Play.attack.go
  updated.here.inhabitant.Value.health |> should equal 15

[<Fact>]
let ``When player attacks the message says how much damage is inflicted`` () =
  let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 20}
  let updated = roomWithMonster |> Play.attack.go
  updated.message |> should equal "You attack and cause 5 damage"

[<Fact>]
let ``When player attacks and the room is uninhabited the message says there is no-one to attack`` () =
  let updated = dungeon |> Play.attack.go
  updated.message |> should equal "There is no one in the room"

[<Fact>]
let ``When player attacks and the inhabitant is dead the message says it is dead`` () =
  let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 0}
  let updated = roomWithMonster |> Play.attack.go
  updated.message |> should equal "You attack a dead body"

[<Fact>]
let ``When player kills an inhabitant then the inhabitant's gold is added to the room`` () =
  let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 2; gold = 20}
  let updated = roomWithMonster |> Play.attack.go
  updated.here.gold |> should equal 23

[<Fact>]
let ``When player kills an inhabitant then the message says that you killed the inhabitant`` () =
  let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 2; gold = 20}
  let updated = roomWithMonster |> Play.attack.go
  updated.message |> should equal "You attack and kill the creature"