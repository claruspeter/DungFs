module DungFs
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
  people: Person list
} with 
  static member empty exits = {Room.exits = exits; gold=0; people = []}


type Dungeon = {
  player: Person
  here: Room
}

let enterDungeon() =
  {
    player = Person.empty
    here = {exits = [ N ]; gold = 3; people = []}
  }

let increaseGold amount (person:Person) =
  { person with gold = person.gold + amount }

let takeDamage amount (person:Person) =
  { person with health = Math.Max(0, person.health - amount) }

let pickupGold (dungeon: Dungeon) =
    {
      dungeon with 
        player = dungeon.player |> increaseGold dungeon.here.gold
        here = {dungeon.here with gold = 0}
    }
