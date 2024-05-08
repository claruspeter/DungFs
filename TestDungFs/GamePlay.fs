module Tests.Gameplay

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs
open Tests.Helpers

let dungeon = Play.enterDungeon minDice

module Initial =
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
    dungeon.message |> should startWith "You walk into the Entrance"

module RoomInteraction =
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
  let ``Player can move into another room`` () =
    let updated = dungeon |> Play.goNorth.go
    updated.here.name |> shouldNotEqual "Entrance"

  [<Fact>]
  let ``When move into another room the message reflects the movement`` () =
    let updated = dungeon |> Play.goNorth.go
    updated.message |> should startWith "You go North into "

module Battle =
  [<Fact>]
  let ``Player can attack person in room`` () =
    let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 20}
    let updated = roomWithMonster |> Play.attack.go
    updated.here.inhabitant.Value.health |> should equal 19

  [<Fact>]
  let ``When player attacks the message says how much damage is inflicted`` () =
    let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 20}
    let updated = roomWithMonster |> Play.attack.go
    updated.messages.[1] |> should equal "You attack and cause 1 damage"

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
    let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 1; gold = 20}
    let updated = roomWithMonster |> Play.attack.go
    updated.here.gold |> should equal 23

  [<Fact>]
  let ``When player kills an inhabitant then the message says that you killed the inhabitant`` () =
    let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 1; gold = 20}
    let updated = roomWithMonster |> Play.attack.go
    updated.message |> should equal "You attack and kill the creature"

  [<Fact>]
  let ``When player enters a room there is a 50% chance any existing inhabitant immediately attacks`` () =
    let attacked = 
      Play.enterDungeon maxDice
      |> Play.goNorth.go
    attacked.player.health |> should lessThan 100
    let notAttacked = 
      Play.enterDungeon (fixedDice 6)
      |> Play.goNorth.go
    notAttacked.player.health |> should equal 100

  [<Fact>]
  let ``When player attacks and the inhabitant is not dead then it attacks back immediately`` () =
    let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 50}
    let updated = roomWithMonster |> Play.attack.go
    updated.player.health |> should lessThan 100

  [<Fact>]
  let ``When player is attacked the message reflects the damage inflicted`` () =
    let roomWithMonster = dungeon |> Builder.withInhabitant {Person.empty with health = 50}
    let updated = roomWithMonster |> Play.attack.go
    updated.message |> should equal "No-one attacks and causes you 1 damage"

  [<Fact>]
  let ``When player is killed the game is over`` () =
    let almostDead = 
      dungeon 
      |> Builder.withInhabitant {Person.empty with health = 50}
      |> fun x -> {x with player = {x.player with health = 1}}
    let updated = almostDead |> Play.attack.go
    updated.player.health |> should equal 0
    updated.player.isDead |> should equal true
    updated.gameOver |> should equal true